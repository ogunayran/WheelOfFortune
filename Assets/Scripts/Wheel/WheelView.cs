using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.Configs;
using WheelGame.Core;
using WheelGame.UI;

namespace WheelGame.Wheel
{
    /// <summary>
    /// Pure visual shell of the wheel: base sprite, indicator sprite and title.
    /// Swapped per zone type (bronze / silver / golden) from the WheelConfig.
    /// </summary>
    public class WheelView : AutoBoundView
    {
        [SerializeField] private Image baseImage;      // static: flat dark backplate disc (never swapped)
        [SerializeField] private Image gearImage;      // rotating: circular crop of the wheel art
        [SerializeField] private Image indicatorImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text centerZoneText;

        protected override void BindReferences()
        {
            baseImage = FindDeep<Image>("ui_image_spin_base");
            gearImage = FindDeep<Image>("ui_image_spin_gear_value");
            indicatorImage = FindDeep<Image>("ui_image_spin_indicator_value");
            titleText = FindDeep<TMP_Text>("ui_text_spin_title_value");
            centerZoneText = FindDeep<TMP_Text>("ui_text_spin_center_value");
        }

        public void SetAppearance(WheelConfig config)
        {
            // baseImage is a flat backplate disc and intentionally keeps its sprite;
            // only the rotating gear layer swaps art per wheel type.
            gearImage.sprite = config.BaseSprite;
            indicatorImage.sprite = config.IndicatorSprite;

            if (titleText.text != config.Title)
            {
                titleText.text = config.Title;
                titleText.transform.DOKill(true);
                titleText.transform.localScale = Vector3.one;
                titleText.transform.DOPunchScale(Vector3.one * 0.12f, 0.3f, 6, 0.7f);
            }
        }

        private void OnEnable()
        {
            GameEvents.BombHit += PlayBombShake;
            GameEvents.ZoneChanged += HandleZoneChanged;
        }

        private void OnDisable()
        {
            GameEvents.BombHit -= PlayBombShake;
            GameEvents.ZoneChanged -= HandleZoneChanged;
            transform.DOKill();
            if (titleText != null) titleText.transform.DOKill();
        }

        private void HandleZoneChanged(int zone, ZoneType _)
        {
            centerZoneText.text = zone.ToString();
        }

        private void PlayBombShake()
        {
            var rect = (RectTransform)transform;
            rect.DOKill(true);
            rect.DOShakeAnchorPos(0.55f, 32f, 28, 90f, false, true);
        }
    }
}
