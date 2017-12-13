using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Comp3490Project
{
    public class SettingsManager : MonoBehaviour
    {
        public AudioMixer Mixer;

        public Slider MasterVolSlider;
        public Slider MusicVolSlider;
        public Slider EffectsVolSlider;
        public Slider MenuVolSlider;

        public InputField MaxAsteroidsField;
        public InputField AsteroidGeneratorThreadCountField;

        private void Awake()
        {
            MasterVolSlider.onValueChanged.AddListener(UpdateMasterVolume);
            MusicVolSlider.onValueChanged.AddListener(UpdateMusicVolume);
            EffectsVolSlider.onValueChanged.AddListener(UpdateEffectsVolume);
            MenuVolSlider.onValueChanged.AddListener(UpdateMenuVolume);

            MaxAsteroidsField.onEndEdit.AddListener(UpdateMaxAsteroids);
            AsteroidGeneratorThreadCountField.onEndEdit.AddListener(UpdateGeneratorThreads);

            ApplySettings();
        }

        public void ApplySettings()
        {
            MasterVolSlider.value = Config.GlobalConfig.MasterVolume;
            Mixer.SetFloat("MasterVol", Config.GlobalConfig.MasterVolume);

            MusicVolSlider.value = Config.GlobalConfig.MusicVolume;
            Mixer.SetFloat("MusicVol", Config.GlobalConfig.MusicVolume);

            EffectsVolSlider.value = Config.GlobalConfig.EffectsVolume;
            Mixer.SetFloat("EffectsVol", Config.GlobalConfig.EffectsVolume);

            MenuVolSlider.value = Config.GlobalConfig.MenuVolume;
            Mixer.SetFloat("MenuVol", Config.GlobalConfig.MenuVolume);

            MaxAsteroidsField.text = Config.GlobalConfig.MaxAsteroids.ToString();
            AsteroidGeneratorThreadCountField.text = Config.GlobalConfig.AsteroidGeneratorThreadCount.ToString();
        }

        private void UpdateMasterVolume(float newVolume)
        {
            Mixer.SetFloat("MasterVol", newVolume);
            Config.GlobalConfig.MasterVolume = newVolume;
            Config.GlobalConfig.Save();
        }

        private void UpdateMusicVolume(float newVolume)
        {
            Mixer.SetFloat("MusicVol", newVolume);
            Config.GlobalConfig.MusicVolume = newVolume;
            Config.GlobalConfig.Save();
        }

        private void UpdateEffectsVolume(float newVolume)
        {
            Mixer.SetFloat("EffectsVol", newVolume);
            Config.GlobalConfig.EffectsVolume = newVolume;
            Config.GlobalConfig.Save();
        }

        private void UpdateMenuVolume(float newVolume)
        {
            Mixer.SetFloat("MenuVol", newVolume);
            Config.GlobalConfig.MenuVolume = newVolume;
            Config.GlobalConfig.Save();
        }

        private void UpdateMaxAsteroids(string amount)
        {
            int maxAsteroids;

            if (int.TryParse(amount, out maxAsteroids))
            {
                if (maxAsteroids < 0)
                {
                    maxAsteroids = 0;
                }

                MaxAsteroidsField.text = maxAsteroids.ToString();
                Config.GlobalConfig.MaxAsteroids = maxAsteroids;
                Config.GlobalConfig.Save();
            }
        }

        private void UpdateGeneratorThreads(string amount)
        {
            int threads;

            if (int.TryParse(amount, out threads))
            {
                if (threads < 1)
                {
                    threads = 1;
                }

                AsteroidGeneratorThreadCountField.text = threads.ToString();
                Config.GlobalConfig.AsteroidGeneratorThreadCount = threads;
                Config.GlobalConfig.Save();
            }
        }
    }
}
