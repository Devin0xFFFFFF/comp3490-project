using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Comp3490Project
{
    public class MainMenu : MonoBehaviour
    {
        public SimCamera SimCam;

        public GameObject MainMenuPanel;
        public GameObject InstructionsPanel;
        public GameObject SettingsPanel;

        public SettingsManager Settings;

        public Button PlayButton;
        public Button AsteroidDemoButton;
        public Button InstructionsButton;
        public Button SettingsButton;
        public Button InstructionsBackToMainMenuButton;
        public Button SettingsBackToMainMenuButton;
        public Button QuitButton;

        public AudioClip HoverSound;
        public AudioClip SelectSound;

        private AudioSource audioPlayer;

        private void Awake()
        {
            audioPlayer = GetComponent<AudioSource>();

            PlayButton.onClick.AddListener(Play);
            AsteroidDemoButton.onClick.AddListener(Demo);
            InstructionsButton.onClick.AddListener(ShowInstructions);
            SettingsButton.onClick.AddListener(ShowSettings);
            InstructionsBackToMainMenuButton.onClick.AddListener(ShowMainMenu);
            SettingsBackToMainMenuButton.onClick.AddListener(ShowMainMenu);
            QuitButton.onClick.AddListener(Quit);

            MainMenuPanel.SetActive(true);
            InstructionsPanel.SetActive(false);
            SettingsPanel.SetActive(false);
        }

        private void Start()
        {
            Settings.ApplySettings();
        }

        public void PlayHoverButtonSound()
        {
            audioPlayer.PlayOneShot(HoverSound);
        }

        private void Play()
        {
            audioPlayer.PlayOneShot(SelectSound);

            SimCam.SwitchToGameMode();

            StartCoroutine(HideMenu());
        }

        private void Demo()
        {
            SceneManager.LoadScene(2);
        }

        private IEnumerator HideMenu()
        {
            yield return new WaitForSeconds(SelectSound.length - 0.25f);
            gameObject.SetActive(false);
        }

        private void ShowInstructions()
        {
            audioPlayer.PlayOneShot(SelectSound);

            MainMenuPanel.SetActive(false);
            InstructionsPanel.SetActive(true);
        }

        private void ShowMainMenu()
        {
            audioPlayer.PlayOneShot(SelectSound);

            MainMenuPanel.SetActive(true);
            InstructionsPanel.SetActive(false);
            SettingsPanel.SetActive(false);
        }

        private void ShowSettings()
        {
            MainMenuPanel.SetActive(false);
            SettingsPanel.SetActive(true);
        }

        private void Quit()
        {
            audioPlayer.PlayOneShot(SelectSound);

            Application.Quit();
        }
    }
}
