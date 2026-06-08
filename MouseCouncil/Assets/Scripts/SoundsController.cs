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

    AudioSource audioSource;
    Coroutine chatterRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
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

    void OnDisable()
    {
        StopMouseChatter();
    }
}
