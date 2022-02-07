using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableRespawmer : MonoBehaviour
{
    [SerializeField] private List<GameObject> collectables;

    public void RespawmCollectables()
    {
        foreach(GameObject collectable in collectables)
        {
            collectable.SetActive(true);
            collectable.transform.position = new Vector3
                (collectable.transform.position.x,
                Random.Range(0, 6) * 6f + 10f,
                collectable.transform.position.z);
        }
    }
}
