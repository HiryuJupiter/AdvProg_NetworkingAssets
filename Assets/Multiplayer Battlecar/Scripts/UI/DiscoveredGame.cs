using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Battlecars.Networking;

namespace Battlecars.UI
{


    [RequireComponent(typeof(Button))]
    public class DiscoveredGame : MonoBehaviour
    {
        public string Gamename => response.gameName;

        [SerializeField] private TextMeshProUGUI ipDisplay;

        private BattlecarsNetworkManager networkManager;
        private DiscoveryResponse response;



        //This script amangers the buttons that 'll appear.
        //When we detect a game, this will take the address, and then we can just click on the button

        public void Setup(DiscoveryResponse _response, BattlecarsNetworkManager _manager)
        {
            UpdateResponse(_response);
            ipDisplay.text = _response.EndPoint.Address.ToString();
            //ipDisplay.text = _response.EndPoint.Address.ToString();
            networkManager = _manager;

            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(JoinGame);
        }

        public void UpdateResponse(DiscoveryResponse _response)
        {
            response = _response;
            ipDisplay.text = $"<b>{response.gameName}</b>\n{response.EndPoint.Address}";
        }

        private void JoinGame ()
        {
            networkManager.networkAddress = ipDisplay.text.Trim((char)8203);
            networkManager.StartClient();
        }
    }
}
/*
  public void Setup(string _address, BattlecarsNetworkManager _manager)
        {
            ipDisplay.text = _address;

            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(JoinGame);
        }
     */
