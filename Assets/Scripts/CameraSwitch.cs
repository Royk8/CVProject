using System.Collections;
using UnityEngine;
using Cinemachine;

/**
 * Cambia la camara cuando el personaje llega a la zona de la webcam
 * */
public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcamStatic;
    [SerializeField] private GameObject worldCamera;
    [SerializeField] private GameObject fullFloor;
    [SerializeField] private CharacterMovement character;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vcamStatic.Priority = 11;
            StartCoroutine("ActivateFloor");
            RandomnessController.instance.ShakeSecondHalf();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vcamStatic.Priority = 9;
            StartCoroutine("DeactivateFloor");            
            character.Accelerate();
        }
    }

    IEnumerator ActivateFloor()
    {
        yield return new WaitForSeconds(2f);
        fullFloor.SetActive(true);
        fullFloor.transform.position = transform.position;
        yield return null;
    }

    IEnumerator DeactivateFloor()
    {
        yield return new WaitForSeconds(6f);
        fullFloor.SetActive(false);
        RandomnessController.instance.ShakeFirstHalf();
        yield return null;
    }

}
