using UnityEngine;
using WheelGame.Core;

namespace WheelGame.Configs
{
    /// <summary>
    /// Single source of truth for game rules and tuning. One asset in the project;
    /// every rule a designer may want to change lives here, not in code.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "WheelGame/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Zone rules")]
        [Tooltip("Every Nth zone is a risk-free silver spin.")]
        [SerializeField, Min(2)] private int safeZoneInterval = 5;
        [Tooltip("Every Nth zone is a risk-free golden spin with special rewards.")]
        [SerializeField, Min(2)] private int superZoneInterval = 30;

        [Header("Reward scaling")]
        [Tooltip("Rewards get better every zone: amount = baseAmount * (1 + (zone-1) * growth).")]
        [SerializeField, Min(0f)] private float rewardGrowthPerZone = 0.25f;

        [Header("Wheels")]
        [SerializeField] private WheelConfig normalWheel; // bronze
        [SerializeField] private WheelConfig safeWheel;   // silver
        [SerializeField] private WheelConfig superWheel;  // golden

        [Header("Revive (bonus feature)")]
        [SerializeField, Min(0)] private int reviveCostCurrency = 25;
        [SerializeField, Min(0)] private int startingCurrency = 50;
        [Tooltip("Reward that converts into wallet currency when the player cashes out.")]
        [SerializeField] private RewardDefinition currencyReward;

        [Header("Spin feel")]
        [SerializeField, Min(0.5f)] private float spinDuration = 4f;
        [SerializeField, Min(1)] private int spinFullTurns = 5;

        public int SafeZoneInterval => safeZoneInterval;
        public int SuperZoneInterval => superZoneInterval;
        public float RewardGrowthPerZone => rewardGrowthPerZone;
        public int ReviveCostCurrency => reviveCostCurrency;
        public int StartingCurrency => startingCurrency;
        public RewardDefinition CurrencyReward => currencyReward;
        public float SpinDuration => spinDuration;
        public int SpinFullTurns => spinFullTurns;

        /// <summary>Zone type from zone index (1-based). Super has priority over safe (zone 30).</summary>
        public ZoneType GetZoneType(int zone)
        {
            if (zone % superZoneInterval == 0) return ZoneType.Super;
            if (zone % safeZoneInterval == 0) return ZoneType.Safe;
            return ZoneType.Normal;
        }

        public WheelConfig GetWheelConfig(ZoneType type)
        {
            switch (type)
            {
                case ZoneType.Super: return superWheel;
                case ZoneType.Safe: return safeWheel;
                default: return normalWheel;
            }
        }

        /// <summary>Rewards get better every zone.</summary>
        public int ScaleAmountForZone(int baseAmount, int zone)
        {
            return Mathf.Max(1, Mathf.RoundToInt(baseAmount * (1f + (zone - 1) * rewardGrowthPerZone)));
        }
    }
}
