using System.Collections;
using UnityEngine;


public class StoveSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource disposableSource;  // source for playin non-looped sounds
    [SerializeField] private AudioSource loopSource;        // source for looped gas flowing sound
    [SerializeField] private AudioClip ignitionClip;        // burner ignition
    [SerializeField] private AudioClip ignitionBrokenClip;  // broken burner ignition
    [SerializeField] private AudioClip gasClip;             // flowing gas
    [SerializeField] private AudioClip burningClip;         // burning gas
    float ignitionLength;


    private void Awake()
    {
        ignitionLength = ignitionClip.length;
    }

    public void PlayGas()
    {
        loopSource.clip = gasClip;
        loopSource.Play();
        Debug.Log($"Loop with sound '{loopSource.clip}' started.");
    }

    public void PlayBurning(bool isFirstBurner)
    {
        disposableSource.PlayOneShot(ignitionClip);

        if (isFirstBurner)
        {
            StartCoroutine(IgnitionDelay());
            loopSource.clip = burningClip;
            loopSource.Play();
            Debug.Log($"Loop with sound '{loopSource.clip}' started.");
        }
    }

    private IEnumerator IgnitionDelay()
    {
        yield return new WaitForSeconds(ignitionLength);
    }

    public void PlayBrokenIgnition()
    {
        disposableSource.PlayOneShot(ignitionBrokenClip);
    }

    public void StopLoop()
    {
        Debug.LogWarning($"Loop with sound '{loopSource.clip}' stopped.");
        loopSource.Stop();
    }
}
