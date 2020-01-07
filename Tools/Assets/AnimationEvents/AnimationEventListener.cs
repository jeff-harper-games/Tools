using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    public delegate void Reached();
    public Reached OnReached;

    private void FrameReached()
    {
        if (OnReached != null)
            OnReached.Invoke();
    }
}