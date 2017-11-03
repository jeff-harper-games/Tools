/*
 * Author: Jeff Harper @jeffdevsitall 
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputFireEvents : MonoBehaviour
{
    public List<InputEvents> inputEvents;
    public bool debug;

	void Update ()
    {
        for (int i = 0; i < inputEvents.Count; i++)
        {
            if (Input.GetKeyDown(inputEvents[i].keyInput))
            {
                inputEvents[i].keyEvent.Invoke();
            }
        }
	}
}

[System.Serializable]
public class InputEvents
{
    public KeyCode keyInput;
    public UnityEvent keyEvent;
    public string Name;
    public bool display;
}
