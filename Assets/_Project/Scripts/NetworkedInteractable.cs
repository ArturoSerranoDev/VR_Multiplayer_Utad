using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedInteractable : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks, IPunObservable
{
    [SerializeField] private Rigidbody rigidbody;

    [SerializeField] private XRGrabInteractable grabInteractable;

    [SerializeField] private Transform gunTransform;

    private bool isBeingHeld;

    void Awake()
    {

    }


    // Start is called before the first frame update
    void OnEnable()
    {

        grabInteractable.onSelectEntered.AddListener(OnSelectEnter);
        grabInteractable.onSelectExited.AddListener(OnSelectExit);
    }

    void OnDisable()
    {
        grabInteractable.onSelectEntered.RemoveListener(OnSelectEnter);
        grabInteractable.onSelectExited.RemoveListener(OnSelectExit);
    }

    private void Update()
    {
        rigidbody.isKinematic = isBeingHeld;
        rigidbody.useGravity = !isBeingHeld;
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

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }
}