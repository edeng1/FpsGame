// Designed by Kinemation, 2023

using UnityEngine;

namespace Demo.Scripts.Runtime.Base
{
    public class AnimEventReceiver : MonoBehaviour
    {
        [SerializeField] private FPSController controller;

        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<FPSController>();
            }
        }
        
        public void SetActionActive(int isActive)
        {
            controller.SetActionActive(isActive);
        }
    }
}
