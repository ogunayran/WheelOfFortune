using UnityEngine;

namespace WheelGame.UI
{
    /// <summary>
    /// Base class for all views. References are bound automatically in OnValidate,
    /// so nothing is dragged by hand and a rename shows up as a console warning
    /// instead of a silently broken reference.
    /// </summary>
    public abstract class AutoBoundView : MonoBehaviour
    {
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying) return;
            BindReferences();
        }
#endif

        /// <summary>Assign all serialized references here using FindDeep, e.g. spinButton = FindDeep&lt;Button&gt;("ui_button_spin").</summary>
        protected abstract void BindReferences();

        /// <summary>Finds a component on a child (any depth, inactive included) by exact hierarchy name.</summary>
        protected T FindDeep<T>(string childName) where T : Component
        {
            foreach (var t in GetComponentsInChildren<Transform>(true))
            {
                if (t.name == childName && t.TryGetComponent(out T component))
                    return component;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Debug.LogWarning($"[AutoBoundView] '{name}' could not bind '{childName}' ({typeof(T).Name}).", this);
#endif
            return null;
        }
    }
}
