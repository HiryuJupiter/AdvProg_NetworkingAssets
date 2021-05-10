using System.Net;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlecars.Networking;
using Mirror;
using TMPro;

namespace Battlecars.UI
{
    public class ConnectionMenu : MonoBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button connectButton;
        [SerializeField] private TextMeshProUGUI ipText;

        [SerializeField] private DiscoveredGame gameTemplate;
        [SerializeField] private Transform foundGamesHolder;

        [SerializeField] private BattlecarsNetworkManager networkManager;

        private Dictionary<IPAddress, DiscoveredGame> discoveredGames = new Dictionary<IPAddress, DiscoveredGame>();

        void Start()
        {
            hostButton.onClick.AddListener(() => networkManager.StartHost());
            connectButton.onClick.AddListener(OnClickConnect);

            //Rebroadcast after connecting because you only have uri after connecting. 
            networkManager.discovery.onServerFound.AddListener(OnDetectServer);
            networkManager.discovery.StartDiscovery();
        }

        void Update()
        {

        }

        private void OnClickConnect()
        {
            networkManager.networkAddress = ipText.text.Trim((char)8203);
            networkManager.StartClient();
        }

        private void OnDetectServer(DiscoveryResponse _response)
        {
            //Here we have received a server that is broadcasting on teh network
            if (!discoveredGames.ContainsKey(_response.EndPoint.Address))
            {
                //We haven't already found a game with this IP, so make it
                DiscoveredGame game = Instantiate(gameTemplate, foundGamesHolder);
                game.gameObject.SetActive(true);

                //Setup the game using the response and add it to the list
                game.Setup(_response, networkManager);
                //game.Setup(_response.EndPoint.Address.ToString(), networkManager);
                discoveredGames.Add(_response.EndPoint.Address, game);
            }
            else
            {
                DiscoveredGame game = discoveredGames[_response.EndPoint.Address];
                if (game.Gamename != _response.gameName)
                {
                    game.UpdateResponse(_response);
                }
            }
        }
    }
}
