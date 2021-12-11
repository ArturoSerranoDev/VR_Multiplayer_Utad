using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedSharedTransform : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Transform entityTransform;

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
