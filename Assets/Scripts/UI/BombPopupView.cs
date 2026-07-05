using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// "OH NO, A BOMB EXPLODED" popup. Give up restarts the run; revive (bonus feature)
    /// spends currency and lets the player keep the rewards.
    /// </summary>
    public class BombPopupView : AutoBoundView
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private RectTransform cardRoot;
        [SerializeField] private Button giveUpButton;
        [SerializeField] private Button reviveButton;
        [SerializeField] private TMP_Text reviveCostText;

        private Tween showDelay;

        protected override void BindReferences()
        {
            panelRoot = FindDeep<RectTransform>("ui_panel_popup_bomb")?.gameObject;
            cardRoot = FindDeep<RectTransform>("ui_panel_bomb_card");
            giveUpButton = FindDeep<Button>("ui_button_bomb_giveup");
            reviveButton = FindDeep<Button>("ui_button_bomb_revive");
            reviveCostText = FindDeep<TMP_Text>("ui_text_bomb_revive_cost_value");
#if UNITY_EDITOR
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>(true);
#endif
        }

        private void Awake()
        {
            giveUpButton.onClick.AddListener(gameManager.RequestGiveUp);
            reviveButton.onClick.AddListener(gameManager.RequestRevive);
            panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.BombHit += Show;
            GameEvents.RunRestarted += Hide;
            GameEvents.Revived += Hide;
        }

        private void OnDisable()
        {
            GameEvents.BombHit -= Show;
            GameEvents.RunRestarted -= Hide;
            GameEvents.Revived -= Hide;
        }

        private void Show()
        {
            // Give the wheel shake a moment to play before the popup lands on top of it.
            showDelay?.Kill();
            showDelay = DOVirtual.DelayedCall(0.55f, () =>
            {
                reviveButton.interactable = gameManager.CanAffordRevive;
                panelRoot.SetActive(true);
                cardRoot.DOKill();
                cardRoot.localScale = Vector3.one * 0.75f;
                cardRoot.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            });
        }

        private void Hide()
        {
            showDelay?.Kill();
            panelRoot.SetActive(false);
        }
    }
}
