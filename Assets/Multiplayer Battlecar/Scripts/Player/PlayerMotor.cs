using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlecars.Player
{

    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private float speed = 3;

        private bool isSetup = false;

        public void Setup ()
        {
            isSetup = true;
        }

        private void Update()
        {
            if (!isSetup)
                return;


            //If we are local player then move
            transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
            transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        }
    }
}
