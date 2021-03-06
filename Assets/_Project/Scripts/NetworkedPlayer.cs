using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.XR;
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

    [SerializeField] private TrackedPoseDriver cameraTrackedPoseDriver;

    [SerializeField] private ContinuousMoveProviderBase moveProvider;
    [SerializeField] private SnapTurnProviderBase turnProvider;

    [SerializeField] private Camera camera;
    [SerializeField] private List<HitboxEventDispatcher> hitboxes;
    
    
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    [SerializeField] private Renderer headMeshRenderer;
    [SerializeField] private Renderer mixamoMeshRenderer;
    [SerializeField] private GameObject mixamoModel;

    [SerializeField] private bool isPlayerDead;

    private List<XRController> controllers;
    private bool colorSet;
    public event Action<NetworkedPlayer> onPlayerHit;

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
            camera.GetComponent<AudioListener>().enabled = false;
            cameraTrackedPoseDriver.enabled = false;
            headMeshRenderer.gameObject.layer = 0;
            mixamoModel.layer = 0;
            mixamoMeshRenderer.gameObject.layer = 0;
        }
    }

    private void OnEnable()
    {
        foreach (var hitbox in hitboxes)
        {
            hitbox.playerHitEvent += PlayerHit;
        }
    }

    private void OnDisable()
    {
        foreach (var hitbox in hitboxes)
        {
            hitbox.playerHitEvent -= PlayerHit;
        }
    }

    private void PlayerHit()
    {
        photonView.RPC("PlayerHitRPC", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    private void PlayerHitRPC(int playerID)
    {
        if (photonView.ViewID != playerID)
        {
            return;
        }

        isPlayerDead = true;
        onPlayerHit?.Invoke(this);
    }

    private void Start()
    {
        UpdateRandomHeadColor();

        //photonView.RPC("UpdateHeadColorInOthers", RpcTarget.OthersBuffered,
        //                                        headMeshRenderer.material.color.r,
        //                                        headMeshRenderer.material.color.g,
        //                                        headMeshRenderer.material.color.b);

        photonView.RPC("UpdateHeadColorInOthers", RpcTarget.OthersBuffered,
                                                mixamoMeshRenderer.material.color.r,
                                                mixamoMeshRenderer.material.color.g,
                                                mixamoMeshRenderer.material.color.b);
    }

    private void UpdateRandomHeadColor()
    {
        //headMeshRenderer.material.color = Random.ColorHSV();
        mixamoMeshRenderer.material.color = UnityEngine.Random.ColorHSV();
        colorSet = true;
    }

    [PunRPC]
    private void UpdateHeadColorInOthers(float red, float green, float blue)
    {
        if (!colorSet)
        {
            mixamoMeshRenderer.material.color = new Color(red, green, blue);
            colorSet = true;
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

            stream.SendNext(headTransform.position);
            stream.SendNext(headTransform.eulerAngles);
        }
        else
        {
            this.playerTransform.position = (Vector3)stream.ReceiveNext();
            this.playerTransform.eulerAngles = (Vector3)stream.ReceiveNext();

            this.leftHandTransform.position = (Vector3)stream.ReceiveNext();
            this.rightHandTransform.position = (Vector3)stream.ReceiveNext();

            this.leftHandTransform.eulerAngles = (Vector3)stream.ReceiveNext();
            this.rightHandTransform.eulerAngles = (Vector3)stream.ReceiveNext();

            this.headTransform.position = (Vector3)stream.ReceiveNext();
            this.headTransform.eulerAngles = (Vector3)stream.ReceiveNext();


        }        
    }
}
