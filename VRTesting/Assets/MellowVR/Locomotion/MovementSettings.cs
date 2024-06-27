using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using VelUtils;

namespace MellowVR
{
    public enum Pivot
    {
    	Head,
        LeftController,
        RightController,
        Hip
    }

    /// <summary>
    /// Contains the settings for different locomotion schemes
    /// </summary>
    public class MovementSettings : MonoBehaviour
    {
        public MellowRig rig;

        public MoveDriver moveDriver;
        public TurnDriver turnDriver;
        public TeleportDriver teleportDriver;
        public SpaceManipulationDriver spaceManupulationDriver;
        public JumpDriver jumpDriver;



        [Header("Movement Types")]
        public bool continuousMovement = true; // uses moveInput / stick
        public bool teleportMovement = false; // uses teleportInput / button or stick up
        //public bool spaceDrag = false; // uses spaceDragInput / button


        
        [Header("Continuous Movement Settings")]
        public Pivot movePivot = Pivot.Head; // When moving continuously or with projection movement, which direction will be treated as forward?
        [HideInInspector]
        public Transform movePivotTransform;
        //public bool useForce = false; // false = velocity based : true = force based
        public float movementSpeed = 3f;
        public bool canRun = true;
        public float runMultiplier = 5/3;
        public bool projectionMovement = false;
        public GameObject projectionCameraPrefab;
        [HideInInspector]
        public Camera playerCamera;



        [Header("Teleport Settings")]
        // teleport pivot will come out of the respective hand
        public float chargeTime = .2f;
        public float teleportVelocity = 5f;
        public float maxSlope = 45;
        public float laserOffsetX = 0;
        public float laserOffsetY = -1;
        public float laserOffsetZ = 2;
        
        public LayerMask blocksTeleportLine;
        public GameObject teleportLinePrefab;
        [HideInInspector]
        public LineRenderer teleportLineRenderer;
        public GameObject teleportMarkerPrefab;
        [HideInInspector]
        public MeshRenderer teleportMarkerMeshRenderer;
        public Material teleportAccepted;
        public Material teleportDenied;
        //public bool hasBlink = false;
        //public float blinkLength = 0.5f;



        [Header("Rotation Types")]
        public bool continuousRotation = true; // uses turnInput
        //public bool teleportRotation = false; // uses teleportInput
        public bool snapRotation = false; // uses turnInput
        //public bool spaceRotation = false; // uses spaceTurnInput



        [Header("Rotation Settings")]
        public Pivot turnPivot = Pivot.Head; // when turning, around what part will we rotate around?
        [HideInInspector]
        public Transform turnPivotTransform;
        public float turningSpeed = 180f; 
        public float snapTurnAngle = 15f;
        public bool snapTurnAround; // if thumbstick far enough backwards, turn at 180 degree angle



        [Header("Misc Options")]
    	public LayerMask floorLayers = ~0; // for jumping and teleporting
        public bool canJump = true;
        public float jumpForce = 3f;
        public float jumpCooldown = 0.15f;
        public bool useFullBody = false;




    /* Add when needed 
        public enum ButtonInput
        {
            None,
            Trigger,
            Grip,
            Button1,
            Button2,
            Thumbstick,
            Thumbstick_Press

        }
        [Header("Which Button Does This Use?")]
        public ButtonInput teleportButton = ButtonInput.Button2;
        public ButtonInput spaceDragButton = ButtonInput.Trigger;
        public ButtonInput spaceTurnButton = ButtonInput.Button2;
        //public ButtonInput moveButton = ButtonInput.Thumbstick;
        //public ButtonInput turnButton = ButtonInput.Thumbstick;
    */

    /* Add when needed
        [Header("What Axes Can We Rotate On?")]
        public bool yaw = true;
        public bool pitch = false;
        public bool roll = false;
    */

        /// <summary>
        /// Sets the turn pivot for controls to the selected enum. Should be called whenever the enum is changed
        /// </summary>
        void SetTurnPivot()
        {
            switch (turnPivot) 
            {
                case Pivot.Hip:
                    turnPivotTransform = rig.hip;
                    break;
                case Pivot.LeftController:
                    turnPivotTransform = rig.leftHand;
                    break;
                case Pivot.RightController:
                    turnPivotTransform = rig.rightHand;
                    break;
                default: // Pivot.Head
                    turnPivotTransform = rig.head;
                    break;
            }
        }

        /// <summary>
        /// Sets the move pivot for controls to the selected enum. Should be called whenever the enum is changed
        /// </summary>
        void SetMovePivot()
        {
            switch (movePivot) 
            {
                case Pivot.Hip:
                    movePivotTransform = rig.hip;
                    break;
                case Pivot.LeftController:
                    movePivotTransform = rig.leftHand;
                    break;
                case Pivot.RightController:
                    movePivotTransform = rig.rightHand;
                    break;
                default: // Pivot.Head
                    movePivotTransform = rig.head;
                    break;
            }
        }



        /// <summary>
        /// Creates the linerenderer and teleport marker for teleportation
        /// </summary>
        void InitializeTeleport()
        {
            teleportLinePrefab = Instantiate(teleportLinePrefab, Vector3.zero, Quaternion.identity, rig.transform);
            teleportLineRenderer = teleportLinePrefab.GetComponent<LineRenderer>();
            teleportLinePrefab.SetActive(false);

            teleportMarkerPrefab = Instantiate(teleportMarkerPrefab, Vector3.zero, Quaternion.identity, rig.transform);
            teleportMarkerMeshRenderer = teleportMarkerPrefab.GetComponentInChildren<MeshRenderer>();
            teleportMarkerPrefab.SetActive(false);

        }



        /// <summary>
        /// Creates a second camera for projection movement
        /// </summary>
        void InitializeProjectionCamera()
        {
            projectionCameraPrefab = Instantiate(projectionCameraPrefab, Vector3.zero, Quaternion.identity);
            projectionCameraPrefab.SetActive(false);
            playerCamera = rig.head.gameObject.GetComponent<Camera>();

        }



        /// <summary>
        /// Checks whether full body is set or not. Should be called whenever full body is toggled
        /// </summary>
        void SetFullBody()
        {
            rig.hip.parent.gameObject.SetActive(useFullBody);
            rig.leftFoot.parent.gameObject.SetActive(useFullBody);
            rig.rightFoot.parent.gameObject.SetActive(useFullBody);

        }



        /// <summary>
        /// Initializes the rig by calling all functions that create prefabs or depend on settings
        /// </summary>
        void InitializeRig()
        {
            SetMovePivot();
            SetTurnPivot();
            SetFullBody();
            InitializeTeleport();
            InitializeProjectionCamera();

        }

        // Start is called before the first frame update
        void Start()
        {
            InitializeRig();

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //grounded = Physics.Raycast(rig.playerCollider.position, Vector3.down, rig.playerCollider.gameObject.GetComponent<CapsuleCollider>().height * 0.5f + 0.1f, floorLayers);
            //Turn();
            //Move();
            //Jump();

        }
    }
}