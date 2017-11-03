/*
 * Author: Jeff Harper @jeffdevsitall
 * Inspired and modified from Brackeys - https://youtu.be/_nRzoTzeyxU 
 */

using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Event", menuName = "Variables/Event/Dialogue Event")]
public class CustomDialogueEvent : ScriptableObject
{
    // delegate that takes in a type Dialogue
    public delegate void InvokeAction(Dialogue dialogue);
    // event of the delegate
    public event InvokeAction OnInvoke;

    public void Invoke(Dialogue _dialogue)
    {
        // make sure that the event is not empty
        if (OnInvoke != null)
        {
            Debug.Log("Custom Dialogue Event for " + _dialogue.Name + " was Invoked.");
            // invoke event
            OnInvoke.Invoke(_dialogue);
        }
        else
        {
            Debug.Log("Event is empty... Therefore nothing happened.");
        }
    }
}
