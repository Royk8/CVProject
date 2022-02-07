using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectableController : MonoBehaviour
{
    public event Action<CollectableController> OnPickup;

    private void OnEnable()
    {
        ScoreController.instance.AddCollectable(this);        
    }
    /*
     * Se activa cuando el personaje toca un item recolectable
     * */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup?.Invoke(this);
            gameObject.SetActive(false);            
        }
    }
}
