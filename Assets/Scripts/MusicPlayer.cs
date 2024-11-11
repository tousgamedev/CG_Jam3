using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> songs = new List<AudioClip>();
    private AudioSource audioSource;
    private int currentSongIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false; // Ensure songs don’t loop individually
    }

    private void Start()
    {
        if (songs.Count > 0)
        {
            PlaySong(currentSongIndex);
        }
        else
        {
            Debug.LogWarning("No songs in the playlist!");
        }
    }

    private void Update()
    {
        // Check if the current song has finished playing
        if (!audioSource.isPlaying && songs.Count > 0)
        {
            PlayNextSong();
        }
    }

    private void PlaySong(int index)
    {
        if (index < 0 || index >= songs.Count) return;

        audioSource.clip = songs[index];
        audioSource.Play();
    }

    public void PlayNextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % songs.Count;
        PlaySong(currentSongIndex);
    }

    public void PlayPreviousSong()
    {
        currentSongIndex = (currentSongIndex - 1 + songs.Count) % songs.Count;
        PlaySong(currentSongIndex);
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}