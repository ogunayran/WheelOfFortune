using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WheelGame.Configs;

namespace WheelGame.Wheel
{
    /// <summary>
    /// Builds the wheel from a WheelConfig (slices are data, never hand-placed) and
    /// animates the spin. Which slice wins is decided by the caller (GameManager);
    /// this class only makes the wheel land on it — presentation, not rules.
    /// </summary>
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private WheelView view;
        [SerializeField] private RectTransform sliceContainer; // the part that rotates
        [SerializeField] private WheelSliceView slicePrefab;
        [Tooltip("Distance of slot centers from the wheel center, in reference resolution pixels.")]
        [SerializeField, Min(10f)] private float slotRadius = 210f;
        [Tooltip("Random landing offset inside the winning slice, as a fraction of slice angle.")]
        [SerializeField, Range(0f, 0.45f)] private float landingJitter = 0.3f;

        private readonly List<WheelSliceView> sliceViews = new List<WheelSliceView>();
        private WheelConfig currentConfig;
        private Tween spinTween;
        private Tween pendingResolve;

        public bool IsSpinning => spinTween != null && spinTween.IsActive() && spinTween.IsPlaying();

        /// <summary>Rebuilds the wheel for the given zone. Amounts shown are already zone-scaled.</summary>
        public void Build(WheelConfig config, int zone, GameConfig gameConfig)
        {
            currentConfig = config;
            view.SetAppearance(config);

            KillSpin();
            sliceContainer.localEulerAngles = Vector3.zero;

            int count = config.Slices.Count;
            float anglePerSlice = 360f / count;

            EnsureSliceViewCount(count);

            for (int i = 0; i < count; i++)
            {
                WheelSliceData data = config.Slices[i];
                WheelSliceView sliceView = sliceViews[i];

                // Slice i sits clockwise from the top slot, tilted with the wheel like the reference art.
                float angle = i * anglePerSlice;
                var rect = (RectTransform)sliceView.transform;
                rect.localEulerAngles = new Vector3(0f, 0f, -angle);
                rect.anchoredPosition = Vector2.zero;
                sliceView.SetSlotRadius(slotRadius);

                if (data.IsBomb)
                    sliceView.SetBomb(config.BombSprite);
                else
                    sliceView.SetReward(data.Reward != null ? data.Reward.Icon : null,
                                        gameConfig.ScaleAmountForZone(data.BaseAmount, zone));
            }
        }

        /// <summary>Spins the wheel so that slice at resultIndex lands under the indicator.</summary>
        public void Spin(int resultIndex, float duration, int fullTurns, Action onCompleted)
        {
            if (currentConfig == null || IsSpinning) return;

            int count = currentConfig.Slices.Count;
            float anglePerSlice = 360f / count;

            float jitter = UnityEngine.Random.Range(-landingJitter, landingJitter) * anglePerSlice;
            float targetZ = fullTurns * 360f + resultIndex * anglePerSlice + jitter;

            KillSpin();
            sliceContainer.localEulerAngles = Vector3.zero;

            // Ratchet ticks: raise an event whenever a slice boundary passes the indicator.
            float traveled = 0f, prevZ = 0f, nextTickAt = anglePerSlice;
            spinTween = sliceContainer
                .DORotate(new Vector3(0f, 0f, targetZ), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuart)
                .OnUpdate(() =>
                {
                    float z = sliceContainer.localEulerAngles.z;
                    traveled += Mathf.Abs(Mathf.DeltaAngle(prevZ, z));
                    prevZ = z;
                    while (traveled >= nextTickAt)
                    {
                        Core.GameEvents.RaiseSpinTick();
                        nextTickAt += anglePerSlice;
                    }
                })
                .OnComplete(() =>
                {
                    spinTween = null;
                    // Small beat: highlight the winning slice, then let the game resolve it.
                    WheelSliceView winner = sliceViews[resultIndex];
                    winner.transform.DOPunchScale(Vector3.one * 0.18f, 0.35f, 6, 0.8f);
                    pendingResolve = DOVirtual.DelayedCall(0.45f, () => onCompleted?.Invoke());
                });
        }

        private void EnsureSliceViewCount(int count)
        {
            while (sliceViews.Count < count)
                sliceViews.Add(Instantiate(slicePrefab, sliceContainer));

            for (int i = 0; i < sliceViews.Count; i++)
                sliceViews[i].gameObject.SetActive(i < count);
        }

        private void KillSpin()
        {
            spinTween?.Kill();
            spinTween = null;
            pendingResolve?.Kill();
            pendingResolve = null;
        }

        private void OnDestroy() => KillSpin();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (view == null) view = GetComponentInChildren<WheelView>(true);
            if (sliceContainer == null)
            {
                foreach (var t in GetComponentsInChildren<RectTransform>(true))
                    if (t.name == "ui_container_spin_slices") { sliceContainer = t; break; }
            }
        }
#endif
    }
}
