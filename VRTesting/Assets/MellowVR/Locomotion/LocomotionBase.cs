using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MellowVR
{
    public class LocomotionBase : MonoBehaviour
    {
        public MovementSettings movementSettings;
        public MellowControls playerControls;
        
        private void Awake()
        {
            playerControls = new MellowControls();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}