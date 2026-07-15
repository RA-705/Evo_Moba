using UnityEngine;
using Evo.Client.UI;

namespace Evo.Client
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public static PlayerInputHandler Instance { get; private set; }

        private VirtualJoystick _joystick;
        private HeroEntity _localHero;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _joystick = VirtualJoystick.Create();
        }

        private void Update()
        {
            if (_localHero == null || !_localHero.IsAlive)
            {
                _localHero = FindLocalHero();
                if (_localHero == null) return;
            }

            Vector3 moveDir = GetMoveDirection();

            _localHero.SetMoveDirection(moveDir);

            if (Evo.Core.GameManager.Instance != null)
                Evo.Core.GameManager.Instance.SendMoveInput(moveDir);
        }

        private Vector3 GetMoveDirection()
        {
            Vector2 joystickDir = _joystick != null ? _joystick.Direction : Vector2.zero;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 keyboardDir = new Vector2(h, v);

            Vector2 combined = joystickDir.magnitude > 0.1f ? joystickDir : keyboardDir;
            if (combined.magnitude < 0.1f)
                return Vector3.zero;

            return new Vector3(combined.x, 0, combined.y).normalized;
        }

        private HeroEntity FindLocalHero()
        {
            var hero = FindObjectOfType<HeroEntity>();
            return hero;
        }
    }
}
