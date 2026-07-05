using System;
using System.Collections.Generic;
using WheelGame.Configs;

namespace WheelGame.Core
{
    /// <summary>
    /// Central event hub (Observer pattern). Gameplay raises events, UI listens.
    /// Keeps UI completely decoupled from game logic: views never call managers directly,
    /// and managers never know which views exist.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>Raised whenever the game state changes.</summary>
        public static event Action<GameState> StateChanged;

        /// <summary>Raised when the player enters a zone (also on restart for zone 1).</summary>
        public static event Action<int, ZoneType> ZoneChanged;

        /// <summary>Raised the moment the wheel starts spinning.</summary>
        public static event Action SpinStarted;

        /// <summary>Raised every time a slice boundary passes under the indicator while spinning.</summary>
        public static event Action SpinTick;

        /// <summary>Raised when the wheel stops. Carries the slice that won and the zone-scaled amount.</summary>
        public static event Action<WheelSliceData, int> SpinResolved;

        /// <summary>Raised after a (non-bomb) reward is added to the session inventory.</summary>
        public static event Action<RewardStack> RewardGranted;

        /// <summary>Raised when the bomb slice is hit, before the player chooses give up / revive.</summary>
        public static event Action BombHit;

        /// <summary>Raised when the player leaves and collects everything gathered so far.</summary>
        public static event Action<IReadOnlyList<RewardStack>> CashedOut;

        /// <summary>Raised when a fresh run starts (first launch, after give up, after cash out).</summary>
        public static event Action RunRestarted;

        /// <summary>Raised when the player pays to continue after a bomb.</summary>
        public static event Action Revived;

        /// <summary>Raised when the premium currency balance changes (revive economy).</summary>
        public static event Action<int> CurrencyChanged;

        public static void RaiseStateChanged(GameState state) => StateChanged?.Invoke(state);
        public static void RaiseZoneChanged(int zone, ZoneType type) => ZoneChanged?.Invoke(zone, type);
        public static void RaiseSpinStarted() => SpinStarted?.Invoke();
        public static void RaiseSpinTick() => SpinTick?.Invoke();
        public static void RaiseSpinResolved(WheelSliceData slice, int amount) => SpinResolved?.Invoke(slice, amount);
        public static void RaiseRewardGranted(RewardStack stack) => RewardGranted?.Invoke(stack);
        public static void RaiseBombHit() => BombHit?.Invoke();
        public static void RaiseCashedOut(IReadOnlyList<RewardStack> rewards) => CashedOut?.Invoke(rewards);
        public static void RaiseRunRestarted() => RunRestarted?.Invoke();
        public static void RaiseRevived() => Revived?.Invoke();
        public static void RaiseCurrencyChanged(int balance) => CurrencyChanged?.Invoke(balance);
    }
}
