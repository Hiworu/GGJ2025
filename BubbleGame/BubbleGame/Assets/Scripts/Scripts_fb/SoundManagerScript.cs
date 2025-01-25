using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private AudioSource audioSource;

    public static SoundManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Funzione per riprodurre un file audio in base al path
    public void PlayAudio(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("Audio clip not found at path: " + path);
        }
    }

}