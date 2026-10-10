using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundsController : MonoBehaviour
{
    [Header("Реплики мышей")]
    public AudioClip[] mouseChatterClips;

    [Header("Интервал между репликами (сек)")]
    public float minInterval = 3f;
    public float maxInterval = 8f;

    [Header("Сказка")]
    public AudioClip taleClip;

    AudioSource audioSource;
    AudioSource taleSource;
    Coroutine chatterRoutine;
    float lastTaleTime;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        taleSource = gameObject.AddComponent<AudioSource>();
        taleSource.playOnAwake = false;
        taleSource.spatialBlend = 0f;
        taleSource.loop = false;
    }

    public void StartMouseChatter()
    {
        if (mouseChatterClips == null || mouseChatterClips.Length == 0)
            return;

        if (chatterRoutine != null)
            return;

        chatterRoutine = StartCoroutine(MouseChatterRoutine());
    }

    public void StopMouseChatter()
    {
        if (chatterRoutine != null)
        {
            StopCoroutine(chatterRoutine);
            chatterRoutine = null;
        }

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    IEnumerator MouseChatterRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

        while (true)
        {
            PlayRandomChatter();

            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
        }
    }

    void PlayRandomChatter()
    {
        if (mouseChatterClips == null || mouseChatterClips.Length == 0)
            return;

        AudioClip clip = mouseChatterClips[Random.Range(0, mouseChatterClips.Length)];
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void PlayTale()
    {
        if (taleClip == null || taleSource == null)
            return;

        lastTaleTime = 0f;
        taleSource.clip = taleClip;
        taleSource.time = 0f;
        taleSource.Play();
    }

    public void PauseTale()
    {
        if (taleSource == null || !taleSource.isPlaying)
            return;

        lastTaleTime = taleSource.time;
        taleSource.Pause();
    }

    public void ResumeTale()
    {
        if (taleSource == null || taleClip == null)
            return;

        if (taleSource.clip != taleClip)
            taleSource.clip = taleClip;

        if (IsTaleFinished)
        {
            PlayTale();
            return;
        }

        if (!taleSource.isPlaying)
        {
            taleSource.UnPause();
            if (!taleSource.isPlaying)
            {
                taleSource.time = lastTaleTime;
                taleSource.Play();
            }
        }
    }

    public void StopTale()
    {
        if (taleSource == null)
            return;

        taleSource.Stop();
        lastTaleTime = 0f;
    }

    void Update()
    {
        if (taleSource != null && taleSource.isPlaying)
            lastTaleTime = taleSource.time;
    }

    public float TaleProgress
    {
        get
        {
            if (taleClip == null || taleClip.length <= 0f)
                return 0f;

            float time = taleSource != null && taleSource.isPlaying ? taleSource.time : lastTaleTime;
            return Mathf.Clamp01(time / taleClip.length);
        }
    }

    public bool IsTalePlaying => taleSource != null && taleSource.isPlaying;

    public bool IsTaleFinished
    {
        get
        {
            if (taleSource == null || taleClip == null || taleClip.length <= 0f)
                return false;

            return !taleSource.isPlaying && taleSource.time >= taleClip.length - 0.05f;
        }
    }

    void OnDisable()
    {
        StopMouseChatter();
        StopTale();
    }
}
