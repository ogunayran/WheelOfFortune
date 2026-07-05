using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// In-game settings dropdown (top-left): toggles for sound and language.
    /// Shares its persisted state with the title screen via the same PlayerPrefs keys.
    /// </summary>
    public class SettingsMenuView : AutoBoundView
    {
        private const string SoundPrefsKey = "wheelgame.sound";

        [SerializeField] private Button menuButton;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button languageButton;
        [SerializeField] private TMP_Text soundLabel;
        [SerializeField] private TMP_Text languageLabel;

        private bool soundOn;

        protected override void BindReferences()
        {
            menuButton = FindDeep<Button>("ui_button_settings");
            panelRoot = FindDeep<RectTransform>("ui_panel_settings")?.gameObject;
            soundButton = FindDeep<Button>("ui_button_settings_sound");
            languageButton = FindDeep<Button>("ui_button_settings_language");
            soundLabel = FindDeep<TMP_Text>("ui_text_settings_sound_value");
            languageLabel = FindDeep<TMP_Text>("ui_text_settings_language_value");
        }

        private void Awake()
        {
            menuButton.onClick.AddListener(TogglePanel);
            soundButton.onClick.AddListener(ToggleSound);
            languageButton.onClick.AddListener(Loc.Toggle);
            panelRoot.SetActive(false);
        }

        private void OnEnable() => Loc.Changed += RefreshLabels;
        private void OnDisable() => Loc.Changed -= RefreshLabels;

        private void TogglePanel()
        {
            bool open = !panelRoot.activeSelf;
            if (open)
            {
                soundOn = PlayerPrefs.GetInt(SoundPrefsKey, 1) == 1; // stay in sync with the title screen
                RefreshLabels();
            }
            panelRoot.SetActive(open);
        }

        private void ToggleSound()
        {
            soundOn = !soundOn;
            PlayerPrefs.SetInt(SoundPrefsKey, soundOn ? 1 : 0);
            PlayerPrefs.Save();
            AudioListener.volume = soundOn ? 1f : 0f;
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            soundLabel.text = Loc.Get(soundOn ? "menu.sound_on" : "menu.sound_off");
            languageLabel.text = Loc.Current == GameLanguage.English ? "EN" : "TR";
        }
    }
}
