using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public GameObject gunPrefab;

        [SerializeField] private GameObject playerSpawnPoint1;
        [SerializeField] private GameObject playerSpawnPoint2;
        [SerializeField] private GameObject gunSpawnPoint;
        #region Photon Callbacks

        private void Start()
        {
            if (NetworkedPlayer.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate

                Vector3 spawnPos = Vector3.zero;

                //if (PhotonNetwork.PlayerList.Length == 1)
                //{
                //    spawnPos = spawnPoint1.transform.position;
                //    PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPos, Quaternion.identity, 0);

                //}
                //else if (PhotonNetwork.PlayerList.Length == 2)
                //{
                //    spawnPos = spawnPoint2.transform.position;
                //    PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPos, Quaternion.identity, 0);
                //}
                //else
                //{
                //    // Spectators
                //}

                spawnPos = playerSpawnPoint1.transform.position;
                PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPos, Quaternion.identity, 0);
                PhotonNetwork.Instantiate(this.gunPrefab.name, gunSpawnPoint.transform.position, Quaternion.identity, 0);

            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
            
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            }
        }
        
        #endregion

        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion
        
    
    }
}