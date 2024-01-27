using System;
using UnityEngine;

namespace Matchbox
{
    public class CollisionEventReceiver : MonoBehaviour
    {

        public Action<Collision> collisionEnterAction;
        public Action<Collision> collisionExitAction;
        public Action<Collision> collisionStayAction;
        public Action<Collider> triggerEnterAction;
        public Action<Collider> triggerExitAction;
        public Action<Collider> triggerStayAction;

        public void OnCollisionEnter(Collision collision)
        {
            if (collisionEnterAction != null)
            {
                collisionEnterAction(collision);
            }
        }

        public void OnCollisionExit(Collision collision)
        {
            if (collisionExitAction != null)
            {
                collisionExitAction(collision);
            }
        }

        public void OnCollisionStay(Collision collision)
        {
            if (collisionStayAction != null)
            {
                collisionStayAction(collision);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (triggerEnterAction != null)
            {
                triggerEnterAction(other);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (triggerExitAction != null)
            {
                triggerExitAction(other);
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (triggerStayAction != null)
            {
                triggerStayAction(other);
            }
        }
    }

}