using System.Collections.Generic;
using UnityEngine;
using WheelGame.Core;

namespace WheelGame.Configs
{
    /// <summary>
    /// Everything that describes one wheel variant (bronze / silver / golden):
    /// its slices, its art and its zone type. Designers tweak these assets freely;
    /// the wheel builds itself from this data at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "WheelConfig_", menuName = "WheelGame/Wheel Config")]
    public class WheelConfig : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private ZoneType zoneType;
        [SerializeField] private string title;

        [Header("Art")]
        [SerializeField] private Sprite baseSprite;      // ui_spin_bronze/silver/golden_base
        [SerializeField] private Sprite indicatorSprite; // matching indicator
        [SerializeField] private Sprite bombSprite;      // ui_card_icon_death (bronze only)

        [Header("Slices (edit freely — wheel is built from this list)")]
        [SerializeField] private List<WheelSliceData> slices = new List<WheelSliceData>();

        public ZoneType ZoneType => zoneType;
        public string Title => title;
        public Sprite BaseSprite => baseSprite;
        public Sprite IndicatorSprite => indicatorSprite;
        public Sprite BombSprite => bombSprite;
        public IReadOnlyList<WheelSliceData> Slices => slices;

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Safety net for design mistakes: safe & super wheels must never contain a bomb.
            if (zoneType == ZoneType.Normal) return;
            foreach (var slice in slices)
            {
                if (slice != null && slice.IsBomb)
                    Debug.LogError($"[WheelConfig] '{name}' is a {zoneType} wheel but contains a bomb slice!", this);
            }
        }
#endif
    }
}
