using System.Collections.Generic;
using UnityEngine;
using WheelGame.Configs;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// Side panel listing everything collected this run. Keeps its own UI-side totals
    /// fed by events, so it never reaches into game logic.
    /// </summary>
    public class CollectedRewardsView : AutoBoundView
    {
        [SerializeField] private RectTransform itemContainer; // has a VerticalLayoutGroup
        [SerializeField] private RewardItemView itemPrefab;

        private readonly Dictionary<RewardDefinition, int> totals = new Dictionary<RewardDefinition, int>();
        private readonly Dictionary<RewardDefinition, RewardItemView> items = new Dictionary<RewardDefinition, RewardItemView>();

        protected override void BindReferences()
        {
            itemContainer = FindDeep<RectTransform>("ui_container_rewards_items");
        }

        private void OnEnable()
        {
            GameEvents.RewardGranted += HandleRewardGranted;
            GameEvents.RunRestarted += HandleRunRestarted;
        }

        private void OnDisable()
        {
            GameEvents.RewardGranted -= HandleRewardGranted;
            GameEvents.RunRestarted -= HandleRunRestarted;
        }

        private void HandleRewardGranted(RewardStack stack)
        {
            totals.TryGetValue(stack.Definition, out int current);
            totals[stack.Definition] = current + stack.Amount;

            if (!items.TryGetValue(stack.Definition, out RewardItemView item))
            {
                item = Instantiate(itemPrefab, itemContainer);
                items[stack.Definition] = item;
            }

            item.Set(new RewardStack(stack.Definition, totals[stack.Definition]));
        }

        private void HandleRunRestarted()
        {
            totals.Clear();
            foreach (RewardItemView item in items.Values)
                Destroy(item.gameObject);
            items.Clear();
        }
    }
}
