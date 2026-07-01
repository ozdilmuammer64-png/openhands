using UnityEngine;

namespace KnightOnline
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;
        public bool followTarget = true;
        
        [Header("Position")]
        public Vector3 offset = new Vector3(0, 5, -10);
        public float distanceFromTarget = 10f;
        public float heightAboveTarget = 5f;
        
        [Header("Rotation")]
        public float rotationSpeed = 5f;
        public float minVerticalAngle = -30f;
        public float maxVerticalAngle = 60f;
        public bool invertY = false;
        
        [Header("Zoom")]
        public float minZoom = 3f;
        public float maxZoom = 15f;
        public float zoomSpeed = 5f;
        public float zoomSmoothness = 10f;
        
        [Header("Shake")]
        public float shakeIntensity = 0.5f;
        public float shakeDuration = 0.3f;
        
        [Header("Combat Camera")]
        public bool enableCombatZoom = true;
        public float combatZoomFOV = 50f;
        public float normalFOV = 60f;
        
        // Private
        private float currentZoom;
        private float currentRotationX;
        private float currentRotationY;
        private Vector3 originalOffset;
        private bool isShaking;
        private float shakeTimer;
        private Vector3 shakeOffset;
        private Camera cam;
        
        void Start()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                cam = gameObject.AddComponent<Camera>();
            }
            
            if (target == null)
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    target = player.transform;
                }
            }
            
            currentZoom = distanceFromTarget;
            originalOffset = offset;
            currentRotationY = 0;
            currentRotationX = 30;
            
            if (cam != null)
            {
                normalFOV = cam.fieldOfView;
            }
        }
        
        void LateUpdate()
        {
            if (target == null || !followTarget) return;
            
            HandleRotation();
            HandleZoom();
            UpdatePosition();
            HandleShake();
            HandleCombatCamera();
        }
        
        void HandleRotation()
        {
            // Mouse rotation
            float mouseX = Input.GetAxisRaw("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxisRaw("Mouse Y") * rotationSpeed;
            
            if (invertY)
            {
                mouseY = -mouseY;
            }
            
            currentRotationY += mouseX;
            currentRotationX -= mouseY;
            currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);
            
            // Update offset based on rotation
            Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
            offset = rotation * originalOffset;
        }
        
        void HandleZoom()
        {
            float scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                currentZoom -= scrollInput * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            }
        }
        
        void UpdatePosition()
        {
            // Calculate target position
            Vector3 targetPosition = target.position + offset.normalized * currentZoom;
            
            // Smooth follow
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            
            // Look at target
            Vector3 lookTarget = target.position + Vector3.up * 1f;
            transform.LookAt(lookTarget);
        }
        
        void HandleShake()
        {
            if (isShaking)
            {
                shakeTimer -= Time.deltaTime;
                
                if (shakeTimer <= 0)
                {
                    isShaking = false;
                    shakeOffset = Vector3.zero;
                }
                else
                {
                    float x = Random.Range(-shakeIntensity, shakeIntensity);
                    float y = Random.Range(-shakeIntensity, shakeIntensity);
                    float z = Random.Range(-shakeIntensity, shakeIntensity);
                    shakeOffset = new Vector3(x, y, z);
                }
            }
            
            transform.position += shakeOffset;
        }
        
        void HandleCombatCamera()
        {
            if (!enableCombatZoom) return;
            
            PlayerController player = target?.GetComponent<PlayerController>();
            bool inCombat = player != null && player.CurrentState == PlayerState.Attacking;
            
            float targetFOV = inCombat ? combatZoomFOV : normalFOV;
            
            if (cam != null)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 5f);
            }
        }
        
        public void ShakeCamera(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
            isShaking = true;
            shakeTimer = duration;
        }
        
        public void ShakeOnHit()
        {
            ShakeCamera(0.2f, 0.1f);
        }
        
        public void ShakeOnDeath()
        {
            ShakeCamera(0.5f, 0.5f);
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        public void SetFollowEnabled(bool enabled)
        {
            followTarget = enabled;
        }
        
        public void ResetCamera()
        {
            currentRotationY = 0;
            currentRotationX = 30;
            currentZoom = distanceFromTarget;
            transform.rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        }
        
        void OnDrawGizmosSelected()
        {
            if (target != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(target.position, distanceFromTarget);
                
                Gizmos.color = Color.green;
                Gizmos.DrawLine(target.position, target.position + offset);
            }
        }
    }
}
