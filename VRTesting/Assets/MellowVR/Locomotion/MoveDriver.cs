using UnityEngine;
using UnityEngine.InputSystem;

namespace MellowVR
{
    /// <summary>
    /// This class lets the player move by sliding across the floor
    /// </summary>
    public class MoveDriver : LocomotionBase
    {
        [HideInInspector]
        public InputAction moveContinuous;
        private InputAction toggleProjection;
        private InputAction run;

        private bool readyToProject = true;
        [HideInInspector]
        public bool projecting = false;

        private void OnEnable()
        {
            moveContinuous = playerControls.Move.Continuous;
            toggleProjection = playerControls.Move.Projection;
            run = playerControls.Move.Run;

            moveContinuous.Enable();
            toggleProjection.Enable();
            run.Enable();
        }

        private void OnDisable()
        {
            moveContinuous.Disable();
            toggleProjection.Disable();
            run.Disable();

        }

        /// <summary>
        /// Moves the player continuously based on thumbstick movement and the selected forward origin
        /// </summary>
        private void Walk()
        {
            Vector3 moveDirection = ForwardTransform(moveContinuous.ReadValue<Vector2>()); // get the controller input and transform it based on the forward origin

            moveDirection.y = movementSettings.rig.rb.velocity.y; // conserve up/down velocity

            movementSettings.rig.rb.velocity = moveDirection; // apply velocity

        }

        /// <summary>
        /// Toggle Button that creates a temporary camera in the same place as the player's head. 
        /// This camera will be the active camera and will not move except through tracking until toggled again. 
        /// Player can Walk while this temporary camera is active.
        /// This creates an experience where the player can see their body move, but their camera does not follow. 
        /// In essence, they are projecting their body to some location and then teleporting to it's position. 
        /// Insipired by VRChat's Holoport and Convrge VR's Ghosting locomotion types
        /// </summary>
        private void Projection()
        {
            if (toggleProjection.ReadValue<float>() < .1f && !readyToProject)
            {
                readyToProject = true;
            }


            if (toggleProjection.ReadValue<float>() > .1f && readyToProject)
            {
                readyToProject = false;

                ToggleProject();

            }

            if (projecting)
            {
                Walk();

            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ToggleProject()
        {
            projecting = !projecting;

            if (projecting)
            {
                movementSettings.projectionCameraPrefab.SetActive(true);
                movementSettings.projectionCameraPrefab.transform.position = movementSettings.rig.playerTracking.position;
                movementSettings.projectionCameraPrefab.transform.rotation = movementSettings.rig.playerTracking.rotation;
                movementSettings.playerCamera.enabled = false;
            }
            else
            {
                movementSettings.playerCamera.enabled = true;
                movementSettings.projectionCameraPrefab.SetActive(false);

            }


        }

        /// <summary>
        /// Takes controller input and transforms it based on the direction of the chosen movePivot
        /// </summary>
        /// <param name="inputDirection"> Intended to be controller input where x will map to the output x and y will map to the output z</param>
        private Vector3 ForwardTransform(Vector2 inputDirection)
        {
            Vector3 moveDirection = movementSettings.movePivotTransform.TransformDirection(inputDirection.x, 0, inputDirection.y);

            // conserve magnitude
            float magnitude = moveDirection.magnitude * getCurrentMovementSpeed(); // magnitude of the direction multiplied by movement speed
            moveDirection.y = 0; // remove y interference from head angle
            moveDirection = moveDirection.normalized * magnitude; // apply magnitude (if this isn't done, some speed will be lost if the player's head isn't level with the ground)

            return moveDirection;
        }



        /// <summary>
        /// Gives the movement speed after modifiers. By default the only modifier is running
        /// </summary>
        /// <returns></returns>
        float getCurrentMovementSpeed()
        {
            float speed = movementSettings.movementSpeed;

            switch (movementSettings.canRun && run.ReadValue<float>() > .1f)
            {
                case true:
                    speed = speed * movementSettings.runMultiplier;
                    break;
                default:
                    break;
            }

            return speed;

        }


        /// <summary>
        /// Checks if projection is true for walking purposes
        /// </summary>
        void Move()
        {
            switch (movementSettings.projectionMovement)
            {
                case true:
                    Projection();
                    break;
                default:
                    Walk();
                    break;
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
            switch (movementSettings.continuousMovement)
            {
                case true:
                    Move();
                    break;
                default:
                    break;
            }
        }
    }
}