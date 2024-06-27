using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MellowVR
{
    public class GrabbableHandle : GrabbableObject
    {
        public Transform handle;

        public override void Grabbed(GrabDriver driver, Transform location)
        {
            rb.constraints = RigidbodyConstraints.None;

            base.Grabbed(driver, location);
        }
        
        public override void Released()
        {
            base.Released();

            rb.constraints = RigidbodyConstraints.FreezeAll;

            transform.position = handle.position;
            transform.rotation = handle.rotation;

            Rigidbody rbHandle = handle.GetComponent<Rigidbody>();
            rbHandle.velocity = Vector3.zero;
            rbHandle.angularVelocity = Vector3.zero;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (grabbedBy != null && Vector3.Distance(transform.position, handle.position) > 0.75f)
            {
                Released();
            }
        }
    }
}