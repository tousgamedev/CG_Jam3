using Characters.Player;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerController controller;

        private void Awake()
        {
            if(!TryGetComponent(out controller))
            {
                Debug.LogError("PlayerController component could not be found");
            }
        }

        public void OnMoveInput(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            if (SettingsManager.Instance.InvertYAxis)
            {
                input.y *= -1;
            }
            
            controller.MovementMode.SetMovementInput(input);
        }

        public void OnBoostInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                controller.MovementMode.StartBoosting();
            }
        }
    }
}
