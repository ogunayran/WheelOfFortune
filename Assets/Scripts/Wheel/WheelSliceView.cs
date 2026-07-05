using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.UI;

namespace WheelGame.Wheel
{
    /// <summary>
    /// Visual of a single slice: reward icon + amount label sitting in one revolver slot.
    /// Instantiated and positioned by WheelController; holds no game logic.
    /// </summary>
    public class WheelSliceView : AutoBoundView
    {
        [SerializeField] private RectTransform contentRoot; // offset from wheel center = slot radius
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text amountText;

        protected override void BindReferences()
        {
            contentRoot = FindDeep<RectTransform>("ui_container_slice_content");
            iconImage = FindDeep<Image>("ui_image_slice_icon_value");
            amountText = FindDeep<TMP_Text>("ui_text_slice_amount_value");
        }

        /// <summary>Pushes the slice content out to the slot ring.</summary>
        public void SetSlotRadius(float radius)
        {
            contentRoot.anchoredPosition = new Vector2(0f, radius);
        }

        public void SetReward(Sprite icon, int amount)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
            amountText.text = $"x{amount}";
            amountText.enabled = true;
        }

        public void SetBomb(Sprite bombIcon)
        {
            iconImage.sprite = bombIcon;
            iconImage.enabled = bombIcon != null;
            amountText.enabled = false; // bomb has no amount label
        }
    }
}
