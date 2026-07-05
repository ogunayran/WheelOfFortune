using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelGame.UI
{
    /// <summary>
    /// One number in the zone strip. Matches the Critical Strike reference:
    /// plain colored number normally; the current zone gets a highlight box
    /// and a pointer notch above it.
    /// </summary>
    public class ZoneChipView : AutoBoundView
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject pointer;
        [SerializeField] private TMP_Text zoneNumberText;

        protected override void BindReferences()
        {
            backgroundImage = FindDeep<Image>("ui_image_zone_chip_bg");
            pointer = FindDeep<RectTransform>("ui_image_zone_chip_pointer")?.gameObject;
            zoneNumberText = FindDeep<TMP_Text>("ui_text_zone_chip_value");
        }

        public void Set(int zoneNumber, bool isCurrent, Color textColor, Color boxColor)
        {
            zoneNumberText.text = zoneNumber.ToString();
            zoneNumberText.color = textColor;
            zoneNumberText.fontStyle = isCurrent ? FontStyles.Bold : FontStyles.Normal;

            backgroundImage.enabled = isCurrent;
            backgroundImage.color = boxColor;
            pointer.SetActive(isCurrent);
        }

        public void PlayFocus()
        {
            transform.DOKill(true);
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 5, 0.7f);
        }

        private void OnDisable() => transform.DOKill();
    }
}
