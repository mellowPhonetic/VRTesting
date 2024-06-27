using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MellowVR
{
    public class GrabDriver : InteractionBase
    {
        [HideInInspector]
        public InputAction[] grab = {null, null};
        
        [HideInInspector]
        public GrabbableObject[] touchedObject = {null, null};

        [HideInInspector]
        public GrabbableObject[] grabbedObject = {null, null};

        [HideInInspector]
        public Transform[] hand = {null, null};

        private void OnEnable()
        {
            grab[(int)Side.Left] = playerControls.Grab.LeftHand;
            grab[(int)Side.Right] = playerControls.Grab.RightHand;

            grab[(int)Side.Left].Enable();
            grab[(int)Side.Right].Enable();

        }

        private void OnDisable()
        {
            grab[(int)Side.Left].Disable();
            grab[(int)Side.Right].Disable();

        }


        void Grab()
        {
            switch (interactionSettings.grabbableHands)
            {
                case Side.Left:
                    Grab((int)Side.Left);
                    break;
                case Side.Right:
                    Grab((int)Side.Right);
                    break;
                default: // case Side.Both();
                    Grab((int)Side.Left);
                    Grab((int)Side.Right);
                    break;
            }
        }

        void Grab(int side)
        {
            if (grabbedObject[side] == null && grab[side].ReadValue<float>() > .1f)
            {
                SetGrab(side);
            }
            else if (grabbedObject[side] != null && grab[side].ReadValue<float>() < .1f)
            {
                SetRelease(side);
            }
        }

        void SetGrab(int side)
        {
            FindObject(side);
            if (touchedObject[side] != null)
            {
                if (interactionSettings.vibrateOnGrab) {interactionSettings.hapticsDriver.SendHaptics(side);}
                grabbedObject[side] = touchedObject[side];
                grabbedObject[side].Grabbed(this, hand[side]);
            }
            touchedObject[side] = null;
        }

        public void SetRelease(int side)
        {
            grabbedObject[side].Released();
            grabbedObject[side] = null;
        }

        /// <summary>
        /// Find objects grabbable by the left controller
        /// </summary>
        void FindObject(int side)
        {
            Collider[] collisions = Physics.OverlapSphere(hand[side].position, .05f);
            foreach (Collider collision in collisions)
            {
                GrabbableObject grabbableObject = collision.gameObject.GetComponent<GrabbableObject>();
                if (grabbableObject != null && (touchedObject[side] == null || grabbableObject.priority > touchedObject[side].priority))
                {
                    touchedObject[side] = grabbableObject;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            hand[(int)Side.Left] = interactionSettings.rig.leftHand;
            hand[(int)Side.Right] = interactionSettings.rig.rightHand;
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate()
        {
            switch (interactionSettings.canGrab)
            {
                case true:
                    Grab();
                    break;
                default:
                    break;
            }
        }
    }
}