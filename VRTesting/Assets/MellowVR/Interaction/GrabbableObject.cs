using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MellowVR
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableObject : MonoBehaviour
    {
        [HideInInspector]
        public GrabDriver grabbedBy;
        [HideInInspector]
        public Transform hand;
        public int priority = 0;
        protected Rigidbody rb;
        
        [HideInInspector]
		public Queue<Vector3> recentVelocities = new Queue<Vector3>();
        [HideInInspector]
        public Queue<Vector3> recentAngularVelocities = new Queue<Vector3>();
		protected int recentVelocitiesLength = 5;

        void UpdateVelocities()
        {
            Vector3 distance = hand.position - transform.position;
            rb.velocity = distance / Time.deltaTime;
            recentVelocities.Enqueue(rb.velocity);
            recentVelocities.Dequeue();

            Quaternion tempRotation = hand.rotation * Quaternion.Inverse(transform.rotation);
            float angle;
            Vector3 axis;
            tempRotation.ToAngleAxis(out angle, out axis);
            angle = angle * Mathf.Deg2Rad;
            rb.angularVelocity = axis * angle / Time.deltaTime;
            recentAngularVelocities.Enqueue(rb.angularVelocity);
            recentAngularVelocities.Dequeue();
        }


        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.maxAngularVelocity = Mathf.Infinity;

            for (int i = 0; i < recentVelocitiesLength; i++)
            {
                recentVelocities.Enqueue(Vector3.zero);
                recentAngularVelocities.Enqueue(Vector3.zero);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected virtual void FixedUpdate()
        {
            if (grabbedBy != null)
            {
                UpdateVelocities();
            }
        }

        public virtual void Grabbed(GrabDriver driver, Transform location)
        {
            Debug.Log("Grabbed");
            grabbedBy = driver;
            hand = location;
        }

        public virtual void Released()
        {
            Vector3[] velocities = recentVelocities.ToArray();
            Vector3[] angularVelocities = recentAngularVelocities.ToArray();
            Vector3 releaseVelocity = Vector3.zero;
            Vector3 releaseAngularVelocity = Vector3.zero;

            for (int i = 0; i < recentVelocitiesLength; i++)
            {
                releaseVelocity += velocities[i];
                releaseAngularVelocity += angularVelocities[i];
            }
            rb.velocity = releaseVelocity / recentVelocitiesLength;
            rb.angularVelocity = releaseAngularVelocity / recentVelocitiesLength;
            grabbedBy = null;
            hand = null;
        }
    }
}
