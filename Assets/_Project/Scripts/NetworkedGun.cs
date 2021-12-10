using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Unity.Mathematics;

public class NetworkedGun : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private XRGrabInteractable gunInteractable;
    [SerializeField] private GameObject barrelEndPoint;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        gunInteractable.activated.AddListener(ShootBullet);
    }
    
    void OnDisable()
    {
        gunInteractable.activated.RemoveListener(ShootBullet);
    }

    private void ShootBullet(ActivateEventArgs arg0)
    {
        PhotonNetwork.Instantiate("Bullet_Networked", barrelEndPoint.transform.position, quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if (stream.IsWriting)
        // {
        //     // We own this player: send the others our data
        //     stream.SendNext(IsFiring);
        // }
        // else
        // {
        //     // Network player, receive data
        //     this.IsFiring = (bool)stream.ReceiveNext();
        // }
    }
}
