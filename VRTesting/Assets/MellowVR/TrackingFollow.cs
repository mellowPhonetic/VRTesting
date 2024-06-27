using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MellowVR
{
    public class TrackingFollow : MonoBehaviour
    {
        public MellowRig rig;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            gameObject.transform.position = new Vector3(rig.head.position.x, gameObject.transform.position.y, rig.head.position.z);

        }
    }
}