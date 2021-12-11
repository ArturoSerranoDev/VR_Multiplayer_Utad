using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class HatSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> hatPrefabs;

    [SerializeField] List<Transform> hatSpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < hatSpawnPoints.Count; i++)
            {
                PhotonNetwork.Instantiate(prefabName: hatPrefabs[Random.Range(0, hatPrefabs.Count - 1)].name, 
                                          position: hatSpawnPoints[i].transform.position, 
                                          rotation: Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
