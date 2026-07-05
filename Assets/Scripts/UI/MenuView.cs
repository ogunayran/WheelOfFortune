using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// Title screen shown on launch: play, sound toggle and language toggle.
    /// Sound and language choices persist across sessions.
    /// </summary>
    public class MenuView : AutoBoundView
    {
        private const string SoundPrefsKey = "wheelgame.sound";

        [SerializeField] private Button playButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button languageButton;
        [SerializeField] private TMP_Text soundLabel;
        [SerializeField] private TMP_Text languageLabel;
        [SerializeField] private RectTransform decorWheel;
        [SerializeField] private AudioSource musicSource;

        private bool soundOn;

        protected override void BindReferences()
        {
            playButton = FindDeep<Button>("ui_button_menu_play");
            soundButton = FindDeep<Button>("ui_button_menu_sound");
            languageButton = FindDeep<Button>("ui_button_menu_language");
            soundLabel = FindDeep<TMP_Text>("ui_text_menu_sound_value");
            languageLabel = FindDeep<TMP_Text>("ui_text_menu_language_value");
            decorWheel = FindDeep<RectTransform>("ui_image_menu_wheel");
        }

        private void Awake()
        {
            soundOn = PlayerPrefs.GetInt(SoundPrefsKey, 1) == 1;
            ApplySound();

            playButton.onClick.AddListener(Hide);
            soundButton.onClick.AddListener(ToggleSound);
            languageButton.onClick.AddListener(ToggleLanguage);
        }

        private void OnEnable()
        {
            Loc.Changed += RefreshLabels;
            RefreshLabels();

            decorWheel.DORotate(new Vector3(0f, 0f, -360f), 24f, RotateMode.FastBeyond360)
                      .SetLoops(-1, LoopType.Incremental)
                      .SetEase(Ease.Linear);

            if (musicSource != null)
            {
                musicSource.volume = 0.4f;
                musicSource.Play();
            }
        }

        private void OnDisable()
        {
            Loc.Changed -= RefreshLabels;
            decorWheel.DOKill();
        }

        private void Hide()
        {
            if (musicSource != null)
                musicSource.DOFade(0f, 0.7f).OnComplete(musicSource.Stop);
            gameObject.SetActive(false);
        }

        private void ToggleSound()
        {
            soundOn = !soundOn;
            PlayerPrefs.SetInt(SoundPrefsKey, soundOn ? 1 : 0);
            PlayerPrefs.Save();
            ApplySound();
            RefreshLabels();
        }

        private void ToggleLanguage() => Loc.Toggle();

        private void ApplySound() => AudioListener.volume = soundOn ? 1f : 0f;

        private void RefreshLabels()
        {
            soundLabel.text = Loc.Get(soundOn ? "menu.sound_on" : "menu.sound_off");
            languageLabel.text = Loc.Current == GameLanguage.English ? "EN" : "TR";
        }
    }
}
