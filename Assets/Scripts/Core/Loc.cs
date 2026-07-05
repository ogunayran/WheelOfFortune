using System;
using System.Collections.Generic;
using UnityEngine;

namespace WheelGame.Core
{
    public enum GameLanguage { English = 0, Turkish = 1 }

    /// <summary>
    /// Minimal string table for the two supported languages. The selected language
    /// persists in PlayerPrefs; views listen to Changed and refresh themselves.
    /// </summary>
    public static class Loc
    {
        private const string PrefsKey = "wheelgame.language";

        public static event Action Changed;

        private static GameLanguage current = (GameLanguage)PlayerPrefs.GetInt(PrefsKey, 0);
        public static GameLanguage Current => current;

        private static readonly Dictionary<string, (string en, string tr)> Table =
            new Dictionary<string, (string, string)>
        {
            { "menu.play", ("PLAY", "OYNA") },
            { "menu.subtitle", ("RISK IT. SPIN IT. WIN IT.", "RİSKE GİR. ÇEVİR. KAZAN.") },
            { "menu.sound_on", ("SOUND: ON", "SES: AÇIK") },
            { "menu.sound_off", ("SOUND: OFF", "SES: KAPALI") },
            { "game.spin", ("SPIN", "ÇEVİR") },
            { "game.exit", ("EXIT", "ÇIKIŞ") },
            { "badge.super", ("SUPER\nZONE", "SÜPER\nBÖLGE") },
            { "badge.safe", ("SAFE\nZONE", "GÜVENLİ\nBÖLGE") },
            { "bomb.title", ("OH NO, A BOMB EXPLODED\nRIGHT IN YOUR HANDS!", "EYVAH, BOMBA\nELİNDE PATLADI!") },
            { "bomb.subtitle", ("Revive yourself to keep your rewards.", "Ödüllerini korumak için canlan.") },
            { "bomb.giveup", ("GIVE UP", "PES ET") },
            { "bomb.revive", ("REVIVE", "CANLAN") },
            { "cashout.title", ("CONGRATULATIONS!", "TEBRİKLER!") },
            { "cashout.subtitle", ("You walked away with your rewards.", "Ödüllerinle birlikte ayrıldın.") },
            { "cashout.playagain", ("PLAY AGAIN", "TEKRAR OYNA") },
        };

        public static string Get(string key)
        {
            if (Table.TryGetValue(key, out var entry))
                return current == GameLanguage.Turkish ? entry.tr : entry.en;
            return key; // visible fallback makes missing keys easy to spot
        }

        public static void Toggle()
        {
            Set(current == GameLanguage.English ? GameLanguage.Turkish : GameLanguage.English);
        }

        public static void Set(GameLanguage language)
        {
            if (language == current) return;
            current = language;
            PlayerPrefs.SetInt(PrefsKey, (int)language);
            PlayerPrefs.Save();
            Changed?.Invoke();
        }
    }
}
