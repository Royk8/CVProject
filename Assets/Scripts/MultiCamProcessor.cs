using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

public class MultiCamProcessor : MonoBehaviour
{
    private WebCamTexture camTex;
    private Texture2D tex, texg, texlab;
    private Mat imat, rmat, strel;
    private Point strpoint;
    [SerializeField] private Camera cam, secondCamera;
    [SerializeField] private RawImage rend;
    [SerializeField] private RawImage redShot, greenShot, LABShot;
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
        texg = new Texture2D(rend.texture.width, rend.texture.height);
        texlab = new Texture2D(rend.texture.width, rend.texture.height);
        strel = new Mat();
        strpoint = new Point(-1, -1);
    }

    /**
     * Metodo que se ejecuta en cada frame, en el se procesa la imagen de la camara 
     */
    private void Update()
    {
        redShot.texture = ProcessTexture();
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
        Mat labmat = imat.Clone();
        LABShot.texture = OpenCvSharp.Unity.MatToTexture(labmat, texlab);
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
        greenShot.texture = OpenCvSharp.Unity.MatToTexture(imat, texg);
        //Retornamos la textura con todas las modificaciones para ser proyectada
        return OpenCvSharp.Unity.MatToTexture(rmat, tex);
    }

    private Mat OpenMat(Mat mat, int erodes, int dilates)
    {
        mat = mat.Erode(strel, strpoint, erodes, BorderTypes.Constant, null);
        mat = mat.Dilate(strel, strpoint, dilates, BorderTypes.Constant, null);
        return mat;
    }

    private Point FindMarkerPosition(Mat mat)
    {
        Point spot = new Point(0, 0);
        Point temp = new Point(0, 0);
        double tempd = 0;
        mat.MinMaxLoc(out tempd, out tempd, out temp, out spot);
        return spot;
    }

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

    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        camTex = new WebCamTexture(devices[0].name);
        rend.texture = camTex;
        camTex.Play();
    }














    /*public static string ScreenShotName(int width, int height)
{
    return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                         Application.dataPath,
                         width, height,
                         System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
}*/
}

/*
public Texture2D TestingCV()
{
    imat = OpenCvSharp.Unity.TextureToMat(camTex);
    /*Debug.Log(imat.Size());
    int[] dim = { imat.Size().Width / 2, imat.Size().Height / 2 };
    Debug.Log(dim[0] + " + " + dim[1]);
    Size nsize = new Size(imat.Size().Width / 2, imat.Size().Height / 2);
    imat = imat.Resize(nsize);
    Debug.Log(nsize);
    imat = imat.CvtColor(ColorConversionCodes.RGB2Lab);
    imat = imat.ExtractChannel(2).Threshold(104, 0, ThresholdTypes.TozeroInv);
    //imat = imat.Canny(30, 30, 3, false);
    //imat = imat.MedianBlur(value2).Flip(FlipMode.Y);
    imat = CloseMat(imat).Flip(FlipMode.Y);
    Point spot = new Point(0, 0);
    Point temp = new Point(0, 0);
    double tempd = 0;
    imat.MinMaxLoc(out tempd, out tempd, out temp, out spot);
    if (spot.X > 0 && spot.Y > 0)
    {
        marker.SetActive(true);
        //imat = imat.Resize(new Size(1920, 1080));
        Vector3 pos = cam.ScreenToWorldPoint(new Vector3(spot.X, Screen.height - spot.Y, marker.transform.position.z));
        marker.transform.position = pos;
        /*GameObject floorAdded = Instantiate(floor, marker.transform.position, Quaternion.identity);
        floorAdded.transform.SetParent(floorContainer.transform);
    }
    else
    {
        marker.SetActive(false);
    }
    int[] center = { y, x };
    SetRect(imat, center);
    //Debug.Log(spot);
    return OpenCvSharp.Unity.MatToTexture(imat, tex);
}
*/