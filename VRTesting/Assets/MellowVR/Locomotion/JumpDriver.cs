using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MellowVR
{
    /// <summary>
    /// Allows the player to jump
    /// </summary>
    public class JumpDriver : LocomotionBase
    {
        private InputAction jump;
        private bool readyToJump;

        private void OnEnable()
        {
            jump = playerControls.Move.Jump;

            jump.Enable();
        }

        private void OnDisable()
        {
            jump.Disable();
        }

        /// <summary>
        /// Makes the player jump when pressing a button
        /// </summary>
        private void Jump()
        {
            if (jump.ReadValue<float>() > .1f && readyToJump && Grounded())
            {
                readyToJump = false;

                movementSettings.rig.rb.velocity = new Vector3(movementSettings.rig.rb.velocity.x, 0f, movementSettings.rig.rb.velocity.z); // reset y velocity
                movementSettings.rig.rb.AddForce(transform.up * movementSettings.jumpForce, ForceMode.Impulse); // jump

                Invoke(nameof(ResetJump), movementSettings.jumpCooldown);

            }

        }

        /// <summary>
        /// Checks if the player collider is on the ground
        /// </summary>
        /// <returns></returns>
        private bool Grounded()
        {
            return Physics.Raycast(movementSettings.rig.playerCollider.position, Vector3.down, movementSettings.rig.playerCollider.gameObject.GetComponent<CapsuleCollider>().height * 0.5f + 0.05f, movementSettings.floorLayers);
        }

        /// <summary>
        /// Marks jump as ready to be called after the cooldown
        /// </summary>
        private void ResetJump()
        {
            readyToJump = true;

        }

        // Start is called before the first frame update
        void Start()
        {
            readyToJump = true;

        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            switch (movementSettings.canJump)
            {
                case true:
                    Jump();
                    break;
                default:
                    break;
            }

        }
    }
}