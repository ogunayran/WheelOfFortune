using UnityEditor;
using UnityEngine;
using WheelGame.Configs;
using WheelGame.Core;

namespace WheelGame.EditorTools
{
    /// <summary>
    /// Fast logic checks for the plain C# core (inventory, wallet, zone rules).
    /// Run from the menu: WheelGame > Run Logic Self Tests. The core classes have no
    /// Unity dependency, which is what makes them this easy to test.
    /// </summary>
    public static class LogicSelfTests
    {
        private static int passed, failed;

        [MenuItem("WheelGame/Run Logic Self Tests")]
        public static void Run()
        {
            passed = failed = 0;

            // --- RewardInventory ---
            var cash = ScriptableObject.CreateInstance<RewardDefinition>();
            var inventory = new RewardInventory();
            Check(inventory.IsEmpty, "inventory starts empty");
            inventory.Add(cash, 100);
            inventory.Add(cash, 50);
            Check(inventory.GetAmount(cash) == 150, "amounts accumulate per reward");
            Check(inventory.ToStacks().Count == 1, "same reward merges into one stack");
            inventory.Add(cash, -5);
            Check(inventory.GetAmount(cash) == 150, "negative amounts are ignored");
            inventory.Clear();
            Check(inventory.IsEmpty, "clear empties the inventory");

            // --- CurrencyWallet ---
            var wallet = new CurrencyWallet(50);
            Check(wallet.CanAfford(25), "wallet affords revive cost");
            Check(wallet.TrySpend(25) && wallet.Balance == 25, "spending reduces balance");
            Check(!wallet.TrySpend(26) && wallet.Balance == 25, "overspending is rejected");
            wallet.Add(10);
            Check(wallet.Balance == 35, "adding currency works");

            // --- GameConfig zone rules (defaults: safe=5, super=30, growth=0.25) ---
            var config = ScriptableObject.CreateInstance<GameConfig>();
            Check(config.GetZoneType(1) == ZoneType.Normal, "zone 1 is normal");
            Check(config.GetZoneType(5) == ZoneType.Safe, "zone 5 is safe");
            Check(config.GetZoneType(29) == ZoneType.Normal, "zone 29 is normal");
            Check(config.GetZoneType(30) == ZoneType.Super, "zone 30 is super (beats safe)");
            Check(config.GetZoneType(60) == ZoneType.Super, "zone 60 is super");
            Check(config.ScaleAmountForZone(100, 1) == 100, "zone 1 has no growth");
            Check(config.ScaleAmountForZone(100, 9) == 300, "zone 9 triples base at 0.25 growth");

            Object.DestroyImmediate(cash);
            Object.DestroyImmediate(config);

            if (failed == 0) Debug.Log($"[SelfTest] All {passed} checks passed. ✔");
            else Debug.LogError($"[SelfTest] {failed} of {passed + failed} checks FAILED.");
        }

        private static void Check(bool condition, string name)
        {
            if (condition) { passed++; return; }
            failed++;
            Debug.LogError($"[SelfTest] FAILED: {name}");
        }
    }
}
