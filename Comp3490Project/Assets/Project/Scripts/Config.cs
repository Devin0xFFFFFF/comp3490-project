using UnityEngine;

namespace Comp3490Project
{
    public class Config
    {
        public float MasterVolume = 0.0f;
        public float MusicVolume = 0.0f;
        public float EffectsVolume = 0.0f;
        public float MenuVolume = 0.0f;

        public int MaxAsteroids = 1000;
        public int AsteroidGeneratorThreadCount = 4;

        public static Config GlobalConfig
        {
            get
            {
                if (globalConfig == null)
                {
                    globalConfig = new Config();
                }
                return globalConfig;
            }
        }

        private static Config globalConfig;

        public Config()
        {
            Load();
        }

        public void Save()
        {
            PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
            PlayerPrefs.SetFloat("EffectsVolume", EffectsVolume);
            PlayerPrefs.SetFloat("MenuVolume", MenuVolume);

            PlayerPrefs.SetInt("MaxAsteroids", MaxAsteroids);
            PlayerPrefs.SetInt("AsteroidGeneratorThreadCount", AsteroidGeneratorThreadCount);
        }

        public void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.0f);
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.0f);
            EffectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 0.0f);
            MenuVolume = PlayerPrefs.GetFloat("MenuVolume", 0.0f);

            MaxAsteroids = PlayerPrefs.GetInt("MaxAsteroids", 1000);
            AsteroidGeneratorThreadCount = PlayerPrefs.GetInt("AsteroidGeneratorThreadCount", 4);
        }
    }
}
