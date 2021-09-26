using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource[] audioSources = null;

    private void Awake()
    {
        audioSources = GetComponents<AudioSource>();
    }

    private void PlaySound(string name, bool checkIfPlaying)
    {
        AudioSource ac = audioSources.FirstOrDefault(a => a.clip.name == name);
        if (ac == null)
        {
            Debug.LogError($"cannot find audio '{name}'");
            return;
        }
        if (checkIfPlaying && ac.isPlaying)
        {
            return;
        }
        ac.Play();
    }

    internal void PlaySFX(string name)
    {
        if (Settings.MuteSFX)
        {
            return;
        }
        PlaySound(name, false);
    }

    internal void PlayMusic(string name)
    {
        if (Settings.MuteMusic)
        {
            return;
        }
        PlaySound(name, true);
    }

    internal void Stop(string name)
    {
        AudioSource ac = audioSources.FirstOrDefault(a => a.clip.name == name);
        if (ac == null)
        {
            Debug.LogError($"cannot find audio '{name}'");
            return;
        }
        if (ac.isPlaying)
        {
            ac.Stop();
        }
    }
}