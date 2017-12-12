using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Comp3490Project
{
    public class MainMenu : MonoBehaviour
    {
        public SimCamera SimCam;

        public GameObject MainMenuPanel;
        public GameObject InstructionsPanel;

        public Button PlayButton;
        public Button InstructionsButton;
        public Button BackToMainMenuButton;
        public Button QuitButton;

        public AudioClip HoverSound;
        public AudioClip SelectSound;

        private AudioSource audioPlayer;

        private void Awake()
        {
            audioPlayer = GetComponent<AudioSource>();

            PlayButton.onClick.AddListener(Play);
            InstructionsButton.onClick.AddListener(ShowInstructions);
            BackToMainMenuButton.onClick.AddListener(ShowMainMenu);
            QuitButton.onClick.AddListener(Quit);

            MainMenuPanel.SetActive(true);
            InstructionsPanel.SetActive(false);
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

        private IEnumerator HideMenu()
        {
            yield return new WaitForSeconds(SelectSound.length);
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
        }

        private void Quit()
        {
            audioPlayer.PlayOneShot(SelectSound);

            Application.Quit();
        }
    }
}
