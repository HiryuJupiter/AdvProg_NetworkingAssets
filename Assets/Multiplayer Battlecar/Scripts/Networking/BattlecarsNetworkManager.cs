using UnityEngine;
using Mirror;

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
    }
}
