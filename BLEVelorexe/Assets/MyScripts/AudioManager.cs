using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;
    public List<AudioClip> audioClips; // Inspector�dan atars�n

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Belirli s�radaki sesi �al
    public void PlaySound(int index)
    {
        if (index >= 0 && index < audioClips.Count)
        {
            audioSource.PlayOneShot(audioClips[index]);
        }
        else
        {
            Debug.LogWarning("Audio index out of range!");
        }
    }

    // �sme g�re sesi �al
    public void PlaySound(string clipName)
    {
        AudioClip clip = audioClips.Find(c => c.name == clipName);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{clipName}' not found!");
        }
    }
}
