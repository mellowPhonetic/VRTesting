using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MellowVR
{
    public class TeleportDriver : LocomotionBase
    {
        private InputAction teleportLeft;
        private InputAction teleportRight;

        private bool teleporting = false;
        private float teleportCharge = 0;
        private Transform teleportOrigin;
        private bool teleportableSurface = false;

        private void OnEnable()
        {
            teleportLeft = playerControls.Move.TeleportLeft;
            teleportRight = playerControls.Move.TeleportRight;

            teleportLeft.Enable();
            teleportRight.Enable();

        }

        private void OnDisable()
        {
            teleportLeft.Disable();
            teleportRight.Disable();

        }


        /// <summary>
        /// Teleports the player
        /// </summary>
        private void Teleport()
        {
            // charge teleport 
            if (!teleporting)
            {
                ChargeTeleport();
            } 

            // draw line and shoot raycasts to determine teleport position
            if (teleporting)
            {
                //Debug.Log("Teleporting");
                CheckHand();
                DrawTeleport();

            }

            // reset the charge time if the buttons are not held
            if (teleportCharge != 0 && teleportLeft.ReadValue<float>() < .1f && teleportRight.ReadValue<float>() < .1f)
            {
                //Debug.Log("Reset Charge");
                teleportCharge = 0;

            }

            // no teleport while walking
            if (movementSettings.continuousMovement && (movementSettings.moveDriver.moveContinuous.ReadValue<Vector2>().x > 0.1f || movementSettings.moveDriver.moveContinuous.ReadValue<Vector2>().x < -0.1f || movementSettings.moveDriver.moveContinuous.ReadValue<Vector2>().y > 0.1f || movementSettings.moveDriver.moveContinuous.ReadValue<Vector2>().y < -0.1f))
            {
                teleportCharge = 0;
                teleporting = false;
                movementSettings.teleportLinePrefab.SetActive(false);
                movementSettings.teleportMarkerPrefab.SetActive(false);
            }

            // teleport as the teleport button is let go
            if (teleporting && teleportCharge == 0)
            {
                //Debug.Log("Initiate Teleport");
                // teleport to the point
                Teleport(movementSettings.teleportMarkerPrefab.transform.position);
            }

        }


        /// <summary>
        /// Teleports the player
        /// </summary>
        private void Teleport(Vector3 position)
        {
            if (teleportableSurface)
            {
                position.y = position.y + movementSettings.rig.playerCollider.gameObject.GetComponent<CapsuleCollider>().height * 0.5f + 0.01f;

                movementSettings.rig.playerCollider.position = position;
                movementSettings.rig.playerTracking.position = position - movementSettings.rig.head.position + movementSettings.rig.playerTracking.position;
            }
            teleporting = false;
            movementSettings.teleportLinePrefab.SetActive(false);
            movementSettings.teleportMarkerPrefab.SetActive(false);


        }



        /// <summary>
        /// Charges the player's teleport action
        /// </summary>
        private void ChargeTeleport()
        {
            if (teleportLeft.ReadValue<float>() > .1f || teleportRight.ReadValue<float>() > .1f)
            {
                //Debug.Log("Charging (" + teleportCharge + " + " + Time.fixedDeltaTime + ") / " + movementSettings.chargeTime);
                teleportCharge = teleportCharge + Time.fixedDeltaTime;
            } 

            if (teleportCharge >= movementSettings.chargeTime)
            {
                //Debug.Log("Charged");
                teleporting = true;
                movementSettings.teleportLinePrefab.SetActive(true);
            }

        }


        /// <summary>
        /// Draws a line as an indicator to where the player will teleport
        /// </summary>
        private void DrawTeleport()
        {
            //Debug.Log("Drawing Line");
            List<Vector3> path = new List<Vector3>();

            Vector3 position = teleportOrigin.position;
            path.Add(position);
            Vector3 lastPosition;

            Vector3 vectorVelocity = teleportOrigin.TransformDirection(movementSettings.laserOffsetX, movementSettings.laserOffsetY, movementSettings.laserOffsetZ).normalized * movementSettings.teleportVelocity;

            float slopeAngle;

            float duration = 3;
            float timestep = Time.fixedDeltaTime;

            RaycastHit hit;
            for (float t = 0f; t < duration; t += timestep)
            {
                //Debug.Log("Drawing Line");
                vectorVelocity += Physics.gravity * timestep;
                lastPosition = position;
                position += vectorVelocity * timestep;
                path.Add(position);
                if (Physics.Raycast(lastPosition, position - lastPosition, out hit, (position - lastPosition).magnitude, movementSettings.blocksTeleportLine))
                {   
                    slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                    //Debug.Log("Angle" + slopeAngle);
                    
                    UpdateLineMaterial((movementSettings.floorLayers & (1 << hit.transform.gameObject.layer)) != 0 && slopeAngle <= movementSettings.maxSlope);
                    movementSettings.teleportMarkerPrefab.SetActive(true);
                    
                    movementSettings.teleportMarkerPrefab.transform.position = hit.point;
                    movementSettings.teleportMarkerPrefab.transform.rotation = Quaternion.LookRotation(hit.normal);
                    break;
                }
                else
                {
                    UpdateLineMaterial(false);
                    movementSettings.teleportMarkerPrefab.SetActive(false);
                }
            }

            movementSettings.teleportLineRenderer.positionCount = path.Count;
            movementSettings.teleportLineRenderer.SetPositions(path.ToArray());
        }



        /// <summary>
        /// Change the material of the teleport line to this
        /// </summary>
        /// <param name="teleportable"></param>
        private void UpdateLineMaterial(bool teleportable)
        {
            teleportableSurface = teleportable;

            Material lineMaterial;

            if (teleportableSurface)
            {
                lineMaterial = movementSettings.teleportAccepted;
            }
            else
            {
                lineMaterial = movementSettings.teleportDenied;
            }

            movementSettings.teleportLineRenderer.material = lineMaterial;
            movementSettings.teleportMarkerMeshRenderer.material = lineMaterial;

        }
        


        /// <summary>
        /// Checks which side the teleport button is being pressed, if both, defaults to the right
        /// </summary>
        private void CheckHand()
        {
            if (teleportRight.ReadValue<float>() > .1f)
            {
                //Debug.Log("Right Hand");
                teleportOrigin = movementSettings.rig.rightHand;
            }
            else if (teleportLeft.ReadValue<float>() > .1f)
            {
                //Debug.Log("Left Hand");
                teleportOrigin = movementSettings.rig.leftHand;
            }

        }



        /// <summary>
        /// Gives the teleport line velocity after modifiers. By default there are none
        /// </summary>
        /// <returns></returns>
        private float getCurrentTeleportVelocity()
        {
            float velocity = movementSettings.teleportVelocity;
            
            /*
            switch (movementSettings.canRun && run.ReadValue<float>() > .1f)
            {
                case true:
                    speed = speed * movementSettings.runMultiplier;
                    break;
                default:
                    break;
            }
            */

            return velocity;

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
            switch (movementSettings.teleportMovement)
            {
                case true:
                    Teleport();
                    break;
                default:
                    break;
            }

        }
    }
}
