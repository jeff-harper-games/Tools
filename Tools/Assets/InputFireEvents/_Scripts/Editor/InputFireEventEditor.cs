/*
 * Author: Jeff Harper @jeffdevsitall
 */

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(InputFireEvents))]
public class InputFireEventEditor : Editor
{
    private ReorderableList list;
    private string inputEvents = "inputEvents";
    private string Name = "Name";
    private string keyInput = "keyInput";
    private string display = "display";
    private string keyEvent = "keyEvent"; 

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty(inputEvents), true, true, true, true);
        float lineHeight = EditorGUIUtility.singleLineHeight + 5;

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            GUIStyle myStyle = new GUIStyle();
            element.FindPropertyRelative(Name).stringValue = element.FindPropertyRelative(keyInput).enumNames[element.FindPropertyRelative(keyInput).enumValueIndex];
            Vector2 sizeOfLabel = myStyle.CalcSize(new GUIContent(element.FindPropertyRelative(Name).stringValue + "  "));

            element.FindPropertyRelative(display).boolValue = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y + 2, sizeOfLabel.x + 30, lineHeight), 
                element.FindPropertyRelative(display).boolValue, element.FindPropertyRelative(Name).stringValue);

            if (element.FindPropertyRelative(display).boolValue)
            {
                EditorGUI.LabelField(new Rect(rect.x + 10, rect.y + lineHeight, 60, lineHeight), "KeyCode: ");
                EditorGUI.PropertyField(new Rect(rect.x + 75, rect.y + lineHeight, 120, lineHeight), element.FindPropertyRelative(keyInput), GUIContent.none, true);
                EditorGUI.LabelField(new Rect(rect.x + 10, rect.y + lineHeight, 60, lineHeight), "KeyCode: ");  
                EditorGUI.PropertyField(new Rect(rect.x + 10, rect.y + (lineHeight * 2), rect.width - 30, lineHeight), element.FindPropertyRelative(keyEvent), GUIContent.none, true);
            }
        };

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Inputs");
        };

        list.elementHeightCallback = (int index) =>
        {

            float height = lineHeight;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            
            if (element.FindPropertyRelative(display).boolValue)
            {
                height += EditorGUI.GetPropertyHeight(element.FindPropertyRelative(keyEvent)) + 40;
            }
            
            return height;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
