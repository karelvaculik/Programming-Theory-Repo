using System;
using UnityEngine;

// INHERITANCE
public class ArmorCar : Car
{
    [Header("SFX")] [SerializeField]
    private AudioSource audioSource; // Assign your AudioSource that has the wav clip

    [SerializeField] private bool playPartial = true;
    [SerializeField, Min(0f)] private float clipStartSeconds = 2.3f;
    [SerializeField, Min(0f)] private float clipEndSeconds = 4.1f;

    private void Awake()
    {
        // Use the attached AudioSource if not manually assigned
        if (audioSource == null)
        {
            TryGetComponent(out audioSource);
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    // POLYMORPHISM
    protected override void Interact()
    {
        PlaySound();
    }

    // ABSTRACTION
    private void PlaySound()
    {
        if (audioSource == null || audioSource.clip == null) return;

        float clipLen = audioSource.clip.length;

        // Clamp and validate times
        float start = Mathf.Clamp(clipStartSeconds, 0f, clipLen);
        float end = Mathf.Clamp(clipEndSeconds, 0f, clipLen);

        // Always stop before re-triggering
        audioSource.Stop();
        audioSource.loop = false;

        // If not playing partial or invalid range, just play from 'start'
        if (!playPartial || end <= start)
        {
            audioSource.time = start;
            audioSource.Play();
            return;
        }

        // Schedule precise segment playback [start, end) using DSP scheduling
        audioSource.time = start;
        double dspStart = AudioSettings.dspTime + 0.02; // small safety offset
        audioSource.PlayScheduled(dspStart);
        audioSource.SetScheduledEndTime(dspStart + (end - start));
    }
}