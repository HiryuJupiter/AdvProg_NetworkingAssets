using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq; //Linq is lspw


namespace Battlecars.Networking
{
    public class BattlecarsNetworkManager : NetworkManager
    {
        #region Fields
        public Dictionary<byte, BattlecarsPlayerNet> players = new Dictionary<byte, BattlecarsPlayerNet>();

        /// <summary>
        /// A reference to the battlecars version of the network manager singleton.
        /// </summary>
        public static BattlecarsNetworkManager Instance => singleton as BattlecarsNetworkManager;
        /// <summary>
        /// Whether or not this Networkmanager is the host
        /// </summary>
        public bool IsHost { get; private set; } = false;
        /// <summary>
        /// Runs only when connecting to an online scene as a host
        /// </summary>
        #endregion

        public override void OnStartHost  ()
        {
            IsHost = true;
        }

        /// <summary>
        /// Attempts to return a player corresponding to the passed id.
        /// If no player found, return null (which is concerning)
        /// </summary>
        public BattlecarsPlayerNet GetPlayerForID(byte _playerID)
        {
            BattlecarsPlayerNet player;
            players.TryGetValue(_playerID, out player); //TryGetValue returns null when it doesn't have a thing.
            return player;
        }

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

        protected void AssignPlayerId(GameObject _player)
        {
            byte id = 0;
            List<byte> playerIDs = players.Keys.OrderBy(x => x).ToList();
            //List<byte> = players.Keys.byte(x => x).ToList();

            foreach (byte key in playerIDs)
            {
                if (id == key)
                    id++;
            }

            //Get the playernet componn==t from the gameobject and asssign its plkayed
            BattlecarsPlayerNet player = _player.GetComponent<BattlecarsPlayerNet>();
            player.playerID = id;
            players.Add(id, player);
        }

        //Removes the player from the dictionary
        public void RemovePlayer(byte _id)
        {
            //If the player is in the dictionary, remorit
            if (players.ContainsKey(_id))
            {
                players.Remove(_id);
            }
        }

        public void AddPlayer(BattlecarsPlayerNet _player)
        {
            if(!players.ContainsKey(_player.playerID))
            {
                players.Add(_player.playerID, _player);
            }
        }
    }
}
