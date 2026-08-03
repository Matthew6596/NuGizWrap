#if UNITY_EDITOR
using System;
using UnityEngine;

namespace TTModdingKit.AI.Scripting
{
    public abstract class AIScript : MonoBehaviour
    {
        //public abstract State Base { get; }

        #region Actions
        #endregion

        #region Conditions
        #endregion

        public class State
        {
            private State() { }

            public State(ConditionResult[] conditions, ActionResult[] actions) { }

            public ActionResult[] actions;
        }

        public class ActionResult
        {

        }

        public class ConditionResult
        {
            public static implicit operator bool(ConditionResult c) => true;
        }
    }
}
#endif