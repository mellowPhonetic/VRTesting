using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MellowVR
{
    public class TurnDriver : LocomotionBase
    {
        private InputAction turnContinuous;
        private InputAction turnSnap;
        private bool hasSnapped = false;

        private void OnEnable()
        {
            turnContinuous = playerControls.Turn.Continuous;
            turnSnap = playerControls.Turn.Snap;

            turnContinuous.Enable();
            turnSnap.Enable();
        }
        
        private void OnDisable()
        {
            turnContinuous.Disable();
            turnSnap.Disable();
        }


        /// <summary>
        /// Turns the player based on controller input
        /// Type of input used is determined by continuousRotation, snapRotation, and spaceRotation
        /// </summary>
        private void Turn() 
        {
            if (movementSettings.continuousRotation && !movementSettings.snapRotation) 
            {
                ContinuousTurn();

            }

            if (movementSettings.snapRotation)
            {
                SnapTurn();

            }
        }

        /// <summary>
        /// Continuously turns the player based on controller input
        /// </summary>
        private void ContinuousTurn()
        {
            float turnDirection = turnContinuous.ReadValue<Vector2>().x;
            
            movementSettings.rig.rb.transform.RotateAround(movementSettings.turnPivotTransform.position, movementSettings.rig.rb.transform.up, turnDirection * Time.deltaTime * movementSettings.turningSpeed);
            
            if (movementSettings.moveDriver.projecting)
            {
                movementSettings.projectionCameraPrefab.transform.RotateAround(movementSettings.projectionCameraPrefab.transform.GetChild(0).GetChild(0).position, movementSettings.projectionCameraPrefab.transform.up, turnDirection * Time.deltaTime * movementSettings.turningSpeed);
            }

        }

        /// <summary>
        /// Turns the player a set amount 
        /// </summary>
        private void SnapTurn()
        {
            Vector2 turnDirection = turnSnap.ReadValue<Vector2>();

            if (hasSnapped && turnDirection.x > -.1f && turnDirection.x < .1f && turnDirection.y > -.5f) // reset snapping
            {
                hasSnapped = false;
            }

            if (movementSettings.snapTurnAround && turnDirection.y < -.5f && !hasSnapped) // turn around
            {
                SnapTurn(180);
            }
            else if (turnDirection.x > .1f && !hasSnapped) // turn right
            {
                SnapTurn(movementSettings.snapTurnAngle);
            }
            else if (turnDirection.x < -.1f && !hasSnapped) // turn left
            {
                SnapTurn(-movementSettings.snapTurnAngle);
            }

        }

        /// <summary>
        /// Turns the player a set amount 
        /// </summary>
        private void SnapTurn(float turnAmount)
        {
            hasSnapped = true;
            movementSettings.rig.rb.transform.RotateAround(movementSettings.turnPivotTransform.position, movementSettings.rig.rb.transform.up, turnAmount);
            if (movementSettings.moveDriver.projecting) 
            {
                movementSettings.projectionCameraPrefab.transform.RotateAround(movementSettings.projectionCameraPrefab.transform.GetChild(0).position, movementSettings.projectionCameraPrefab.transform.up, turnAmount);
            }
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            Turn();
        }
    }
}