/*
 * Author: Jeff Harper @jeffdevsitall
 * Inspired and modified from Brackeys - https://youtu.be/_nRzoTzeyxU 
 */

using UnityEngine;
using UnityEditor;
using UnityEditorInternal; // for ReorderableList

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor
{
    // a very dynamic list, use as reference (http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/)
    private ReorderableList list;

    private void OnEnable()
    {
        // setup a style to use for calculations
        GUIStyle style = new GUIStyle();

        // create the list, and reference the "sentences"
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("sentences"), true, true, true, true);
        // create an array for the heights for each element in the list (allows for different size per element)
        float[] height = new float[list.count];
        // reference the default line height of the editor
        float lineHeight = EditorGUIUtility.singleLineHeight;

        // what to draw from the list
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            // get property of the element at the index
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            
            // get the expand property, so we can collapse or expand
            SerializedProperty property = element.FindPropertyRelative("expand");

            // create a foldout for collapsing and expanding
            property.boolValue = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y + 2, 60, lineHeight), property.boolValue, "Line " + (index + 1));

            if (element.FindPropertyRelative("expand").boolValue)
            {
                // calculate the height of the sentence for the text area
                height[index] = style.CalcHeight(new GUIContent(element.FindPropertyRelative("sentence").stringValue), rect.width) + lineHeight + 10;
                // create rect to use to display
                Rect textAreaRect = new Rect(rect.x, rect.y + 10, rect.width, height[index]);
                // draw the property field
                EditorGUI.PropertyField(textAreaRect, element.FindPropertyRelative("sentence"), GUIContent.none);
            }
        };

        // height for each element height
        list.elementHeightCallback = (int index) =>
        {
            float h = 0;
            // index element
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            // make sure that there is at least one element in the list 
            if (height.Length > 0)
            {
                // if expanded, add the height of the text to the height of the element
                if (element.FindPropertyRelative("expand").boolValue)
                    h = height[index] + 25;
                else
                    h = 25;
            }

            return h;
        };

        // when you add an element to the list
        list.onAddCallback = (ReorderableList l) => {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            // increase the height array
            height = new float[index + 1];
        };

        // display name of the list
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Dialogue Tree");
        };
    }

    public override void OnInspectorGUI()
    {
        // watch for changes in inspector
        EditorGUI.BeginChangeCheck();

        //base.OnInspectorGUI();

        // get a reference to the dialogue script
        Dialogue myScript = (Dialogue)target;

        // field for the name
        myScript.Name = EditorGUILayout.TextField("Name: ", myScript.Name);

        // toggle for if using a text asset for lines
        myScript.useTextAsset = EditorGUILayout.ToggleLeft("Get Lines from Text Asset", myScript.useTextAsset);

        // do the follow is useTextAsset is true
        if (myScript.useTextAsset)
        {
            // start a horizontal row of elements
            EditorGUILayout.BeginHorizontal();

            // get the property for the text asset
            SerializedProperty property = serializedObject.FindProperty("textAsset");

            // draw the object field
            EditorGUILayout.ObjectField(property, new GUIContent("Text Asset: "));
            
            // if the textAsset is not null
            if (myScript.textAsset)
            {
                // get line button, that will be used to call GetLines()
                if (GUILayout.Button("Get Lines"))
                {
                    // message to display after the GetLines button is pressed
                    string message = "Are you sure you want to replace the lines with those of the TextAsset?";

                    // check the user to make sure they want to replace the lines
                    if (EditorUtility.DisplayDialog("Get Lines?", message, "Yes", "No"))
                    {
                        // call the GetLines()
                        myScript.GetLines();
                    }
                }
            }

            // end the horizontal row
            EditorGUILayout.EndHorizontal();
        }

        // draw the reorderable list
        list.DoLayoutList();

        // check if changes were made
        if (EditorGUI.EndChangeCheck())
        {
            // apply changes
            serializedObject.ApplyModifiedProperties();
            // set the scriptable object dirty so that it will retain changes when project is exited
            EditorUtility.SetDirty(target);
        }

        // update the script
        serializedObject.Update();
    }
}
