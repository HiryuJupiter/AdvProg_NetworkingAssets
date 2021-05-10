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
        [SyncVar]
        public byte playerID; //A byte is unsigned (only positive) and goes between 0~255. You only need more in MMO
        [SyncVar]
        public string username = ""; //

        private Lobby lobby;
        private bool hasJoinedLobby = false;

        private void Update()
        {
            //Get the lobby 
            if (lobby == null && !hasJoinedLobby)
                lobby = FindObjectOfType<Lobby>();

            //
            if (lobby != null && !hasJoinedLobby)
            {
                hasJoinedLobby = true;
                lobby.OnPlayerConnected(this);
            }
        }

        //Runs only when the object is connected to the lcal player
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            SceneManager.LoadSceneAsync("InGameMenus", LoadSceneMode.Additive);
        }

        [Command] //Must be void, must start with Cmd
        public void CmdSetUsername (string _name)
        {
            username = _name;
        }

        public void SetUesrname (string _name)
        {
            if (isLocalPlayer)
            {
                //Only localplayers can call ocmmands as local players 
                //are the only one who have the authority to talk to the server
                CmdSetUsername(_name);
            }
        }

        //public override void OnStartClient()
        //{
        //    Lobby lobby = FindObjectOfType<Lobby>();
        //}

        //Clean up
        //Runs whne the client is disconnected from the server.
        public override void OnStopClient()
        {
            //Remove the playerID from the server
            BattlecarsNetworkManager.Instance.RemovePlayer(playerID);
        }
    }
}
