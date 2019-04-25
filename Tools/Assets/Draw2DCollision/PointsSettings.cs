using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsSettings : ScriptableObject
{
    public Color handleColor = Color.cyan;
    public Color lineColor = Color.white;
    public Color unselectedColor = Color.black;
    public float handleSize = 0.1f;
    public Vector2 handleRange = new Vector2(0, 1);
    public bool autoBuild = true; 
    public bool edit = true;
    public bool showControls = true;
}
