using UnityEngine;

namespace KnightOnline
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0, 5, -10);
        public float smoothSpeed = 10f;
        public float rotationSpeed = 5f;
        
        private float currentX = 0f;
        private float currentY = 30f;
        public float minY = -30f;
        public float maxY = 60f;
        
        void LateUpdate()
        {
            if (target == null)
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    return;
                }
            }
            
            // Mouse rotation
            if (Input.GetMouseButton(1)) // Right click to rotate
            {
                currentX += Input.GetAxis("Mouse X") * rotationSpeed;
                currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
                currentY = Mathf.Clamp(currentY, minY, maxY);
            }
            
            // Calculate rotation
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            
            // Calculate position
            Vector3 position = rotation * offset;
            Vector3 finalPosition = target.position + position;
            
            // Smooth follow
            transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed * Time.deltaTime);
            
            // Look at target
            transform.LookAt(target.position + Vector3.up);
        }
    }
}
