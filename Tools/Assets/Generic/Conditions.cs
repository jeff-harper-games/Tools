using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class Conditions : MonoBehaviour
{
    public List<Condition> conditions;

    private void Start()
    {
        CheckConditions();
    }

    public bool CheckConditions()
    {
        bool meet = true;
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].type != Condition.Types.Invalid)
            {
                if (!conditions[i].Compare())
                    meet = false;
            }

            //conditions[i].GetSubNames();
        }
        Debug.Log(meet ? "<color=green>Conditions all met</color>" : "<color=red>Conditions not met</color>");
        return meet;
    }

    [System.Serializable]
    public class Condition
    {
        public Object target;
        public int index;
        public int subIndex = 0;
        public string varName;
        public string subVarName;
        public int comparer;
        //public object value;

        public enum Types { Invalid, Object, Int, Float, String, Bool, Vector3, Vector2}
        public Types type; 

        public Object objectArgument;
        public int intArgument;
        public float floatArgument;
        public string stringArgument;
        public bool boolArgument;
        public Vector3 vector3Argument; 
        public Vector2 vector2Argument; 

        private string[] shortComparer = new string[] { "Equals", "Not Equal" };
        private string[] longComparer = new string[] { "Equals", "Not Equal", "Greater", "Less" };

        public string[] GetNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < target.GetType().GetFields().Length; i++)
            {
                FieldInfo info = target.GetType().GetFields()[i];
                names.Add(info.Name);
            }
            return names.ToArray();
        }

        public string[] GetSubNames()
        {
            List<string> names = new List<string>();

            FieldInfo info = target.GetType().GetField(varName);
            object subObj = info.GetValue(target);
            if (info != null && subObj != null)
            {
                int amount = subObj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Length;
                if (amount > 0)
                    names.Add(info.Name);
                for (int i = 0; i < amount; i++)
                {
                    FieldInfo info2 = subObj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)[i];
                    if (info2 != null)
                    {
                        names.Add(info2.Name);
                    }
                }
            }
            return names.ToArray();
        }

        public System.Type GetVarType()
        {
            FieldInfo info = target.GetType().GetField(varName);
            if (varName == subVarName)
                return info.FieldType;
            else
                return info.GetValue(target).GetType().GetField(subVarName).FieldType;
        }

        public string[] GetComparers(out bool value)
        {
            if (type == Types.Invalid)
            {
                value = false;
                return new string[0];
            }

            value = true;
            if (type == Types.Float || type == Types.Int)
                return longComparer;
            else
                return shortComparer;
        }

        public bool Compare()
        {
            var type = GetVarType();
            bool met = false;
            if (varName == subVarName)
            {
                object objValue = target.GetType().GetField(varName).GetValue(target);

                if (comparer == 0)
                {
                    if (type == typeof(bool))
                        met = (bool)objValue == boolArgument;
                    else if (type == typeof(int))
                        met = (int)objValue == intArgument;
                    else if (type == typeof(float))
                        met = (float)objValue == floatArgument;
                    else if (type == typeof(string))
                        met = (string)objValue == stringArgument;
                    else if (type == typeof(Vector3))
                        met = (Vector3)objValue == vector3Argument;
                    else if (type == typeof(Vector2))
                        met = (Vector2)objValue == vector2Argument;
                    else
                        met = (Object)objValue == objectArgument;
                }
                else if (comparer == 1)
                {
                    if (type == typeof(bool))
                        met = (bool)objValue != boolArgument;
                    else if (type == typeof(int))
                        met = (int)objValue != intArgument;
                    else if (type == typeof(float))
                        met = (float)objValue != floatArgument;
                    else if (type == typeof(string))
                        met = (string)objValue != stringArgument;
                    else if (type == typeof(Vector3))
                        met = (Vector3)objValue != vector3Argument;
                    else if (type == typeof(Vector2))
                        met = (Vector2)objValue != vector2Argument;
                    else
                        met = (Object)objValue != objectArgument;
                }
                else if (comparer == 2)
                {
                    if (type == typeof(int))
                        met = (int)objValue > intArgument;
                    else if (type == typeof(float))
                        met = (float)objValue > floatArgument;
                }
                else if (comparer == 3)
                {
                    if (type == typeof(int))
                        met = (int)objValue < intArgument;
                    else if (type == typeof(float))
                        met = (float)objValue < floatArgument;
                }
            }
            else
            {
                object subObj = target.GetType().GetField(varName).GetValue(target);
                object subObjValue = subObj.GetType().GetField(subVarName).GetValue(subObj);

                if (comparer == 0)
                {
                    if (type == typeof(bool))
                        met = (bool)subObjValue == boolArgument;
                    else if (type == typeof(int))
                        met = (int)subObjValue == intArgument;
                    else if (type == typeof(float))
                        met = (float)subObjValue == floatArgument;
                    else if (type == typeof(string))
                        met = (string)subObjValue == stringArgument;
                    else if (type == typeof(Vector3))
                        met = (Vector3)subObjValue == vector3Argument;
                    else if (type == typeof(Vector2))
                        met = (Vector2)subObjValue == vector2Argument;
                    else
                        met = (Object)subObjValue == objectArgument;
                }
                else if (comparer == 1)
                {
                    if (type == typeof(bool))
                        met = (bool)subObjValue != boolArgument;
                    else if (type == typeof(int))
                        met = (int)subObjValue != intArgument;
                    else if (type == typeof(float))
                        met = (float)subObjValue != floatArgument;
                    else if (type == typeof(string))
                        met = (string)subObjValue != stringArgument;
                    else if (type == typeof(Vector3))
                        met = (Vector3)subObjValue != vector3Argument;
                    else if (type == typeof(Vector2))
                        met = (Vector2)subObjValue != vector2Argument;
                    else
                        met = (Object)subObjValue != objectArgument;
                }
                else if (comparer == 2)
                {
                    if (type == typeof(int))
                        met = (int)subObjValue > intArgument;
                    else if (type == typeof(float))
                        met = (float)subObjValue > floatArgument;
                }
                else if (comparer == 3)
                {
                    if (type == typeof(int))
                        met = (int)subObjValue < intArgument;
                    else if (type == typeof(float))
                        met = (float)subObjValue < floatArgument;
                }
            }
            return met;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Conditions))]
public class ConditionsEditor : Editor
{
    Component[] comps;

    private void OnSelect(object value)
    {
        Vector2Int v2i = (Vector2Int)value; 

        Conditions c = (Conditions)target;
        c.conditions[v2i.x].target = comps[v2i.y];
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();
        Conditions c = (Conditions)target;
       
        if (c.conditions != null)
        {
            for (int i = 0; i < c.conditions.Count; i++)
            {
                Conditions.Condition condition = c.conditions[i];

                EditorGUILayout.BeginHorizontal();

                condition.target = EditorGUILayout.ObjectField(condition.target, typeof(Object), true, GUILayout.Width(200));

                if (condition.target)
                {
                    if (condition.target.GetType() == typeof(GameObject))
                    {
                        if (GUILayout.Button("Select Component"))
                        {
                            comps = ((GameObject)condition.target).GetComponents<Component>();
                            GenericMenu menu = new GenericMenu();

                            for (int j = 0; j < comps.Length; j++)
                            {
                                menu.AddItem(new GUIContent(comps[j].ToString()), false, OnSelect, new Vector2Int(i, j));
                            }

                            menu.ShowAsContext();
                            condition.index = 0;
                        }
                    }

                    if (condition.GetNames().Length > 0)
                    {
                        condition.index = EditorGUILayout.Popup(condition.index, condition.GetNames(), GUILayout.MinWidth(100));
                        if (condition.varName != condition.GetNames()[condition.index])
                        {
                            condition.comparer = 0;
                            condition.subIndex = 0;
                        }
                        condition.varName = condition.GetNames()[condition.index];

                        if (condition.GetSubNames().Length > 0)
                        {
                            condition.subIndex = EditorGUILayout.Popup(condition.subIndex, condition.GetSubNames(), GUILayout.MinWidth(100));
                            if (condition.subVarName != condition.GetSubNames()[condition.subIndex])
                                condition.comparer = 0;
                            condition.subVarName = condition.GetSubNames()[condition.subIndex];
                        }
                        else
                            condition.subVarName = condition.varName;

                        bool showComparer = false;
                        string[] comparers = condition.GetComparers(out showComparer);
                        if (showComparer)
                            condition.comparer = EditorGUILayout.Popup(condition.comparer, comparers);
                        else
                            EditorGUILayout.LabelField(condition.GetVarType().ToString() + " is an invalid type.", GUILayout.Width(200));

                        if (condition.GetVarType() == typeof(bool))
                        {
                            condition.type = Conditions.Condition.Types.Bool;
                            condition.boolArgument = EditorGUILayout.Toggle((bool)condition.boolArgument, GUILayout.Width(200));
                        }
                        else if (condition.GetVarType() == typeof(string))
                        {
                            condition.type = Conditions.Condition.Types.String;
                            condition.stringArgument = EditorGUILayout.TextField((string)condition.stringArgument, GUILayout.Width(200));
                        }
                        else if (condition.GetVarType() == typeof(float))
                        {
                            condition.type = Conditions.Condition.Types.Float;
                            condition.floatArgument = EditorGUILayout.FloatField((float)condition.floatArgument, GUILayout.Width(200));
                        }
                        else if (condition.GetVarType() == typeof(int))
                        {
                            condition.type = Conditions.Condition.Types.Int;
                            condition.intArgument = EditorGUILayout.IntField((int)condition.intArgument, GUILayout.Width(200));
                        }
                        else if (condition.GetVarType() == typeof(Vector3))
                        {
                            condition.type = Conditions.Condition.Types.Vector3;
                            condition.vector3Argument = EditorGUILayout.Vector3Field("", condition.vector3Argument, GUILayout.Width(200));
                        }
                        else if (condition.GetVarType() == typeof(Vector2))
                        {
                            condition.type = Conditions.Condition.Types.Vector2;
                            condition.vector2Argument = EditorGUILayout.Vector2Field("", condition.vector2Argument, GUILayout.Width(200));
                        }
                        else if (condition.GetVarType().Module == typeof(Object).Module)
                        {
                            if (condition.objectArgument)
                            {
                                if (condition.objectArgument.GetType() != condition.GetVarType())
                                    condition.objectArgument = null;
                            }
                            condition.type = Conditions.Condition.Types.Object;
                            condition.objectArgument = EditorGUILayout.ObjectField(condition.objectArgument, condition.GetVarType(), true, GUILayout.Width(200));
                        }
                        else
                        {
                            condition.type = Conditions.Condition.Types.Invalid;
                        }
                    }
                    else
                    {
                        condition.type = Conditions.Condition.Types.Invalid;
                        EditorGUILayout.LabelField(condition.target.GetType().Name + " is an invalid type.", GUILayout.Width(200));
                    }
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    c.conditions.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Add"))
        {
            if (c.conditions == null)
                c.conditions = new List<Conditions.Condition>();
            c.conditions.Add(new Conditions.Condition());
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif