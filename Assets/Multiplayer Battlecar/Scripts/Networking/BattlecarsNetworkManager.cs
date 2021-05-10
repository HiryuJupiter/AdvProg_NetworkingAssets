using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq; //Linq is lspw


namespace Battlecars.Networking
{
    public class BattlecarsNetworkManager : NetworkManager
    {
        /// <summary>
        /// A reference to the battlecars version of the network manager singleton.
        /// </summary>
        public static BattlecarsNetworkManager Instance => singleton as BattlecarsNetworkManager;

        /// <summary>
        /// Whether or not this Networkmanager is the host
        /// </summary>
        public bool IsHost { get; private set; } = false;

        //To set this on the host...

        /// <summary>
        /// Runs only when connecting to an online scene as a host
        /// </summary>
        public override void OnStartHost  ()
        {
            IsHost = true;
        }

        public Dictionary<byte, BattlecarsPlayerNet> players = new Dictionary<byte, BattlecarsPlayerNet>();

        //Custom player spawning system.
        //Runs when a client connects to the server. This function is responsible for creating the player object and placing it in the center.
        //When playeer first joins the server.
        public override void OnServerAddPlayer (NetworkConnection connection)
        {
            //Give us the next spawn position depending onthe spawnMode
            Transform spawnPos = GetStartPosition();

            //Spawn a player and tyr to use the spawnPos
            GameObject playerObj = spawnPos != null ?
                Instantiate(playerPrefab, spawnPos.position, spawnPos.rotation) :
                Instantiate(playerPrefab);

            //Assign the player ID and ad them to the server based on the coonection
            AssignPlayerId(playerObj);
            //Associates the player GameObject ot the network connection on the server
            NetworkServer.AddPlayerForConnection(connection, playerObj);
        }

        //Removes the player from the dictionary
        public void RemovePlayer (byte _id)
        { 
            //If the player is in the dictionary, remorit
            if (players.ContainsKey(_id))
            {
                players.Remove(_id);
            }
        }

        protected void AssignPlayerId(GameObject gameObject)
        {
            byte id = 0;
            List<byte> playerIDs = players.Keys.OrderBy(x => x).ToList();
            //List<byte> = players.Keys.byte(x => x).ToList();

            foreach (byte key in players.Keys)
            {
                if (id == key)
                    id++;
            }

            //Get the playernet componn==t from the gameobject and asssign its plkayed
            BattlecarsPlayerNet player = playerPrefab.GetComponent<BattlecarsPlayerNet>();
            player.playerID = id;
        }
    }
}
