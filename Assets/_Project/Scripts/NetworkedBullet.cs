using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedBullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().AddForce(transform.forward * 10);
    }

    private void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * 3, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        Debug.Log("BulletOnCollision");
        PhotonNetwork.Destroy(GetComponent<PhotonView>());
    }
}
