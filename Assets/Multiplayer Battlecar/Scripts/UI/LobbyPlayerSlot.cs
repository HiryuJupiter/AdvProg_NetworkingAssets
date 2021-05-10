using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Battlecars.Networking;

namespace Battlecars.UI
{
    //we want to assign a player to the slot. If no player is active, then 
    //dont have it active
    public class LobbyPlayerSlot : MonoBehaviour
    {
        public bool IsTaken => player != null;
        public BattlecarsPlayerNet Player => player;
        public bool IsLeft { get; private set; } = false;

        [SerializeField]
        private TextMeshProUGUI nameDisplay;
        [SerializeField]
        private Button playerButton;

        private BattlecarsPlayerNet player;

        //Set the player in this to the passed player
        public void AssignPlayer(BattlecarsPlayerNet _player)
        {
            player = _player;
        }

        public void SetSide (bool _left)
        {
            IsLeft = _left;
        }

        void Update()
        {
            //If the slot is empty then the button shouldn't be active.
            playerButton.interactable = IsTaken;

            //If the player is set, then display their name, otherwise disoplay "Awaiting Player"
            nameDisplay.text = IsTaken ? GetPlayerName() : "Awaiting Player"; 
        }

        private string GetPlayerName ()
        {
            return string.IsNullOrEmpty(player.username) ? $"Player {player.playerID + 1}" : player.username;
        }
    }
}
