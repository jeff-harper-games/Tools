using System.Collections;
using UnityEngine;

public class CoroutineLerp
{
    private Coroutine coroutine;
    public delegate void MarkerDelegate(bool b);
    public MarkerDelegate marker;
    public float Progress; 

    public void Begin(float duration, MonoBehaviour mono)
    {
        if(coroutine != null)
            mono.StopCoroutine(coroutine);
        coroutine = mono.StartCoroutine(Perform(duration));
    }

    public void Stop(MonoBehaviour mono)
    {
        if (coroutine != null)
            mono.StopCoroutine(coroutine);
    }

    public IEnumerator Perform(float duration)
    {
        if (marker != null)
            marker.Invoke(true);

        Progress = 0.0f;

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            Progress = Mathf.Lerp(0.0f, 1.0f, t);
            yield return null;
        }

        Progress = 1.0f;

        if (marker != null)
            marker.Invoke(false);
    }
}