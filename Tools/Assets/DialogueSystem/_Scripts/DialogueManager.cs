/*
 * Author: Jeff Harper @jeffdevsitall
 * Inspired and modified from Brackeys - https://youtu.be/_nRzoTzeyxU 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // canvas dialogue text objects
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text dialogueText;
    // buttons
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button previousButton;
    [SerializeField]
    private Button endDialogueButton;
    // scriptable object dialogue event
    [SerializeField]
    private CustomDialogueEvent beginDialogueEvent;
    // menu animator
    [SerializeField]
    private Animator animator;
    // check for pausing during dialogue
    [SerializeField]
    private bool pauseDuringDialogue;

    // list of sentences from the dialogue
    private List<string> sentences = new List<string>();
    // IEnumerator that is currently typing a sentence 
    // (we store this, so that if you skip before it is done)
    private Coroutine typing;
    // int to determine what sentence you are on
    private int count;
    // float used to determine the delay of the start of the dialogue 
    [SerializeField]
    private float timeDuration = 1.0f;
    // for delaying the dialogue, we could link in the start animation here, but using a float allows a little more flexablity
    private WaitForSecondsRealtime delayDialogue;

    private void OnEnable()
    {
        // add StartDialogue function to the custom event
        beginDialogueEvent.OnInvoke += StartDialogue;
        // add founctions to the button events (could be done in inspector instead)
        nextButton.onClick.AddListener(DisplayNextSentence);
        previousButton.onClick.AddListener(DisplayPreviousSentence);
        endDialogueButton.onClick.AddListener(EndDialogue);
    }

    private void OnDisable()
    {
        // remove StartDialogue function to the custom event
        beginDialogueEvent.OnInvoke -= StartDialogue;
        // remove founctions to the button events
        nextButton.onClick.RemoveListener(DisplayNextSentence);
        previousButton.onClick.RemoveListener(DisplayPreviousSentence);
        endDialogueButton.onClick.RemoveListener(EndDialogue);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // play the start dialogue animation
        if(animator)
            animator.SetBool("IsOpen", true);

        // pause the game
        if(pauseDuringDialogue)
            Time.timeScale = 0;

        // display the name of the Dialogue
        nameText.text = dialogue.Name;

        // clear the dialogue text
        dialogueText.text = "";

        // clear the sentences for the new set
        sentences.Clear();
        // set the count to the first sentence
        count = 0;

        // toggle on and off buttons
        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);
        endDialogueButton.gameObject.SetActive(false);

        // run through the list of the sentence sent from the dialogue
        for (int i = 0; i < dialogue.sentences.Count; i++)
        {
            sentences.Add(dialogue.sentences[i].sentence);
        }

        // start the delay for the first sentence
        StartCoroutine(DelayFirstSentence());
    }

    IEnumerator DelayFirstSentence()
    {
        // delay the start of typing depending on the timeDuration
        yield return new WaitForSecondsRealtime(timeDuration);

        // start the typing of the first sentence
        typing = StartCoroutine(TypeSentence(sentences[count]));

        // enable next button
        nextButton.gameObject.SetActive(true);
    }

    public void DisplayNextSentence()
    {
        // increment the count for the next sentence
        count++;

        // if count is not the first sentence, allow the player to go back
        if (count > 0)
            previousButton.gameObject.SetActive(true);

        // if count is the last sentence, disable the next button and turn on the end button
        if (count == sentences.Count - 1)
        {
            endDialogueButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }

        // get the current sentence
        string sentence = sentences[count];

        // if a sentence is currently typing, stop
        if (typing != null)
        {
            StopCoroutine(typing);
        }

        // start next sentence
        typing = StartCoroutine(TypeSentence(sentence));
    }

    public void DisplayPreviousSentence()
    {
        // decrement the count
        count--;

        // turn off the end conversation button and on the next button
        endDialogueButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);

        // if count is first, turn off previous button
        if (count == 0)
        {
            previousButton.gameObject.SetActive(false);
        }

        // get the current sentence
        string sentence = sentences[count];

        // if a sentence is currently typing, stop
        if (typing != null)
        {
            StopCoroutine(typing);
        }

        // start next sentence
        typing = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        // clear the dialogue 
        dialogueText.text = "";

        // run through each char in the sentence and type it
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            // wait a frame
            yield return null;
        }
    }

    public void EndDialogue()
    {
        // unpause
        if(pauseDuringDialogue)
            Time.timeScale = 1;

        // play close dialogue animation
        if(animator)
            animator.SetBool("IsOpen", false);

        Debug.Log("End of conversation");
    }
}
