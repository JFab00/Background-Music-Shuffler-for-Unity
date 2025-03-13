
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMusicShuffle : MonoBehaviour
{
    [Header("Main")]
    [Tooltip("Add the tracks you want to get randomized")]
    public List<AudioClip> tracks;
    [Tooltip("Made [SerializedField] for Debugging purposes. Please let it Empty!")]
    [SerializeField] private List<AudioClip> shuffleCotnainer;
    [Tooltip("AudioSource of your game")]
    public AudioSource audioSource;

    [Header("Fade")]
    [Tooltip("Will the Audios have Fade In and Fade Out?")]
    public bool isFading = true;
    [Tooltip("The duration (in seconds) for the fade")]
    public float fadeDuration = 2f;
    [Tooltip("MaxVolume of the Audio Source")]
    [Range(0.0f, 1.0f)]
    public float maxVolume = 1f;

    private int shuffler = 0;

    public static BackgroundMusicShuffle instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource.volume = 0f;
    }

    void Update()
    {
        if (!audioSource.isPlaying)
            Shuffle();
    }

    private void Shuffle()
    {
        shuffler = Random.Range(0, tracks.Count);
        audioSource.clip = tracks.ElementAt(shuffler);
        audioSource.Play();
        if (isFading)
        {
            StartCoroutine(Fade(true, audioSource, fadeDuration, maxVolume));
            StartCoroutine(Fade(false, audioSource, fadeDuration, 0f));
        }
        shuffleCotnainer.Add(tracks.ElementAt(shuffler));
        tracks.RemoveAt(shuffler);
        if (tracks.Count == 0)
        {
            tracks = new List<AudioClip>(shuffleCotnainer);
            shuffleCotnainer.Clear();
            shuffler = 0;
        }
    }

    // https://www.youtube.com/watch?v=kYGXGDjL5jM   --->  for explanation
    public IEnumerator Fade(bool fadeIn, AudioSource source, float duration, float targetVolume)
    {
        if (!fadeIn)
        {
            // Debug.Log("!fadeIN" + (double)source.clip.samples + " | " + source.clip.samples + " | " + source.clip.frequency);
            double lengthOfSource = (double)source.clip.samples / source.clip.frequency;
            yield return new WaitForSecondsRealtime((float)(lengthOfSource - duration));
        }

        float time = 0f;
        float startVol = source.volume;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }

        yield break;

    }
}
