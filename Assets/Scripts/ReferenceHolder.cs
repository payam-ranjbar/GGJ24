using UnityEngine;

namespace Matchbox
{
    public class ReferenceHolder : MonoBehaviour
    {
        public GameObject[] References;

        public T GetReferenceComponent<T>() where T : Component
        {
            foreach (var reference in References)
            {
                var component = reference.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }
    }
}