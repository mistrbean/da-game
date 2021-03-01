using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{

    public class playerInput : MonoBehaviour, IInput
    {
        public Action<Vector2> OnMovementInput { get; set; }

        public Action<Vector3> OnMovementDirectionInput { get; set; }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            GetMovementInput();
            GetMovementDirection();
        }

        private void GetMovementDirection()
        {
            var cameraForwardDirection = Camera.main.transform.forward;
            Debug.DrawRay(Camera.main.transform.position, cameraForwardDirection * 10, Color.red);
            var directionToMoveIn = Vector3.Scale(cameraForwardDirection, (Vector3.right + Vector3.forward));
            Debug.DrawRay(Camera.main.transform.position, directionToMoveIn * 10, Color.blue);
            OnMovementDirectionInput?.Invoke(directionToMoveIn);
        }

        private void GetMovementInput()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            OnMovementInput?.Invoke(input);
        }
    }
}