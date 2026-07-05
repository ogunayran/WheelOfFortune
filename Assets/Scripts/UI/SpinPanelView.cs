using UnityEngine;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// SPIN and EXIT (cash out) buttons. Listeners are added in code;
    /// interactability is driven by game state events.
    /// </summary>
    public class SpinPanelView : AutoBoundView
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button cashOutButton;

        private bool hasRewards;

        protected override void BindReferences()
        {
            spinButton = FindDeep<Button>("ui_button_spin");
            cashOutButton = FindDeep<Button>("ui_button_cashout");
#if UNITY_EDITOR
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>(true);
#endif
        }

        private void Awake()
        {
            spinButton.onClick.AddListener(gameManager.RequestSpin);
            cashOutButton.onClick.AddListener(gameManager.RequestCashOut);
        }

        private void OnEnable()
        {
            GameEvents.StateChanged += HandleStateChanged;
            GameEvents.RewardGranted += HandleRewardGranted;
            GameEvents.RunRestarted += HandleRunRestarted;
        }

        private void OnDisable()
        {
            GameEvents.StateChanged -= HandleStateChanged;
            GameEvents.RewardGranted -= HandleRewardGranted;
            GameEvents.RunRestarted -= HandleRunRestarted;
        }

        private void HandleStateChanged(GameState state)
        {
            bool idle = state == GameState.Idle;
            spinButton.interactable = idle;
            cashOutButton.interactable = idle && hasRewards;
        }

        private void HandleRewardGranted(RewardStack _)
        {
            hasRewards = true;
        }

        private void HandleRunRestarted()
        {
            hasRewards = false;
            cashOutButton.interactable = false;
        }
    }
}
