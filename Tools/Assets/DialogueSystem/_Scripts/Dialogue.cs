/*
 * Author: Jeff Harper @jeffdevsitall
 * Inspired and modified from Brackeys - https://youtu.be/_nRzoTzeyxU 
 */

using System.Collections.Generic;
using UnityEngine;

// this allows for us to right click in the project window and create this scriptable object
[CreateAssetMenu(fileName = "Dialogue", menuName = "Variables/Dialogue")]
public class Dialogue : ScriptableObject
{
    // NPC Name, whoever player is talking to
    public string Name;

    // lines of dialogue
    public List<Sentence> sentences;

    // this is a txt file that can be brought into Unity
    public TextAsset textAsset;
    
    // whether we want to pull the lines from the text asset
    public bool useTextAsset;

    public void GetLines()
    {
        // make sure that the textAsset is not null
        // make sure that we want to get the lines from the text asset (this will erase all lines currently written)
        if (useTextAsset && textAsset)
        {
            // get the name from the title of the text asset (TODO: Needs some thinking about, you may not want the name to match)
            Name = textAsset.name;

            sentences.Clear();
            string[] lines = textAsset.text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 3)
                {
                    // for testing, making sure unity reads the lines in right
                    Debug.Log(lines[i]);
                    // create a new struct of type Sentence
                    Sentence sentence = new Sentence();
                    // add the line
                    sentence.sentence = lines[i];
                    // add sentence to the list of sentences
                    sentences.Add(sentence);
                }
                else
                {
                    // make sure that the line is not blank
                    Debug.LogWarning("Dialogue must be at least 3 characters long!");
                }
            }
        }
        else
        {
            // should never get to this point because of checks in custom editor
            Debug.LogWarning("Make sure that you add a TextAsset Variable and checked the Use TextAsset Toggle.");
        }
    }
}

[System.Serializable]
public struct Sentence
{
    // text fields for typing in our sentence (this is heavy modified by out custom editor)
    [TextArea(3,10)]
    public string sentence;

    // this is for our custom editor, it allows for us to easily collapse and expand a certain line
    public bool expand;
}
