using WheelGame.Configs;

namespace WheelGame.Core
{
    /// <summary>
    /// An amount of a specific reward, e.g. "Cash x400". Immutable value type.
    /// </summary>
    public readonly struct RewardStack
    {
        public readonly RewardDefinition Definition;
        public readonly int Amount;

        public RewardStack(RewardDefinition definition, int amount)
        {
            Definition = definition;
            Amount = amount;
        }
    }
}
