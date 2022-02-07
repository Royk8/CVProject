using UnityEngine;

public class MarkerController : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private MarkerColor markerColor;

    /**
     * Activa o desactiva los bloques de la malla dependiendo si es el marcador rojo o verde
     */
    void Update()
    {
        if(markerColor == MarkerColor.green)
        {
            grid.SetActiveBloqON(transform.position);
        }
        else
        {
            grid.SetActiveBloqOFF(transform.position);
        }       
    }

    enum MarkerColor
    {
        red,
        green
    }
}
