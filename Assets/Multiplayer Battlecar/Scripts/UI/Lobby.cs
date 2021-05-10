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
        private bool assigningToRight = true;

        public void OnPlayerConnected(BattlecarsPlayerNet player)
        {
            bool assigned = false;

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
                    slot.AssignPlayer(player);
                    assigned = true;
                }
            });

            //Flip the flag so that the next one will end up in the other list.
            assigningToLeft = !assigningToLeft;
        }

        void Start()
        {
            leftTeamSlots.AddRange(leftTeamHolder.GetComponentsInChildren<LobbyPlayerSlot>());
            rightTeamSlots.AddRange(rightTeamHolder.GetComponentsInChildren<LobbyPlayerSlot>());
        }

        void Update()
        {

        }
    }
}
