using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reseter : MonoBehaviour
{
    [SerializeField] private Transform begin;
    [SerializeField] private Transform player;
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject helpPlatform;
    [SerializeField] private CollectableRespawmer respawmer;
    private float heightOffset = 5.8f;

    /**
     * Envia al jugagor al principio del mundo cuando llega al final
     */
    public void ResetPlayer()
    {
        player.position = new Vector3(begin.position.x, begin.position.y + heightOffset, player.position.z);
        grid.ResetBloqs();
        helpPlatform.SetActive(false);
        respawmer.RespawmCollectables();
    }
}
