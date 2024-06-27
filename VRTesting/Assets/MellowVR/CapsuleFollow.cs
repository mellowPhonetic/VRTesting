using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MellowVR
{
    public class CapsuleFollow : MonoBehaviour
    {
        public MellowRig rig;
        public float offset = 0.75f;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, rig.playerCollider.position.y - offset, gameObject.transform.position.z);
        }
    }
}