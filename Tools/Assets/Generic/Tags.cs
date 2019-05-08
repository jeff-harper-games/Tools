/*

    Author: Jeff Harper
    Twitter: @jeffdevsitall
    Website: jeffharper.games
 
    Copyright (c) 2019 by Jeff Harper

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif 

[System.Serializable]
public class TagList
{
    public string[] Tags;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TagList))]
public class TagsDrawer : PropertyDrawer
{
    public TagList tags;
    public Object _object;
    private List<string> tempTags = new List<string>();
    private SerializedProperty tagsProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _object = property.serializedObject.targetObject;

        EditorGUI.BeginProperty(position, label, property);

        tempTags = new List<string>();

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        tagsProp = property.FindPropertyRelative("Tags");

        if (tagsProp.isArray)
        {
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                tempTags.Add(tagsProp.GetArrayElementAtIndex(i).stringValue);
            }
        }

        int tagCount = UnityEditorInternal.InternalEditorUtility.tags.Length;
        string str = tempTags.Count > 1 && tempTags.Count != tagCount ? "Mixed..." : tempTags.Count == 1
            ? tempTags[0] : tempTags.Count == tagCount ? "Everything" : "Nothing";

        GUIStyle style = new GUIStyle("button");
        style.alignment = TextAnchor.MiddleLeft;

        if (GUI.Button(position, str, style))
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                string tag = UnityEditorInternal.InternalEditorUtility.tags[i];
                bool on = false;
                if (tempTags.Contains(tag))
                    on = true;
                AddMenuItem(menu, on, tag, tag);
            }
            menu.ShowAsContext();
        }

        EditorGUI.EndProperty();
    }

    void AddMenuItem(GenericMenu menu, bool on, string menuPath, string tag)
    {
        menu.AddItem(new GUIContent(menuPath), on, OnSelect, tag);
    }

    void OnSelect(object tag)
    {
        string str = (string)tag;

        if (tempTags.Contains(str))
            tempTags.Remove(str);
        else
            tempTags.Add(str);

        tagsProp.ClearArray();
        for (int i = 0; i < tempTags.Count; i++)
        {
            tagsProp.InsertArrayElementAtIndex(i);
            SerializedProperty index = tagsProp.GetArrayElementAtIndex(i);
            index.stringValue = tempTags[i];
        }
        
        tagsProp.serializedObject.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(_object);
    }
}
#endif 
