using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EventAtFrame : MonoBehaviour
{
    public AnimationClip clip;
    public Animator anim;
    public int frame;
    public bool removeAfterReached;
    public UnityEvent frameReachedEvent; 

    private AnimationEventListener frameEvent;
    private AnimationEvent evt;

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if (!anim)
        {
            Debug.Log("<color=red>No animator found.</color>", this);
            return;
        }

        frameEvent = anim.GetComponent<AnimationEventListener>();
        if(!frameEvent)
            frameEvent = anim.gameObject.AddComponent<AnimationEventListener>();

        evt = new AnimationEvent();
        evt.time = frame / clip.frameRate;
        evt.functionName = "FrameReached";
        clip.AddEvent(evt);

        frameEvent.OnReached += FrameReached;
    }

    private void FrameReached()
    {
        frameReachedEvent.Invoke();

        if (removeAfterReached)
        {
            List<AnimationEvent> events = new List<AnimationEvent>(clip.events);
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].time == evt.time && events[i].functionName == evt.functionName)
                {                    
                    events.RemoveAt(i);
                    break;
                }
            }
            clip.events = events.ToArray();
            Destroy(frameEvent);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EventAtFrame))]
public class EventAtFrameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EventAtFrame eaf = (EventAtFrame)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("anim"), true);

        if (eaf.anim)
        {
            GetClip();

            if (eaf.clip)
            {
                int maxFrame = Mathf.RoundToInt(eaf.clip.frameRate * eaf.clip.length);
                eaf.frame = EditorGUILayout.IntSlider("Frame", eaf.frame, 0, maxFrame);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("removeAfterReached"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameReachedEvent"), true);
        }
        else
            EditorGUILayout.HelpBox("You must have an Animator referenced.", MessageType.Error);

        serializedObject.ApplyModifiedProperties();
    }

    private void GetClip()
    {
        EventAtFrame eaf = (EventAtFrame)target;

        if (!eaf.anim)
            return;

        AnimationClip[] clips = eaf.anim.runtimeAnimatorController.animationClips;

        if (clips.Length <= 0)
            return;

        int index = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == eaf.clip)
                index = i;
        }

        string[] clipNames = new string[clips.Length];
        for (int i = 0; i < clips.Length; i++)
        {
            clipNames[i] = clips[i].name;
        }

        index = EditorGUILayout.Popup("Clip", index, clipNames);

        eaf.clip = clips[index];
    }
}
#endif