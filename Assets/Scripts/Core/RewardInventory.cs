using System.Collections.Generic;
using System.Linq;
using WheelGame.Configs;

namespace WheelGame.Core
{
    /// <summary>
    /// Rewards collected during the current run. Plain C# class (no UnityEngine dependency)
    /// so it is trivially unit-testable. Lost entirely when the bomb resolves with "give up".
    /// </summary>
    public class RewardInventory
    {
        private readonly Dictionary<RewardDefinition, int> amounts = new Dictionary<RewardDefinition, int>();

        public bool IsEmpty => amounts.Count == 0;

        public void Add(RewardDefinition definition, int amount)
        {
            if (definition == null || amount <= 0) return;
            amounts.TryGetValue(definition, out int current);
            amounts[definition] = current + amount;
        }

        public int GetAmount(RewardDefinition definition)
        {
            return definition != null && amounts.TryGetValue(definition, out int amount) ? amount : 0;
        }

        public IReadOnlyList<RewardStack> ToStacks()
        {
            return amounts.Select(pair => new RewardStack(pair.Key, pair.Value)).ToList();
        }

        public void Clear() => amounts.Clear();
    }
}
