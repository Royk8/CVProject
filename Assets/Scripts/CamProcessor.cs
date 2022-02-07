using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using System;
using System.Linq;

public class CamProcessor : MonoBehaviour
{
    private WebCamTexture camTex;
    [SerializeField] private Material cameraRT;
    private Texture2D tex;
    private Mat imat, rmat, strel;
    private Point strpoint;
    [SerializeField] private Camera cam, secondCamera;
    [SerializeField] private GameObject greenMarker, redMarker;
    [SerializeField] private RawImage rend;
    [SerializeField] private RawImage shot;
    [SerializeField] private int channel = 0, erodes, dilates;
    [SerializeField] [Range(0, 255)] private double downValue, upValue;
    [SerializeField] [Range(2, 255)] private int value2 = 3;
    [SerializeField] private ThresholdTypes tipo;
    [SerializeField] private int x = 500, y = 500;

    /**
     * Se ejecuta al inicio.
     * Busca los dispositivos de camara conectados
     * Instancio una matriz y un punto para usar como elemento estructural en el metodo Open. 
     */
    private void Start()
    {
        SetUpCamera();
        tex = new Texture2D(rend.texture.width, rend.texture.height);
        strel = new Mat();
        strpoint = new Point(-1, -1);
    }

    /**
     * Metodo que se ejecuta en cada frame, en el se procesa la imagen de la camara 
     */
    private void Update()
    {
        shot.texture = ProcessTexture();
    }

    /**
     * Procesa la textura y le hace las transformaciones necesarias para el juego
     */
    public Texture2D ProcessTexture()
    {
        //Transforma la textura de la camara de unity en una matriz para OpenCV
        imat = OpenCvSharp.Unity.TextureToMat(camTex);
        //Paso al espacio de color LAB
        imat = imat.CvtColor(ColorConversionCodes.RGB2Lab);
        //Volteo la imagen para que coincida con las coordenadas de unity.
        imat = imat.Flip(FlipMode.Y);
        //Extraigo el canal L para B para identificar el marcador rojo
        rmat = imat.ExtractChannel(2);
        //Extraigo el canal L para A para identificar el marcador verde
        imat = imat.ExtractChannel(1);
        //Eliminamos las zonas con un valor de verde o rojo menor para dejar solo el marcador
        //Valores del threshold obtendidos experimentalmente
        rmat = rmat.Threshold(100, 0, ThresholdTypes.TozeroInv);                  
        imat = imat.Threshold(104, 0, ThresholdTypes.TozeroInv);
        //Open ambas imagenes con el numero de iteracciones necesarias
        rmat = OpenMat(rmat, 10, 10);
        imat = OpenMat(imat, 5, 5);
        // Buscamos el punto donde se debe ubicar el marcador (El mayor de la zona)
        Point greenSpot = FindMarkerPosition(imat);
        Point redSpot = FindMarkerPosition(rmat);
        //Dibujamos el marcador en pantalla
        PutMarkerInScreen(redSpot, redMarker);
        PutMarkerInScreen(greenSpot, greenMarker);
        //Retornamos la textura con todas las modificaciones para ser proyectada
        return OpenCvSharp.Unity.MatToTexture(rmat, tex);
    }

    /**
     * Aplica Open a la matriz con el numero de erodes y dilares de los parametros
     * */
    private Mat OpenMat(Mat mat, int erodes, int dilates)
    {
        mat = mat.Erode(strel, strpoint, erodes, BorderTypes.Constant, null);
        mat = mat.Dilate(strel, strpoint, dilates, BorderTypes.Constant, null);
        return mat;
    }

    /**
     * Usa el metodo minmaxloc, que encuentra el punto de la matriz con mayoy y con menor valor.
     * Retorna la localizacion del mayor valor
     * */
    private Point FindMarkerPosition(Mat mat)
    {
        Point spot = new Point(0, 0);
        Point temp = new Point(0, 0);
        double tempd = 0;
        mat.MinMaxLoc(out tempd, out tempd, out temp, out spot);
        return spot;
    }

    /*
     * Transforma la ubicacion del punto en el mundo 2D (de la interfaz) al 3D del mundo
     */
    private void PutMarkerInScreen(Point spot, GameObject marker)
    {
        if (spot.X > 0 && spot.Y > 0)
        {
            marker.SetActive(true);
            Vector3 pos = cam.ScreenToWorldPoint(new Vector3(spot.X, Screen.height - spot.Y, 75f));
            marker.transform.position = pos;
        }
        else
        {
            marker.SetActive(false);
        }
    }

    /**
     * Escoge el primer dispositivo de camara para usarlo como webcam
     * Estaria bien mejorarlo para que encuentre un dispositivo que funcione
     */
    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        camTex = new WebCamTexture(devices[0].name);
        rend.texture = camTex;
        cameraRT.mainTexture = camTex;
        camTex.Play();
    }
}
