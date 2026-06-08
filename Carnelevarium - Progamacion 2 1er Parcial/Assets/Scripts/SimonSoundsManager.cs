using System.Collections;
using UnityEngine;

public class SimonSoundsManager : MonoBehaviour
{
    private float[] frequencies = { 261.63f, 329.63f, 392.00f, 523.25f };

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayTone(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= frequencies.Length) return;
        PlayBeep(frequencies[buttonIndex], 0.35f);
    }

    public void PlayWin()
    {
        StartCoroutine(PlayMelody(
            new float[] { 523f, 659f, 784f, 1046f },
            new float[] { 0.12f, 0.12f, 0.12f, 0.4f }
        ));
    }

    public void PlayLose()
    {
        StartCoroutine(PlayMelody(
            new float[] { 300f, 200f },
            new float[] { 0.25f, 0.5f }
        ));
    }

    void PlayBeep(float frequency, float duration)
    {
        int sampleRate = AudioSettings.outputSampleRate;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Clamp01(1f - t / duration * 2f);
            data[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * 0.4f * envelope;
        }

        AudioClip clip = AudioClip.Create("beep", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
    }

    IEnumerator PlayMelody(float[] freqs, float[] durations)
    {
        for (int i = 0; i < freqs.Length; i++)
        {
            PlayBeep(freqs[i], durations[i]);
            yield return new WaitForSeconds(durations[i] + 0.05f);
        }
    }
}
