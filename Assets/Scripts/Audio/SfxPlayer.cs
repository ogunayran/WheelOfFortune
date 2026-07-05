using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.Audio
{
    /// <summary>
    /// Single place that turns game events into sounds. Reward sounds are data-driven:
    /// each RewardDefinition carries its own pickup clip (cash register for cash,
    /// bolt rack for weapons...), with a fallback for rewards that define none.
    /// Two sources so rapid wheel ticks (pitch-randomized) never cut other effects.
    /// </summary>
    public class SfxPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource mainSource;
        [SerializeField] private AudioSource tickSource;

        [Header("Clips")]
        [SerializeField] private AudioClip spinStart;
        [SerializeField] private AudioClip spinTick;
        [SerializeField] private AudioClip bombExplosion;
        [SerializeField] private AudioClip cashOutJingle;
        [SerializeField] private AudioClip reviveSound;
        [SerializeField] private AudioClip rewardFallback;
        [SerializeField] private AudioClip buttonClick;

        private void Start()
        {
            // Every button in the scene gets the same mechanical click.
            foreach (Button button in FindObjectsOfType<Button>(true))
                button.onClick.AddListener(PlayButtonClick);
        }

        private void PlayButtonClick()
        {
            if (tickSource == null || buttonClick == null) return;
            tickSource.pitch = Random.Range(0.97f, 1.03f);
            tickSource.PlayOneShot(buttonClick);
        }

        private void OnEnable()
        {
            GameEvents.SpinStarted += HandleSpinStarted;
            GameEvents.SpinTick += HandleSpinTick;
            GameEvents.BombHit += HandleBombHit;
            GameEvents.RewardGranted += HandleRewardGranted;
            GameEvents.CashedOut += HandleCashedOut;
            GameEvents.Revived += HandleRevived;
        }

        private void OnDisable()
        {
            GameEvents.SpinStarted -= HandleSpinStarted;
            GameEvents.SpinTick -= HandleSpinTick;
            GameEvents.BombHit -= HandleBombHit;
            GameEvents.RewardGranted -= HandleRewardGranted;
            GameEvents.CashedOut -= HandleCashedOut;
            GameEvents.Revived -= HandleRevived;
        }

        private void HandleSpinStarted() => Play(spinStart);
        private void HandleBombHit() => Play(bombExplosion);
        private void HandleCashedOut(IReadOnlyList<RewardStack> _) => Play(cashOutJingle);
        private void HandleRevived() => Play(reviveSound);

        private void HandleSpinTick()
        {
            if (tickSource == null || spinTick == null) return;
            tickSource.pitch = Random.Range(0.92f, 1.08f);
            tickSource.PlayOneShot(spinTick);
        }

        private void HandleRewardGranted(RewardStack stack)
        {
            AudioClip clip = stack.Definition != null && stack.Definition.PickupSound != null
                ? stack.Definition.PickupSound
                : rewardFallback;
            Play(clip);
        }

        private void Play(AudioClip clip)
        {
            if (mainSource != null && clip != null)
                mainSource.PlayOneShot(clip);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            var sources = GetComponents<AudioSource>();
            if (mainSource == null && sources.Length > 0) mainSource = sources[0];
            if (tickSource == null && sources.Length > 1) tickSource = sources[1];
        }
#endif
    }
}
