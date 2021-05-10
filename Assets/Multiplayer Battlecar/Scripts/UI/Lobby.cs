using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlecars.Networking;


namespace Battlecars.UI
{
    public class Lobby : MonoBehaviour
    {
        private List<LobbyPlayerSlot> leftTeamSlots = new List<LobbyPlayerSlot>();
        private List<LobbyPlayerSlot> rightTeamSlots = new List<LobbyPlayerSlot>();

        [SerializeField]
        private GameObject leftTeamHolder;
        [SerializeField]
        private GameObject rightTeamHolder;

        //Flipping bool that determines which column the connected player will be added to
        private bool assigningToLeft = true;

        private BattlecarsPlayerNet localPlayer;

        public void AssignPlayerToSlot(BattlecarsPlayerNet _player, bool _left, int _slotID)
        {
            // Get the current slot list depending on the left param
            List<LobbyPlayerSlot> slots = _left ? leftTeamSlots : rightTeamSlots;
            // Assign the player to the releant slot in the list
            slots[_slotID].AssignPlayer(_player);
        }

        public void OnPlayerConnected(BattlecarsPlayerNet _player)
        {
            bool assigned = false;

            //If the player is the localPlayer, assign it.
            if (_player.isLocalPlayer)
            {
                localPlayer = _player;
            }

            List<LobbyPlayerSlot> slots = assigningToLeft ? leftTeamSlots : rightTeamSlots;

            //Loop through and run a lambda with the item at the index
            slots.ForEach(slot =>
            {
                //If we have assigned the value already, return from the lambda
                if (assigned)
                {
                    return; //This is break within a lambda break. There is no break in lambda looop.
                }
                else if (!slot.IsTaken)
                {
                    //If we haven't already assigned the player to a sllot and this slot havent been
                    //taken, then assign the player to this slot and flag as the slot been assigned.
                    slot.AssignPlayer(_player);
                    slot.SetSide(assigningToLeft);
                    assigned = true;
                }
            });

            for (int i = 0; i < leftTeamSlots.Count; i++)
            {
                LobbyPlayerSlot slot = leftTeamSlots[i];
                if (slot.IsTaken)
                    localPlayer.AssignPlayerToSlot(slot.IsLeft, i, slot.Player.playerID);
            }


            for (int i = 0; i < rightTeamSlots.Count; i++)
            {
                LobbyPlayerSlot slot = rightTeamSlots[i];
                if (slot.IsTaken)
                    localPlayer.AssignPlayerToSlot(slot.IsLeft, i, slot.Player.playerID);
            }

            //Flip the flag so that the next one will end up in the other list.
            assigningToLeft = !assigningToLeft;
        }

        void Start()
        {
            leftTeamSlots.AddRange(leftTeamHolder.GetComponentsInChildren<LobbyPlayerSlot>());
            rightTeamSlots.AddRange(rightTeamHolder.GetComponentsInChildren<LobbyPlayerSlot>());
        }

    }
}
