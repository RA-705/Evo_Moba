using UnityEngine;

namespace Evo.Client
{
    public class GameCamera : MonoBehaviour
    {
        public static GameCamera Instance { get; private set; }

        [Header("Camera Settings")]
        public float FollowHeight = 15f;
        public float FollowDistance = 20f;
        public float MinZoom = 10f;
        public float MaxZoom = 30f;
        public float ZoomSpeed = 5f;
        public float PanSpeed = 20f;
        public float RotationSpeed = 2f;
        public float SmoothTime = 0.1f;

        [Header("Bounds")]
        public float MapMinX = -50f;
        public float MapMaxX = 50f;
        public float MapMinZ = -50f;
        public float MapMaxZ = 50f;

        private Transform _target;
        private Vector3 _velocity;
        private float _currentZoom;
        private float _targetRotation = 45f;
        private bool _isPanning = false;
        private Vector3 _lastMousePosition;

        private void Awake()
        {
            Instance = this;
            _currentZoom = FollowDistance;
            
            // Set initial camera angle
            transform.rotation = Quaternion.Euler(45f, 45f, 0f);
            transform.position = new Vector3(0, FollowHeight, -FollowDistance);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            HandleInput();
            
            if (_target != null)
            {
                FollowTarget();
            }
            
            ApplyZoom();
            ClampToBounds();
        }

        private void HandleInput()
        {
            // Zoom with scroll wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                _currentZoom = Mathf.Clamp(_currentZoom - scroll * ZoomSpeed * 10f, MinZoom, MaxZoom);
            }

            // Pan with middle mouse or WASD
            if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
            {
                _isPanning = true;
                _lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(1))
            {
                _isPanning = false;
            }

            if (_isPanning)
            {
                Vector3 delta = Input.mousePosition - _lastMousePosition;
                Vector3 pan = new Vector3(-delta.x, 0, -delta.y) * PanSpeed * Time.deltaTime * (_currentZoom / 20f);
                transform.Translate(pan, Space.World);
                _lastMousePosition = Input.mousePosition;
            }

            // Keyboard pan
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            {
                Vector3 pan = new Vector3(horizontal, 0, vertical) * PanSpeed * Time.deltaTime;
                transform.Translate(pan, Space.World);
            }

            // Rotate with Q/E
            if (Input.GetKey(KeyCode.Q))
                _targetRotation -= RotationSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.E))
                _targetRotation += RotationSpeed * Time.deltaTime;
        }

        private void FollowTarget()
        {
            Vector3 targetPosition = _target.position;
            targetPosition.y = 0;

            Vector3 desiredPosition = targetPosition - transform.forward * _currentZoom;
            desiredPosition.y = FollowHeight;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, SmoothTime);

            // Smooth rotation
            Quaternion targetRot = Quaternion.Euler(45f, _targetRotation, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        private void ApplyZoom()
        {
            // Zoom is handled by changing FollowDistance in FollowTarget
        }

        private void ClampToBounds()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, MapMinX + _currentZoom, MapMaxX - _currentZoom);
            pos.z = Mathf.Clamp(pos.z, MapMinZ + _currentZoom, MapMaxZ - _currentZoom);
            transform.position = pos;
        }

        public void FocusOnPosition(Vector3 position, float zoom = -1f)
        {
            if (zoom > 0) _currentZoom = Mathf.Clamp(zoom, MinZoom, MaxZoom);
            
            Vector3 targetPos = position;
            targetPos.y = 0;
            
            Vector3 desiredPosition = targetPos - transform.forward * _currentZoom;
            desiredPosition.y = FollowHeight;
            
            transform.position = desiredPosition;
        }

        public void SetMapBounds(float minX, float maxX, float minZ, float maxZ)
        {
            MapMinX = minX;
            MapMaxX = maxX;
            MapMinZ = minZ;
            MapMaxZ = maxZ;
        }
    }
}