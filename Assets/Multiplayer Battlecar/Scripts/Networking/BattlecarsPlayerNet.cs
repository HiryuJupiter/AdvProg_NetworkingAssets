using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Battlecars.UI;

namespace Battlecars.Networking
{
    public class BattlecarsPlayerNet : NetworkBehaviour
    {
        //Player data
        [SyncVar]
        public byte playerID; //A byte is unsigned (only positive) and goes between 0~255. You only need more in MMO
        [SyncVar]
        public string username = ""; //


        //Lobby status
        private Lobby lobby;
        private bool hasJoinedLobby = false;

        private void Update()
        {
            //Determine if we are on the host client
            if (BattlecarsNetworkManager.Instance.IsHost)
            {
                //Get the lobby 

                if (!hasJoinedLobby)
                {
                    if (lobby == null)
                        lobby = FindObjectOfType<Lobby>();

                    if (lobby != null)
                    {
                        hasJoinedLobby = true;
                        lobby.OnPlayerConnected(this);
                    }
                }
            }
        }

        #region Starting and ending client
        public override void OnStartClient()
        {
            BattlecarsNetworkManager.Instance.AddPlayer(this);
        }

        //Runs only when the object is connected to the lcal player
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            SceneManager.LoadSceneAsync("InGameMenus", LoadSceneMode.Additive);
        }

        //Clean up
        //Runs whne the client is disconnected from the server.
        public override void OnStopClient()
        {
            //Remove the playerID from the server
            BattlecarsNetworkManager.Instance.RemovePlayer(playerID);
        }
        #endregion


        public void SetUesrname(string _name)
        {
            if (isLocalPlayer)
            {
                //Only localplayers can call ocmmands as local players 
                //are the only one who have the authority to talk to the server
                CmdSetUsername(_name);
            }
        }

        public void AssignPlayerToSlot(bool _left, int _slotID, byte _playerID)
        {
            if (isLocalPlayer)
            {
                CmdAssignPlayerToLobbySlot(_left, _slotID, _playerID);
            }
        }

        //Commands are Messages to the server from a single person
        #region Commands
        [Command] //Must be void, must start with Cmd
        public void CmdSetUsername(string _name)
        {
            username = _name;
        }

        [Command]
        public void CmdAssignPlayerToLobbySlot(bool _left,  int _slotID, byte _playerID) => RpcAssignPlayerToLobbySlot(_left, _slotID, _playerID);
        #endregion

        #region RPCs (Remote procedure calls - runs on every instance ofthe object. Player 1 runs it on his pc, player 2 ... etc.
        [ClientRpc]
        public void RpcAssignPlayerToLobbySlot(bool _left, int _slotID, byte _playerID)
        {
            //If this is running on the host client, we don't need to set the player to the slot, so just ignore this call.
            if (BattlecarsNetworkManager.Instance.IsHost)
                return;

            //Find the lobby in the scene and let the player to the correct hint.
            //Lobby lobby = FindObjectOfType<Lobby>();
            //lobby.AssignPlayerToSlot(BattlecarsNetworkManager.Instance.GetPlayerForID(_playerID), _left, _slotID);
            StartCoroutine(AssignPlayerToLobbySlotDelayed(BattlecarsNetworkManager.Instance.GetPlayerForID(_playerID), _left, _slotID));
        }
        #endregion

        #region Coroutine
        private IEnumerator AssignPlayerToLobbySlotDelayed(BattlecarsPlayerNet _player, bool _left, int _slotID)
        {
            //Keep trying to get the lobby until it's not null
            Lobby lobby = FindObjectOfType<Lobby>();
            while (lobby == null)
            {
                yield return null;
                lobby = FindObjectOfType<Lobby>();
            }

            //Lobby successffly got, so assign the player
            lobby.AssignPlayerToSlot(_player, _left, _slotID);
        }
        #endregion
    }
}
