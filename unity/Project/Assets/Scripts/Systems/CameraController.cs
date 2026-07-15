using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;
    
    private float _currentZoom = 15f;
    private Vector3 _cameraOffset;
    
    private void Start()
    {
        _cameraOffset = transform.position;
    }
    
    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }
    
    private void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;
        
        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
    }
    
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _currentZoom -= scroll * zoomSpeed;
            _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
            
            Vector3 pos = transform.position;
            pos.y = _currentZoom;
            transform.position = pos;
        }
    }
}
