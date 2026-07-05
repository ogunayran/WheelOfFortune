using UnityEngine;
using WheelGame.Configs;
using WheelGame.Wheel;

namespace WheelGame.Core
{
    /// <summary>
    /// Single owner of the game flow (state machine): zone progression, spin requests,
    /// reward granting, bomb resolution, cash out and revive. UI talks to this class
    /// only through the Request* methods; results flow back through GameEvents.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private WheelController wheel;

        private readonly RewardInventory inventory = new RewardInventory();
        private CurrencyWallet wallet;
        private WheelConfig currentWheelConfig;
        private GameState state;
        private int currentZone;

        public GameState State => state;
        public int CurrentZone => currentZone;

        public const string WalletPrefsKey = "wheelgame.wallet";

        private void Awake()
        {
            Application.targetFrameRate = 60; // mobile default is often 30

            // Wallet persists across sessions (bonus feature).
            wallet = new CurrencyWallet(PlayerPrefs.GetInt(WalletPrefsKey, config.StartingCurrency));
            wallet.BalanceChanged += GameEvents.RaiseCurrencyChanged;
            wallet.BalanceChanged += SaveWallet;
        }

        private void SaveWallet(int balance)
        {
            PlayerPrefs.SetInt(WalletPrefsKey, balance);
            PlayerPrefs.Save();
        }

        private void Start()
        {
            GameEvents.RaiseCurrencyChanged(wallet.Balance);
            StartNewRun();
        }

        /// <summary>Fresh run: zone 1, empty inventory. Used on launch, after give up and after cash out.</summary>
        public void StartNewRun()
        {
            inventory.Clear();
            currentZone = 1;
            GameEvents.RaiseRunRestarted();
            EnterCurrentZone();
            SetState(GameState.Idle);
        }

        /// <summary>Called by the spin button (listener added in code, never from the editor).</summary>
        public void RequestSpin()
        {
            if (state != GameState.Idle) return;

            SetState(GameState.Spinning);
            GameEvents.RaiseSpinStarted();

            int resultIndex = Random.Range(0, currentWheelConfig.Slices.Count);
            wheel.Spin(resultIndex, config.SpinDuration, config.SpinFullTurns,
                () => ResolveSlice(currentWheelConfig.Slices[resultIndex]));
        }

        /// <summary>Leave with everything collected so far. Only while the wheel is not spinning.</summary>
        public void RequestCashOut()
        {
            if (state != GameState.Idle || inventory.IsEmpty) return;

            SetState(GameState.CashedOut);
            var stacks = inventory.ToStacks();

            // Gold rewards convert into the persistent revive wallet; in a full game the
            // remaining rewards would be written to the player's account inventory.
            foreach (RewardStack stack in stacks)
            {
                if (config.CurrencyReward != null && stack.Definition == config.CurrencyReward)
                    wallet.Add(stack.Amount);
            }

            GameEvents.RaiseCashedOut(stacks);
        }

        /// <summary>Bomb popup: abandon all rewards and restart from zone 1.</summary>
        public void RequestGiveUp()
        {
            if (state != GameState.BombResolve) return;
            StartNewRun();
        }

        /// <summary>Bomb popup (bonus feature): pay currency, keep rewards, respin the same zone.</summary>
        public void RequestRevive()
        {
            if (state != GameState.BombResolve) return;
            if (!wallet.TrySpend(config.ReviveCostCurrency)) return;

            GameEvents.RaiseRevived();
            EnterCurrentZone(); // same zone, wheel rebuilt, rewards kept
            SetState(GameState.Idle);
        }

        public bool CanAffordRevive => wallet.CanAfford(config.ReviveCostCurrency);

        private void ResolveSlice(WheelSliceData slice)
        {
            int amount = config.ScaleAmountForZone(slice.BaseAmount, currentZone);
            GameEvents.RaiseSpinResolved(slice, amount);

            if (slice.IsBomb)
            {
                SetState(GameState.BombResolve);
                GameEvents.RaiseBombHit();
                return;
            }

            inventory.Add(slice.Reward, amount);
            GameEvents.RaiseRewardGranted(new RewardStack(slice.Reward, amount));

            currentZone++;
            EnterCurrentZone();
            SetState(GameState.Idle);
        }

        private void EnterCurrentZone()
        {
            ZoneType type = config.GetZoneType(currentZone);
            currentWheelConfig = config.GetWheelConfig(type);
            wheel.Build(currentWheelConfig, currentZone, config);
            GameEvents.RaiseZoneChanged(currentZone, type);
        }

        private void SetState(GameState newState)
        {
            state = newState;
            GameEvents.RaiseStateChanged(state);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (wheel == null) wheel = FindObjectOfType<WheelController>(true);
        }
#endif
    }
}
