using UnityEngine;

public class Grid : MonoBehaviour
{
    private float gridHeight;
    [SerializeField] private float gridWidth;
    private int nBloqsHeight, nBloqsWidth;
    [SerializeField] private GameObject bloq;
    private GameObject[,] bloqMatrix;
    [SerializeField] private float bloqSize;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform fullFloor;
    private float camCompensation = 68f;
    [SerializeField] private Transform cameraSwitch;

    void Start()
    {
        gridHeight = Screen.height;
        nBloqsHeight = (int) gridHeight / (int) bloqSize + 1;
        nBloqsWidth = (int) gridWidth / (int) bloqSize + 1;
        bloqMatrix = new GameObject[nBloqsWidth, nBloqsHeight];
        SpawnBloqs();
    }

    /**
     * Crea los bloques al principio del juego para no instancialos en runtime
     * Luego solo los activo y desactivo a placer.
     */
    private void SpawnBloqs()
    {
        float startPos = bloqSize / 2;
        for (int i = 0; i < nBloqsWidth; i++)
        {
            for (int j = 0; j < nBloqsHeight; j++)
            {
                Vector3 pos = new Vector3(startPos + i * bloqSize, startPos + j * bloqSize, camCompensation);
                GameObject newBloq = Instantiate(bloq,
                    (cam.ScreenToWorldPoint(pos)),
                    Quaternion.identity);
                newBloq.SetActive(false);
                bloqMatrix[i, j] = newBloq;
            }
        }
        Vector3 center = FindCenter();        
        fullFloor.transform.position = new Vector3(center.x + bloqMatrix[0,0].transform.position.x, 
            center.y + bloqMatrix[0, 0].transform.position.y, 0);
        for (int i = 0; i < nBloqsWidth; i++)
        {
            for (int j = 0; j < nBloqsHeight; j++)
            {
                bloqMatrix[i, j].transform.SetParent(fullFloor);
            }
        }
        transform.position = new Vector3(cameraSwitch.position.x, cameraSwitch.position.y, transform.position.z);
    }

    public void SetActiveBloqON(Vector3 markerPosition)
    {
        if (gameObject.activeSelf)
        {
            int[] index = ClosestBloq(cam.WorldToScreenPoint(markerPosition));
            GameObject bloq = bloqMatrix[index[0], index[1]];
            bloq.SetActive(true);
            bloq.GetComponent<SpriteChooser>().SetBeam(!IsThereSomethingUp(index));
            if (IsThereSomethingDown(index))
            {
                bloqMatrix[index[0], index[1] - 1].GetComponent<SpriteChooser>().SetBeam(false);
            }
        }
    }

    public void SetActiveBloqOFF(Vector3 markerPosition)
    {
        if (gameObject.activeSelf)
        {
            int[] index = ClosestBloq(cam.WorldToScreenPoint(markerPosition));
            bloqMatrix[index[0], index[1]].SetActive(false);
            if (IsThereSomethingDown(index))
            {
                bloqMatrix[index[0], index[1] - 1].GetComponent<SpriteChooser>().SetBeam(true);
            }
        }
    }

    private int[] ClosestBloq(Vector3 position)
    {        
        int[] index = { (int)(position.x / bloqSize), (int)(position.y / bloqSize) };
        return index;
    }

    /**
     * Verifica si hay bloques encima para saber que sprite usar
     * */
    private bool IsThereSomethingUp(int[] index)
    {
        if ((index[1] + 1) != nBloqsHeight)
        {
            return bloqMatrix[index[0], index[1] + 1].activeSelf;
        }
        return false;
    }

    /**
    * Verifica si hay bloques debajo para saber si es necesario cambiarlos
    * */
    private bool IsThereSomethingDown(int[] index)
    {
        if (index[1] != 0)
        {
            return bloqMatrix[index[0], index[1] - 1].activeSelf;
        }
        return false;
    }

    public Vector3 FindCenter()
    {
        Vector3 ll = bloqMatrix[0, 0].transform.position;
        Vector3 ul = bloqMatrix[0, nBloqsHeight - 1].transform.position;
        Vector3 ur = bloqMatrix[nBloqsWidth -1, nBloqsHeight - 1].transform.position;
        Vector3 lr = bloqMatrix[nBloqsWidth -1, 0].transform.position;
        return (ur - ll) / 2;
    }

    public void ResetBloqs()
    {
        for (int i = 0; i < nBloqsWidth; i++)
        {
            for (int j = 0; j < nBloqsHeight; j++)
            {
                bloqMatrix[i, j].SetActive(false);
            }
        }
    }
}
