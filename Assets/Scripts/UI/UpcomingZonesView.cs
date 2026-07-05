using TMPro;
using UnityEngine;
using WheelGame.Configs;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// Top-right badges showing the next safe zone and next super zone,
    /// exactly like the Critical Strike card game reference.
    /// </summary>
    public class UpcomingZonesView : AutoBoundView
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private TMP_Text nextSafeText;
        [SerializeField] private TMP_Text nextSuperText;

        protected override void BindReferences()
        {
            nextSafeText = FindDeep<TMP_Text>("ui_text_badge_safe_value");
            nextSuperText = FindDeep<TMP_Text>("ui_text_badge_super_value");
        }

        private void OnEnable() => GameEvents.ZoneChanged += HandleZoneChanged;
        private void OnDisable() => GameEvents.ZoneChanged -= HandleZoneChanged;

        private void HandleZoneChanged(int zone, ZoneType _)
        {
            nextSafeText.text = NextMultipleAfter(zone, gameConfig.SafeZoneInterval).ToString();
            nextSuperText.text = NextMultipleAfter(zone, gameConfig.SuperZoneInterval).ToString();
        }

        /// <summary>Smallest multiple of interval strictly greater than zone (zone 6, 5 -> 10).</summary>
        private static int NextMultipleAfter(int zone, int interval)
        {
            return (zone / interval + 1) * interval;
        }
    }
}
