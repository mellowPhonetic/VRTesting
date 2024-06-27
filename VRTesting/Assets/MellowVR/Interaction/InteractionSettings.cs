using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MellowVR
{
    public enum Side
    {
    	Left,
        Right,
        Both
    }
    public class InteractionSettings : MonoBehaviour
    {
        public MellowRig rig;

        public GrabDriver grabDriver;
        public HapticsDriver hapticsDriver;

        [Header("Grab Settings")]
        public bool canGrab = true;
        public Side grabbableHands = Side.Both;
        public bool vibrateOnGrab = true;
		public LayerMask layerMask = ~0;

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