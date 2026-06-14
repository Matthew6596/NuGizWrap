using System;
using UnityEngine;
using UnityEngine.Events;

namespace TTModdingKit.Emulation
{
    public class Character : MonoBehaviour
    {
        public bool isPlayable = false;
        public Stats stats;
        public AI ai;

        [NonSerialized]
        public bool isPlayer = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (isPlayer) ProcessPlayerInput();
            else ProcessAI();
        }

        public void ProcessAI()
        {

        }

        public void ProcessPlayerInput()
        {
            if(TTLevelTester.IsKeyPressed(TTInputAction.Swap) && TrySwitchCharacter(out var newChar))
            {
                TTLevelTester.SelectCharacter(newChar);
                return;
            }
        }

        private bool TrySwitchCharacter(out Character newCharacter)
        {
            newCharacter = null;
            return false;
        }

        [Serializable]
        public struct Stats
        {

        }

        [Serializable]
        public struct AI
        {

        }
    }
}