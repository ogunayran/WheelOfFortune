using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>One row/cell showing a collected reward: icon + total amount.</summary>
    public class RewardItemView : AutoBoundView
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text amountText;

        protected override void BindReferences()
        {
            iconImage = FindDeep<Image>("ui_image_reward_item_icon_value");
            amountText = FindDeep<TMP_Text>("ui_text_reward_item_amount_value");
        }

        public void Set(RewardStack stack)
        {
            iconImage.sprite = stack.Definition.Icon;
            amountText.text = $"x{stack.Amount}";

            transform.DOKill(true);
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.12f, 0.25f, 5, 0.8f);
        }

        private void OnDisable() => transform.DOKill();
    }
}
