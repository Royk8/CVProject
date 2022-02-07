using UnityEngine;

public class RandomnessController : MonoBehaviour
{
    public static RandomnessController instance;
    [SerializeField] private Transform firstHalf, secondHalf;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    /*
     * Mueve la primera mitad del mundo despues de superarla para que se vea distinto en la siguiente vuelta
     * */
    public void ShakeFirstHalf()
    {
        int rnd = Random.Range(1, 9);
        firstHalf.localPosition = new Vector3(firstHalf.localPosition.x,
            rnd * 5.5f, firstHalf.localPosition.z);
    }

    /*
    * Mueve la primera mitad del mundo despues de superarla para que se vea distinto en la siguiente vuelta
    * */
    public void ShakeSecondHalf()
    {
        int rnd = Random.Range(1, 7);
        secondHalf.localPosition = new Vector3(secondHalf.localPosition.x,
            rnd * 5.5f, secondHalf.localPosition.z);
    }
}
