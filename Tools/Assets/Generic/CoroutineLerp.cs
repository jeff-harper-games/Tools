using System.Collections;
using UnityEngine;

public class CoroutineLerp
{
    private Coroutine coroutine;
    public delegate void MarkerDelegate(bool b);
    public MarkerDelegate marker;
    public delegate void ProgressDelegate(float f);
    public ProgressDelegate progress;

    public void Begin(float start, float end, float duration, MonoBehaviour mono)
    {
        if(coroutine != null)
            mono.StopCoroutine(coroutine);
        coroutine = mono.StartCoroutine(Perform(start, end, duration));
    }

    public void Stop(MonoBehaviour mono)
    {
        if (coroutine != null)
            mono.StopCoroutine(coroutine);
    }

    public IEnumerator Perform(float start, float end, float duration)
    {
        if (marker != null)
            marker.Invoke(true);
        if (progress != null)
            progress.Invoke(start);
        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            if (progress != null)
                progress.Invoke(Mathf.Lerp(start, end, t));
            yield return null;
        }
        if(progress != null)
            progress.Invoke(end);
        if (marker != null)
            marker.Invoke(false);
    }
}