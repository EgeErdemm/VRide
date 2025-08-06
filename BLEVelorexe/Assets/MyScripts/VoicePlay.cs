using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicePlay : MonoBehaviour
{
    private AudioManager m_AudioManager;
    public int playAbleCount;
    public int startIndex;
    public bool isPlaying = false;

    private void Start()
    {
        m_AudioManager = AudioManager.Instance;

        if (m_AudioManager == null)
        {
            Debug.LogError("AudioManager.Instance is null! Make sure AudioManager exists in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bicycle>())
        {
            Bicycle bicycle = other.GetComponent<Bicycle>();
            //m_AudioManager.PlaySound(bicycle.voiceCount);
            //bicycle.voiceCount++;
            StartCoroutine(PlayVoicesSequentially(startIndex));


        }
    }

    private IEnumerator PlayVoicesSequentially(int startIndex)
    {
        isPlaying = true;

        for (int i = 0; i < playAbleCount; i++)
        {
            int currentIndex = startIndex + i;
            if (currentIndex >= m_AudioManager.audioClips.Count)
            {
                Debug.LogWarning("Audio clip index out of bounds: " + currentIndex);
                break;
            }
            AudioClip clip = m_AudioManager.audioClips[currentIndex];
            m_AudioManager.PlaySound(currentIndex);
            yield return new WaitForSeconds(clip.length);
        }
        isPlaying = false;
    }



}
