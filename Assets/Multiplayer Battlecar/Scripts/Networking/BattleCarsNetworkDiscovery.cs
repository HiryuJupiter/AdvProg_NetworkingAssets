using System;
using System.Net;
using Battlecars.UI;
using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.Events;

//URL tells you how to access it. URI is a reference for a page or document.
//URL is more generic and refers to the full address, URI point to the specific

/*
	Discovery Guide: https://mirror-networking.com/docs/Guides/NetworkDiscovery.html
    Documentation: https://mirror-networking.com/docs/Components/NetworkDiscovery.html
    API Reference: https://mirror-networking.com/docs/api/Mirror.Discovery.NetworkDiscovery.html
*/

namespace Battlecars.Networking
{
    [Serializable] public class ServerFoundEvent : UnityEvent<DiscoveryResponse> { }

    //The data and what we send to the client
    public class DiscoveryRequest : NetworkMessage
    {
        // Add properties for whatever information you want sent by clients
        // in their broadcast messages that servers will consume.

        //The name of the game being sent
        public string gameName;

    }

    //What we want to be received and converted by the client
    public class DiscoveryResponse : NetworkMessage
    {
        //The server that sent this message
        //this is a property so that it is not serialized but the client
        //fills this up after we receive it.
        public IPEndPoint EndPoint { get; set; }
        public Uri uri;
        public long serverID;

        //The name of the game being sent
        public string gameName;
        
    }

    public class BattleCarsNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
    {
        #region Server

        public long ServerID { get; private set; }
        [Tooltip("Transport to the advertised during discovery")]
        public Transport transport; //Allows the thing to go through the server

        [Tooltip("Invoked when a server is found")]
        public ServerFoundEvent onServerFound = new ServerFoundEvent();

        private Lobby lobby;

        //public override void Start()
        //{
        //    ServerID = RandomLong();

        //    //If the transport wasn't set in the inspector
        //    //find the active one. activeTransport is set in awake.
        //    if (transport == null)
        //        transport = Transport.activeTransport;

        //    base.Start();
        //}
        public override void Start()
        {
            ServerID = RandomLong();

            //If the transport wasn't set in the inspector
            //find the active one. activeTransport is set in awake.
            if (transport == null)
                transport = Transport.activeTransport;

            base.Start();
        }

        private void Update()
        {
            if (lobby = null)
                lobby = FindObjectOfType<Lobby>();
        }

        ///// <summary>
        ///// Reply to the client to inform it of this server
        ///// </summary>
        ///// <remarks>
        ///// Override if you wish to ignore server requests based on
        ///// custom criteria such as language, full server game mode or difficulty
        ///// </remarks>
        ///// <param name="request">Request coming from client</param>
        ///// <param name="endpoint">Address of the client that sent the request</param>
        protected override void ProcessClientRequest(DiscoveryRequest request, IPEndPoint endpoint)
        {
            base.ProcessClientRequest(request, endpoint);
        }

        /// <summary>
        /// Process the request from a client
        /// </summary>
        /// <remarks>
        /// Override if you wish to provide more information to the clients
        /// such as the name of the host player
        /// </remarks>
        /// <param name="_request">Request coming from client</param>
        /// <param name="_endpoint">Address of the client that sent the request</param>
        /// <returns>A message containing information about this server</returns>
        protected override DiscoveryResponse ProcessRequest(DiscoveryRequest _request, IPEndPoint _endpoint)
        {
            try
            {
                //This is just an example reply message,
                //you could add the game name here or game mode
                //if the player wants a specific game mode.
                return new DiscoveryResponse()
                {
                    serverID = ServerID,
                    uri = transport.ServerUri(),
                    gameName = lobby.LobbyName
                };
            }
            catch (NotImplementedException _e)
            {
                //Someone dun goofed, so let us know what happened
                Debug.LogError($"Transport {transport} does not support network discovery");
                throw;
            }
            return new DiscoveryResponse();
        }

        #endregion

        #region Client

        /// <summary>
        /// Create a message that will be broadcasted on the network to discover servers
        /// </summary>
        /// <remarks>
        /// Override if you wish to include additional data in the discovery message
        /// such as desired game mode, language, difficulty, etc... </remarks>
        /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
        protected override DiscoveryRequest GetRequest() => new DiscoveryRequest();

        /// <summary>
        /// Process the answer from a server
        /// </summary>
        /// <remarks>
        /// A client receives a reply from a server, this method processes the
        /// reply and raises an event
        /// </remarks>
        /// <param name="response">Response that came from the server</param>
        /// <param name="endpoint">Address of the server that replied</param>
        protected override void ProcessResponse(DiscoveryResponse _response, IPEndPoint _endpoint)
        {
            //We don't fully understand this code, we just know it's something we need to do.
            #region WTF
            //We received a messaged from the remote endpoint.
            _response.EndPoint = _endpoint;

            // Although we got a supposedly valid url, we may not be able to resolve the
            //provided host (i.e. may be an invalid connection). 
            //However, we know the real ip address of the server because
            //we just receive a packet from it, so use that as host. So we can convert it
            UriBuilder realUri = new UriBuilder(_response.uri)
            {
                Host = _response.EndPoint.Address.ToString()
            };
            _response.uri = realUri.Uri;
            #endregion

            onServerFound.Invoke(_response);
        }
        #endregion
    }
}