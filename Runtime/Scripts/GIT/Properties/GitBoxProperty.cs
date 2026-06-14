#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace TTModdingKit.GizFlow
{
    public abstract class GitBoxProperty
    {
        public string name;

        public VisualElement RootVisualElement { get; private set; }

        public GitBoxProperty(string name)
        {
            this.name = name;
            RootVisualElement = new();
        }

        public abstract T GetValue<T>();
        public abstract void SetValue(object value);
    }
}
#endif