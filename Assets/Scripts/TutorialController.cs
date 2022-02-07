using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialFloor;
    [SerializeField] private GameObject firstHalf;
    
    void Start()
    {
        StartCoroutine("KillTutorial");
    }

    /**
     * Desactiva el tutorial despues de un rato
     * */
    IEnumerator KillTutorial()
    {
        yield return new WaitForSeconds(35);
        Debug.Log("Se supone que pasan cosas");
        tutorialFloor.SetActive(false);
        firstHalf.SetActive(true);
        yield return new WaitForSeconds(9);
        gameObject.SetActive(false);
        yield return null;
    }
}
