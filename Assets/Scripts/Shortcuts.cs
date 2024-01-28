using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Matchbox
{
    public class Shortcuts : MonoBehaviour
    {
        public UnityEvent Pause;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause?.Invoke();
            }
        }
    }
}