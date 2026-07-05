using System;

namespace WheelGame.Core
{
    /// <summary>
    /// Premium currency used by the revive system (bonus feature).
    /// Plain C# class, unit-testable, no Unity dependency.
    /// </summary>
    public class CurrencyWallet
    {
        public int Balance { get; private set; }

        public event Action<int> BalanceChanged;

        public CurrencyWallet(int startingBalance)
        {
            Balance = Math.Max(0, startingBalance);
        }

        public bool CanAfford(int cost) => cost >= 0 && Balance >= cost;

        public bool TrySpend(int cost)
        {
            if (!CanAfford(cost)) return false;
            Balance -= cost;
            BalanceChanged?.Invoke(Balance);
            return true;
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            Balance += amount;
            BalanceChanged?.Invoke(Balance);
        }
    }
}
