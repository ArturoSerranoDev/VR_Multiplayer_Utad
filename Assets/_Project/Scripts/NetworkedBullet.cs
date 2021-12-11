using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedBullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * 10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("BulletOnCollision");
        PhotonNetwork.Destroy(GetComponent<PhotonView>());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("BulletOnCollision");
        PhotonNetwork.Destroy(GetComponent<PhotonView>());
    }
}
