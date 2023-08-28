using System;
using UnityEngine;

namespace Player.Runtime
{
    public class RotationTest : MonoBehaviour
    {
        [Range(0f, 360f)]
        [SerializeField]
        private float maxRotation = 360f;

        [Range(0f, 360f)]
        [SerializeField]
        private float minRotation = 0f;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            LookAtCursor();
            LimitRotation();
        }

        private void LookAtCursor()
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = new Vector2(
                mousePosition.x - transform.position.x,
                mousePosition.y - transform.position.y);

            transform.up = direction;
        }

        private void LimitRotation()
        {
            float newRotation = RotationUtility.LimitRotation(
                transform.eulerAngles.z,
                minRotation,
                maxRotation);

            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                newRotation);
        }
    }

    public static class RotationUtility
    {
        /// <param name="rotation">
        ///  Value of 0f to 360f.
        ///  Rotation that will be limitted to a certain angle.
        /// </param>
        public static float LimitRotation(
            float rotation,
            float minRotation,
            float maxRotation)
        {
            if (minRotation > maxRotation)
            {
                throw new ArgumentException("Min rotation cannot exceed the max!");
            }

            if (rotation < 0f ||
                rotation > 360f)
            {
                throw new ArgumentException("Rotation has to be a value in range from 0f to 360f!");
            }

            if (rotation > maxRotation ||
                rotation < minRotation)
            {
                float rotationDistanceFromMaxValue = Mathf.Abs(Mathf.DeltaAngle(rotation, maxRotation));
                float rotationDistanceFromMinValue = Mathf.Abs(Mathf.DeltaAngle(rotation, minRotation));

                if (rotationDistanceFromMaxValue < rotationDistanceFromMinValue)
                {
                    return maxRotation;
                }
                return minRotation;
            }

            return rotation;
        }
    }
}
