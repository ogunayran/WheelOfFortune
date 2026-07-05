using System.Collections.Generic;
using UnityEngine;
using WheelGame.Configs;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// Zone number strip at the top (Critical Strike style): past zones dimmed,
    /// safe zones green, super zones orange, current zone boxed with a pointer.
    /// Rebuilds around the current zone on every ZoneChanged event.
    /// </summary>
    public class ZoneProgressView : AutoBoundView
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private RectTransform chipContainer; // has a HorizontalLayoutGroup
        [SerializeField] private ZoneChipView chipPrefab;
        [SerializeField, Min(3)] private int visibleChipCount = 9;

        [Header("Reference palette")]
        [SerializeField] private Color pastColor = new Color(0.45f, 0.45f, 0.45f, 1f);
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color safeColor = new Color(0.48f, 0.82f, 0.17f, 1f);
        [SerializeField] private Color superColor = new Color(0.96f, 0.65f, 0.14f, 1f);
        [SerializeField] private Color currentTextColor = Color.white;

        private readonly List<ZoneChipView> chips = new List<ZoneChipView>();

        protected override void BindReferences()
        {
            chipContainer = FindDeep<RectTransform>("ui_container_zones_chips");
        }

        private void OnEnable() => GameEvents.ZoneChanged += HandleZoneChanged;
        private void OnDisable() => GameEvents.ZoneChanged -= HandleZoneChanged;

        private void HandleZoneChanged(int currentZone, ZoneType _)
        {
            EnsureChipCount();

            int firstZone = Mathf.Max(1, currentZone - 1);
            for (int i = 0; i < chips.Count; i++)
            {
                int zone = firstZone + i;
                bool isCurrent = zone == currentZone;
                chips[i].Set(zone, isCurrent, TextColorFor(zone, currentZone), BoxColorFor(zone));
            }

            int currentIndex = currentZone - firstZone;
            if (currentIndex >= 0 && currentIndex < chips.Count)
                chips[currentIndex].PlayFocus();
        }

        private Color TextColorFor(int zone, int currentZone)
        {
            if (zone == currentZone) return currentTextColor;
            if (zone < currentZone) return pastColor;

            switch (gameConfig.GetZoneType(zone))
            {
                case ZoneType.Super: return superColor;
                case ZoneType.Safe: return safeColor;
                default: return normalColor;
            }
        }

        /// <summary>The current-zone highlight box is tinted by the zone's type.</summary>
        private Color BoxColorFor(int zone)
        {
            switch (gameConfig.GetZoneType(zone))
            {
                case ZoneType.Super: return superColor;
                case ZoneType.Safe: return safeColor;
                default: return new Color(0.32f, 0.36f, 0.32f, 1f);
            }
        }

        private void EnsureChipCount()
        {
            while (chips.Count < visibleChipCount)
                chips.Add(Instantiate(chipPrefab, chipContainer));

            for (int i = 0; i < chips.Count; i++)
                chips[i].gameObject.SetActive(i < visibleChipCount);
        }
    }
}
