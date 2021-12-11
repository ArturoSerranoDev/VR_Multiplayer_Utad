using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class PlayerRespawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] public NetworkedPlayer myPlayer;

    // Start is called before the first frame update
    void OnEnable()
    {
        //myPlayer.onPlayerHit += DestroyAndRespawn;
    }

    public void DestroyAndRespawn(NetworkedPlayer myPlayer)
    {
        if (myPlayer.photonView.Owner != photonView.Owner)
        {
            return;
        }

        PhotonNetwork.Destroy(myPlayer.photonView);

        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(1f);
        Vector3 spawnPos = new Vector3(Random.Range(-20, 20), 0, Random.Range(0, 20));
        PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPos, Quaternion.identity, 0);
    }
}
