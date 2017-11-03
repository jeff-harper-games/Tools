/*
 * Author: Jeff Harper @jeffdevsitall
 * Inspired and modified from Brackeys - https://youtu.be/_nRzoTzeyxU 
 */

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // dialogue scriptable object
    [SerializeField]
    private Dialogue dialogue;
    // dialogue event scriptable object
    [SerializeField]
    private CustomDialogueEvent beginDialogueEvent;

    public void TriggerDialogue()
    {
        // invoke the dialogue
        beginDialogueEvent.Invoke(dialogue);
    }

    // for testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerDialogue();
        }
    }
}
