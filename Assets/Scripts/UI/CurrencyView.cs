using DG.Tweening;
using TMPro;
using UnityEngine;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>Top corner currency counter for the revive economy (bonus feature).</summary>
    public class CurrencyView : AutoBoundView
    {
        [SerializeField] private TMP_Text balanceText;

        protected override void BindReferences()
        {
            balanceText = FindDeep<TMP_Text>("ui_text_currency_value");
        }

        private void OnEnable() => GameEvents.CurrencyChanged += HandleCurrencyChanged;

        private void OnDisable()
        {
            GameEvents.CurrencyChanged -= HandleCurrencyChanged;
            if (balanceText != null) balanceText.transform.DOKill();
        }

        private void HandleCurrencyChanged(int balance)
        {
            if (balanceText.text == balance.ToString()) return;
            balanceText.text = balance.ToString();
            balanceText.transform.DOKill(true);
            balanceText.transform.localScale = Vector3.one;
            balanceText.transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 5, 0.8f);
        }
    }
}
