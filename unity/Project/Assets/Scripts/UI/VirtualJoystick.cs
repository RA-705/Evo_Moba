using UnityEngine;

namespace Evo.Client.UI
{
    public class VirtualJoystick : MonoBehaviour
    {
        public float JoystickSize = 160f;
        public float DeadZone = 0.15f;

        public Vector2 Direction { get; private set; }
        public bool IsPressed { get; private set; }

        public static VirtualJoystick Create()
        {
            var go = new GameObject("VirtualJoystick");
            DontDestroyOnLoad(go);
            return go.AddComponent<VirtualJoystick>();
        }

        private void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector2 input = new Vector2(h, v);

            IsPressed = input.magnitude > DeadZone;
            Direction = IsPressed ? input.normalized : Vector2.zero;
        }
    }
}
