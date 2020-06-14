using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class DisableExitTime : Editor
{
    [MenuItem("Tools/Disable Exit Time on Animation Transition _F12")]
    public static void Disable()
    {
        Undo.RecordObjects(Selection.objects, "Disable Exit Time");
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            Object sel = Selection.objects[i];

            if (sel.GetType() == typeof(AnimatorStateTransition))
            {
                AnimatorStateTransition transition = (AnimatorStateTransition)sel;
                transition.hasExitTime = false;
                transition.exitTime = 1.0f;
                transition.hasFixedDuration = true;
                transition.duration = 0.0f;
            }
        }
    }
}
