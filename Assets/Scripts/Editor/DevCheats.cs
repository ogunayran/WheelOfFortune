using UnityEditor;
using UnityEngine;
using WheelGame.Core;

namespace WheelGame.EditorTools
{
    /// <summary>
    /// Editor-only cheats for testing the revive economy. Never ships in a build.
    /// Use while NOT in play mode (the wallet is loaded once on Awake).
    /// </summary>
    public static class DevCheats
    {
        [MenuItem("WheelGame/Cheats/Set Wallet To 200")]
        public static void SetWalletTo200() => SetWallet(200);

        [MenuItem("WheelGame/Cheats/Set Wallet To 1000")]
        public static void SetWalletTo1000() => SetWallet(1000);

        [MenuItem("WheelGame/Cheats/Reset Wallet Save")]
        public static void ResetWallet()
        {
            PlayerPrefs.DeleteKey(GameManager.WalletPrefsKey);
            PlayerPrefs.Save();
            Debug.Log("[DevCheats] Wallet save deleted; next run starts with the config default.");
        }

        private static void SetWallet(int amount)
        {
            PlayerPrefs.SetInt(GameManager.WalletPrefsKey, amount);
            PlayerPrefs.Save();
            Debug.Log($"[DevCheats] Wallet set to {amount}. Enter play mode to see it.");
        }
    }
}
