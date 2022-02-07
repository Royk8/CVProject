using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReseterEnd : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnd;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onEnd?.Invoke();
        }
    }
}
