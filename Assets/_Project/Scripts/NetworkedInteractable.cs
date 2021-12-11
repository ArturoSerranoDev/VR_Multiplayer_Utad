using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedInteractable : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    internal XRBaseInteractable m_InteractableBase;
    internal Player owner;
    internal Rigidbody rigidBody;
    internal XRController interactingController;
    private bool isBeingHeld;
    public UnityEvent RpcActivate;
    public UnityEvent RpcDeactivate;
    public UnityEvent RpcSelect;
    public bool useGravity = true;
    public UnityEvent RpcDeselect;

    private Transform entityTransform;

// Start is called before the first frame update
    void Awake()
    {
        m_InteractableBase = GetComponent<XRBaseInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        entityTransform = GetComponent<Transform>();

        m_InteractableBase.onSelectEntered.AddListener(OnSelectEnter);
        m_InteractableBase.onSelectExited.AddListener(OnSelectExit);
        m_InteractableBase.onActivate.AddListener(OnActivate);
        m_InteractableBase.onDeactivate.AddListener(OnDeactivate);
    }

    void Update()
    {
        if (isBeingHeld)
        {
            if (rigidBody)
                rigidBody.isKinematic = true;
            //gameObject.layer = LayerMask.NameToLayer("InHand");
        }

        else
        {
            if (rigidBody && useGravity)
                rigidBody.isKinematic = false;
            //gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
    }

    void TransferOwnership()
    {
        photonView.RequestOwnership();
    }

    public void OnSelectEnter(XRBaseInteractor obj)
    {
        print("Interactor: " + obj.name);
        interactingController = obj.GetComponent<XRController>();
        print("NetworkedInteractable -> OnSelectEnter");
        if (photonView && PhotonNetwork.InRoom)
        {
            photonView.RPC("RpcOnSelect", RpcTarget.AllBuffered);
            if (photonView.Owner != PhotonNetwork.LocalPlayer)
            {
                TransferOwnership();
            }
        }
        else
        {
            RpcOnSelect();
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

        interactingController = null;
    }

    public void OnDeactivate(XRBaseInteractor obj)
    {
        if (photonView && PhotonNetwork.InRoom)
            photonView.RPC("RpcOnDeactivate", RpcTarget.AllBuffered);
        else
            RpcOnDeactivate();
    }

    public void OnActivate(XRBaseInteractor obj)
    {
        if (photonView && PhotonNetwork.InRoom)
            photonView.RPC("RpcOnActivate", RpcTarget.AllBuffered);
        else
            RpcOnActivate();
    }

    [PunRPC]
    public void RpcOnSelect()
    {
        isBeingHeld = true;
        RpcSelect.Invoke();
    }

    [PunRPC]
    public void RpcOnDeselect()
    {
        isBeingHeld = false;
        print("RPC Deselect");
        RpcDeselect.Invoke();
    }

    [PunRPC]
    public void RpcOnActivate()
    {
        RpcActivate.Invoke();
    }

    [PunRPC]
    public void RpcOnDeactivate()
    {
        RpcDeactivate.Invoke();
    }


    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != photonView)
        {
            return;
        }

        print("Ownership Requested for: " + targetView.name + " from " + requestingPlayer.NickName);
        photonView.TransferOwnership(requestingPlayer);
        owner = requestingPlayer;
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        print("Ownership Transfered. New Owner: " + targetView.Owner.NickName);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(entityTransform.position);
            stream.SendNext(entityTransform.eulerAngles);
        }
        else
        {
            this.entityTransform.position = (Vector3)stream.ReceiveNext();
            this.entityTransform.eulerAngles = (Vector3)stream.ReceiveNext();
        }
    }
}