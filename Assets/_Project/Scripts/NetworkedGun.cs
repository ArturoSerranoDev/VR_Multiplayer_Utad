using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Unity.Mathematics;
using System;

public class NetworkedGun : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Rigidbody gunRigidbody;

    [SerializeField] private XRGrabInteractable gunInteractable;
    [SerializeField] private GameObject barrelEndPoint;

    [SerializeField] private Transform gunTransform;

    private bool isBeingHeld;

    void Awake()
    {
   
    }


    // Start is called before the first frame update
    void OnEnable()
    {
        gunInteractable.activated.AddListener(ShootBullet);

        gunInteractable.onSelectEntered.AddListener(OnSelectEnter);
        gunInteractable.onSelectExited.AddListener(OnSelectExit);
    }

    void OnDisable()
    {
        gunInteractable.activated.RemoveListener(ShootBullet);
    }

    private void Update()
    {
        gunRigidbody.isKinematic = isBeingHeld;
        gunRigidbody.useGravity = !isBeingHeld;
    }

    private void ShootBullet(ActivateEventArgs arg0)
    {
        PhotonNetwork.Instantiate("Bullet_Networked", barrelEndPoint.transform.position, barrelEndPoint.transform.rotation);
    }

    public void OnSelectEnter(XRBaseInteractor obj)
    {
        print("Interactor: " + obj.name);
        print("NetworkedInteractable -> OnSelectEnter");
        if (photonView && PhotonNetwork.InRoom)
        {
            photonView.RPC("UpdateAllThatWeaponIsHeld", RpcTarget.AllBuffered);
            if (photonView.Owner != PhotonNetwork.LocalPlayer)
            {
                photonView.RequestOwnership();
            }
        }
        else
        {
            UpdateAllThatWeaponIsHeld();
        }
    }
    public void OnSelectExit(XRBaseInteractor obj)
    {
        if (photonView && PhotonNetwork.InRoom)
        {
            photonView.RPC("RpcOnDeselect", RpcTarget.AllBuffered);
        }
        else
        {
            RpcOnDeselect();
        }

    }
    [PunRPC]
    public void UpdateAllThatWeaponIsHeld()
    {
        isBeingHeld = true;
    }

    [PunRPC]
    public void RpcOnDeselect()
    {
        isBeingHeld = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(gunTransform.position);
            stream.SendNext(gunTransform.eulerAngles);
        }
        else
        {
            // Network player, receive data
            this.gunTransform.position = (Vector3)stream.ReceiveNext();
            this.gunTransform.eulerAngles = (Vector3)stream.ReceiveNext();
        }
    }
}
