using System;
using UnityEngine;

namespace WheelGame.Configs
{
    /// <summary>One slice of a wheel, edited inline inside a WheelConfig asset.</summary>
    [Serializable]
    public class WheelSliceData
    {
        [SerializeField] private RewardDefinition reward;
        [Tooltip("Base amount at zone 1. Scaled up every zone by GameConfig.rewardGrowthPerZone.")]
        [SerializeField] private int baseAmount = 1;
        [SerializeField] private bool isBomb;

        public RewardDefinition Reward => reward;
        public int BaseAmount => baseAmount;
        public bool IsBomb => isBomb;
    }
}
