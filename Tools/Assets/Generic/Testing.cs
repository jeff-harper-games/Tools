using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public References references;
    public float floatValue;
    public string stringValue;
    public bool boolValue;
    public int intValue;
    public Vector2 vector2Value; 

    [System.Serializable]
    public class References
    {
        public GameObject gameObjectValue;
        public Sprite spriteValue;
        public Transform transformValue;
        public Vector3 vector3Value;
    }
}
