using System;
using UnityEngine;

namespace NuGizWrap.Emulation
{
    public enum TTInputAction { Up, Left, Right, Down, Jump, Special, Attack, Swap, Pause }

    [DefaultExecutionOrder(-1)]
    public class TTLevelTester : MonoBehaviour
    {
        public static TTLevelTester Instance { get; private set; }

        public Character player;
        public InputScheme inputs;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SelectCharacter(player);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SelectNewCharacter(Character newPlayer)
        {
            if (player != null) player.isPlayer = false;
            player = newPlayer;
            player.isPlayer = true;
        }

        public static void SelectCharacter(Character newPlayer) => Instance.SelectNewCharacter(newPlayer);

        private static KeyCode GetKeyCode(TTInputAction input) => Instance.inputs.GetKeyCode(input);
        public static bool IsKeyPressed(TTInputAction input) => Input.GetKeyDown(GetKeyCode(input));
        public static bool IsKeyDown(TTInputAction input) => Input.GetKey(GetKeyCode(input));
        public static bool IsKeyReleased(TTInputAction input) => Input.GetKeyUp(GetKeyCode(input));
        public static bool IsKeyUp(TTInputAction input) => !IsKeyDown(input);
        public static Vector2 GetMoveInput()
        {
            Vector2 movement = Vector2.zero;
            if (IsKeyDown(TTInputAction.Up)) movement.y++;
            if (IsKeyDown(TTInputAction.Down)) movement.y--;
            if (IsKeyDown(TTInputAction.Right)) movement.x++;
            if (IsKeyDown(TTInputAction.Left)) movement.x--;
            return movement.normalized;
        }

        [Serializable]
        public struct InputScheme
        {
            public KeyCode up, left, right, down, jump, special, attack, swap, pause;
            public readonly KeyCode GetKeyCode(TTInputAction input) => (input) switch
            {
                TTInputAction.Up => up,
                TTInputAction.Left => left,
                TTInputAction.Right => right,
                TTInputAction.Down => down,
                TTInputAction.Jump => jump,
                TTInputAction.Special => special,
                TTInputAction.Attack => attack,
                TTInputAction.Swap => swap,
                TTInputAction.Pause => pause,
                _ => KeyCode.None
            };
        }
    }
}