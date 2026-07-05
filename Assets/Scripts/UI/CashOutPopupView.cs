using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// Shown when the player walks away: lists everything they leave with,
    /// and offers to start a fresh run.
    /// </summary>
    public class CashOutPopupView : AutoBoundView
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private RectTransform cardRoot;
        [SerializeField] private RectTransform itemContainer; // has a layout group
        [SerializeField] private RewardItemView itemPrefab;
        [SerializeField] private Button playAgainButton;

        private readonly List<RewardItemView> spawnedItems = new List<RewardItemView>();

        protected override void BindReferences()
        {
            panelRoot = FindDeep<RectTransform>("ui_panel_popup_cashout")?.gameObject;
            cardRoot = FindDeep<RectTransform>("ui_panel_cashout_card");
            itemContainer = FindDeep<RectTransform>("ui_container_cashout_items");
            playAgainButton = FindDeep<Button>("ui_button_cashout_playagain");
#if UNITY_EDITOR
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>(true);
#endif
        }

        private void Awake()
        {
            playAgainButton.onClick.AddListener(gameManager.StartNewRun);
            panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.CashedOut += Show;
            GameEvents.RunRestarted += Hide;
        }

        private void OnDisable()
        {
            GameEvents.CashedOut -= Show;
            GameEvents.RunRestarted -= Hide;
        }

        private void Show(IReadOnlyList<RewardStack> rewards)
        {
            foreach (RewardItemView item in spawnedItems)
                Destroy(item.gameObject);
            spawnedItems.Clear();

            foreach (RewardStack stack in rewards)
            {
                RewardItemView item = Instantiate(itemPrefab, itemContainer);
                item.Set(stack);
                spawnedItems.Add(item);
            }

            panelRoot.SetActive(true);
            cardRoot.DOKill();
            cardRoot.localScale = Vector3.one * 0.75f;
            cardRoot.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        private void Hide() => panelRoot.SetActive(false);
    }
}
