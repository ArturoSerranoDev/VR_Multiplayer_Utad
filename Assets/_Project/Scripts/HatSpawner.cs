using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class HatSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<GameObject> hatPrefabs;

    [SerializeField] private List<Transform> hatSpawnPoints;

    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private GameObject cameraSpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < hatSpawnPoints.Count; i++)
        {
            PhotonNetwork.Instantiate(prefabName: hatPrefabs[Random.Range(0, hatPrefabs.Count - 1)].name, 
                                        position: hatSpawnPoints[i].transform.position, 
                                        rotation: Quaternion.identity);
        }

        PhotonNetwork.Instantiate(prefabName: cameraPrefab.name,
                                        position: cameraSpawnPoint.transform.position,
                                        rotation: Quaternion.identity);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
