using UnityEngine;

namespace WheelGame.Configs
{
    /// <summary>A reward type (cash, gold, weapon points, chest...), authored as an asset.</summary>
    [CreateAssetMenu(fileName = "Reward_", menuName = "WheelGame/Reward Definition")]
    public class RewardDefinition : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [Tooltip("Played when this reward is won. Falls back to SfxPlayer's default when empty.")]
        [SerializeField] private AudioClip pickupSound;

        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public AudioClip PickupSound => pickupSound;
    }
}
