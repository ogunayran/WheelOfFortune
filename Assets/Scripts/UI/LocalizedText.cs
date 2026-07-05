using TMPro;
using UnityEngine;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>Keeps a TMP text in sync with the string table for the active language.</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;

        private TMP_Text text;

        private void Awake() => text = GetComponent<TMP_Text>();

        private void OnEnable()
        {
            Loc.Changed += Apply;
            Apply();
        }

        private void OnDisable() => Loc.Changed -= Apply;

        private void Apply()
        {
            if (!string.IsNullOrEmpty(key))
                text.text = Loc.Get(key);
        }
    }
}
