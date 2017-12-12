using System.Collections;
using UnityEngine;

namespace Comp3490Project
{
    public class MusicManager : MonoBehaviour
    {
        public AudioSource AudioPlayer;
        public AudioClip[] Clips;

        private static bool initialized = false;

        private void Awake()
        {
            if(!initialized)
            {
                DontDestroyOnLoad(gameObject);
                initialized = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (Clips.Length > 0)
            {
                StartCoroutine(PlayMusic());
            }
        }

        private IEnumerator PlayMusic()
        {
            int clipIndex = 0;
            int prevClipIndex = clipIndex;
            
            while(true)
            {
                while (clipIndex == prevClipIndex)
                {
                    clipIndex = Random.Range(0, Clips.Length);
                }
                prevClipIndex = clipIndex;

                AudioPlayer.clip = Clips[clipIndex];
                AudioPlayer.Play();
                yield return new WaitForSeconds(AudioPlayer.clip.length);
                AudioPlayer.Stop();
            }
        }
    }
}
