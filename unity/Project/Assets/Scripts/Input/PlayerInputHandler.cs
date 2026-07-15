using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private int _heroEntityId;
    private CameraController _camera;
    
    private void Start()
    {
        _camera = GetComponent<CameraController>();
    }
    
    private void Update()
    {
        HandleMovementInput();
        HandleAbilityInput();
        HandleShopInput();
    }
    
    private void HandleMovementInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                SendMoveCommand(hit.point);
            }
        }
    }
    
    private void HandleAbilityInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) SendAbilityCommand(0);
        if (Input.GetKeyDown(KeyCode.W)) SendAbilityCommand(1);
        if (Input.GetKeyDown(KeyCode.E)) SendAbilityCommand(2);
        if (Input.GetKeyDown(KeyCode.R)) SendAbilityCommand(3);
    }
    
    private void HandleShopInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShopUI.Instance?.ToggleShop();
        }
    }
    
    private void SendMoveCommand(Vector3 targetPosition)
    {
        // InputType 0 = Move
        GameClient.Instance.SendInput(0, _heroEntityId, System.BitConverter.GetBytes(targetPosition.x)
            .Concat(System.BitConverter.GetBytes(targetPosition.y))
            .Concat(System.BitConverter.GetBytes(targetPosition.z))
            .ToArray());
    }
    
    private void SendAbilityCommand(int abilitySlot)
    {
        // InputType 1 = Ability
        GameClient.Instance.SendInput(1, _heroEntityId, new byte[] { (byte)abilitySlot });
    }
}
