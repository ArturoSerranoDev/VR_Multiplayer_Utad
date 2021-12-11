using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRController = UnityEngine.InputSystem.XR.XRController;

public class NetworkedPlayer : MonoBehaviourPunCallbacks,IPunObservable
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [SerializeField]
    private XROrigin playerXROrigin;
    [SerializeField] private ActionBasedController leftControllerInput;
    [SerializeField] private ActionBasedController rightControllerInput;
    [SerializeField] private ContinuousMoveProviderBase moveProvider;
    [SerializeField] private SnapTurnProviderBase turnProvider;
    [SerializeField] private Camera camera;
    
    
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    private List<XRController> controllers;
    
    // Start is called before the first frame update
    void Awake()
    {
        // #Important
// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            NetworkedPlayer.LocalPlayerInstance = this.gameObject;
        }
// #Critical
// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
        

        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            leftControllerInput.enabled = false;
            rightControllerInput.enabled = false;
            playerXROrigin.enabled = false;
            moveProvider.enabled = false;
            turnProvider.enabled = false;
            camera.enabled = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerTransform.position);
            stream.SendNext(playerTransform.eulerAngles);
            
            stream.SendNext(leftHandTransform.position);
            stream.SendNext(rightHandTransform.position);
            
            stream.SendNext(leftHandTransform.eulerAngles);
            stream.SendNext(rightHandTransform.eulerAngles);
        }
        else
        {
            this.playerTransform.position = (Vector3)stream.ReceiveNext();
            this.playerTransform.eulerAngles = (Vector3)stream.ReceiveNext();

            this.leftHandTransform.position = (Vector3)stream.ReceiveNext();
            this.rightHandTransform.position = (Vector3)stream.ReceiveNext();

            this.leftHandTransform.eulerAngles = (Vector3)stream.ReceiveNext();
            this.rightHandTransform.eulerAngles = (Vector3)stream.ReceiveNext();
        }        
    }
}