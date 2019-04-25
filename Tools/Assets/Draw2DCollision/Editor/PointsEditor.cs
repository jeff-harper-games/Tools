using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Points))]
public class PointsEditor : Editor
{
    private int? mouseOverPoint;
    private int rightClickPoint;
    private int rightClickPath; 
    private bool dragging;
    private int currentPath = 0;
    private float height = 200;
    private float collapseHeight = 155;
    private float expandedHeight = 245;
    private float width = 210;

    private Color handleColor = Color.cyan;
    private Color lineColor = Color.white;
    private Color unselectedColor = Color.white;
    private float handleSize = 0.1f;
    private bool edit = true;
    private bool autoBuild = true;
    private bool showControls = true;

    private SceneView view;
    private bool locked = false;
    private bool is2D = false;

    private bool showPositionSetting = false;
    private Vector2 positionSettingStart;

    [MenuItem("GameObject/2D Object/Points", false)]
    public static void Create()
    {
        GameObject go = new GameObject("Points");
        go.AddComponent<Points>();
        Selection.activeGameObject = go;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Points)target), typeof(Points), false);
    }

    private void OnEnable()
    {
        locked = false; 
    }

    private void OnDisable()
    {
        if (view)
            view.in2DMode = is2D;
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.Layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        #region Set SceneView Settings
        SceneView currentView = SceneView.currentDrawingSceneView;
        if (view != currentView || !locked)
        {
            is2D = currentView.in2DMode;
            currentView.in2DMode = true; 
            view = currentView;
            locked = true; 
        }
        #endregion

        Points points = (Points)target;

        #region Determine Global Settings
        if (points.useGlobal)
        {
            if (!points.settings)
                points.settings = (PointsSettings)Resources.Load("PointsSettings", typeof(PointsSettings));

            if (!points.settings)
            {
                Handles.BeginGUI();
                GUI.Box(new Rect(5, 5, width + 10, height + 10), "");
                GUILayout.BeginArea(new Rect(10, 10, width, height));

                GUILayout.Label("Points Settings", EditorStyles.boldLabel);

                if (GUILayout.Button("Create Global Settings"))
                {
                    string path = EditorUtility.SaveFolderPanel("Choose a folder", "", "");
                    string relative = path.Replace(Application.dataPath, "Assets");
                    if (AssetDatabase.IsValidFolder(relative))
                    {
                        points.settings = CreateInstance<PointsSettings>();
                        AssetDatabase.CreateAsset(points.settings, relative + "/PointsSettings.asset");
                    }
                }

                if (GUILayout.Button("Locate Global Settings"))
                {
                    string path = EditorUtility.OpenFilePanel("Find Points Settings", "", "asset");
                    string relative = path.Replace(Application.dataPath, "Assets");
                    points.settings = (PointsSettings)AssetDatabase.LoadAssetAtPath(relative, typeof(PointsSettings));

                    if (!points.settings)
                        Debug.LogError("Invalid File");
                }

                GUILayout.EndArea();
                Handles.EndGUI();

                return;
            }
        }
        #endregion

        Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        float drawPlaneHeight = 0;
        float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3 mousePosition = mouseRay.GetPoint(dstToDrawPlane);

        if (e.type == EventType.Repaint && edit)
            Draw(mousePosition);
        else
        {
            Input(e, mousePosition);
            HandleUtility.Repaint();
        }

        #region Control Window
        autoBuild = points.useGlobal ? points.settings.autoBuild : points.autoBuild;
        edit = points.useGlobal ? points.settings.edit : points.edit;

        Handles.BeginGUI();
        GUI.Box(new Rect(5, 5, width + 10, height + 10), CreateTexture((int)width + 10, (int)height + 10, new Color(1,1,1,.75f)));
        GUI.contentColor = Color.black;
        GUI.backgroundColor = Color.white;
        GUILayout.BeginArea(new Rect(10, 10, width, height));

        GUILayout.BeginHorizontal();
        GUILayout.Label("Points Settings", EditorStyles.boldLabel);
        string str = points.useGlobal ? "Global" : "Local";

        GUI.contentColor = Color.white;
        if (GUILayout.Button(str))
        {
            points.useGlobal = !points.useGlobal;
            return;
        }
        GUILayout.EndHorizontal();

        GUI.contentColor = Color.black;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Edit", GUILayout.Width(80));
        if(!points.useGlobal)
            points.edit = EditorGUILayout.Toggle(points.edit);
        else
            points.settings.edit = EditorGUILayout.Toggle(points.settings.edit);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Auto Build", GUILayout.Width(80));
        if(!points.useGlobal)
            points.autoBuild = EditorGUILayout.Toggle(points.autoBuild, GUILayout.Width(30));
        else
            points.settings.autoBuild = EditorGUILayout.Toggle(points.settings.autoBuild, GUILayout.Width(30));
        if (!autoBuild)
        {
            GUI.contentColor = Color.white;
            if (GUILayout.Button("Build"))
                points.BuildCollider();
            GUI.contentColor = Color.black;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Handle Size", GUILayout.Width(80));
        if (!points.useGlobal)
            points.handleSize = GUILayout.HorizontalSlider(points.handleSize, 0, 1, GUILayout.Width(width - 85));
        else
            points.settings.handleSize = GUILayout.HorizontalSlider(points.settings.handleSize, points.settings.handleRange.x, points.settings.handleRange.y, GUILayout.Width(width - 85));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Handle Color", GUILayout.Width(80));
        if (!points.useGlobal)
            points.handleColor = EditorGUILayout.ColorField(points.handleColor);
        else
            points.settings.handleColor = EditorGUILayout.ColorField(points.settings.handleColor);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Line Color", GUILayout.Width(80));
        if (!points.useGlobal)
            points.lineColor = EditorGUILayout.ColorField(points.lineColor);
        else
            points.settings.lineColor = EditorGUILayout.ColorField(points.settings.lineColor);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Default Color", GUILayout.Width(80));
        if (!points.useGlobal)
            points.unselectedColor = EditorGUILayout.ColorField(points.unselectedColor);
        else
            points.settings.unselectedColor = EditorGUILayout.ColorField(points.settings.unselectedColor);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (!points.useGlobal)
            points.showControls = EditorGUILayout.Foldout(points.showControls, "Controls");
        else
            points.settings.showControls = EditorGUILayout.Foldout(points.settings.showControls, "Controls", true);
        showControls = points.useGlobal ? points.settings.showControls : points.showControls;
        if (showControls)
        {
            GUILayout.Label("Shift + Left Click = Add Point");
            GUILayout.Label("Control + Left Click = Delete Point");
            GUILayout.Label("Left Click + Drag = Move Point");
            GUILayout.Label("Shift + Right Click = Add Path");
            GUILayout.Label("Control + Right Click = Delete Path");
            height = expandedHeight;
        }
        else
            height = collapseHeight;

        GUILayout.EndArea();
        Handles.EndGUI();
        #endregion

        #region Position Popup Settings
        if (showPositionSetting)
        {
            Handles.BeginGUI();

            Vector3 pos = points.paths[currentPath].points[rightClickPoint];
            pos.x = points.paths[currentPath].points[rightClickPoint].x + handleSize;
            Vector2 v2 = HandleUtility.WorldToGUIPoint(pos);

            GUI.DrawTexture(new Rect(v2.x + 5, v2.y - 15f, 260, 25), CreateTexture(215, 25, handleColor));

            GUILayout.BeginArea(new Rect(v2.x + 10, v2.y - 10f, 250, 20));

            EditorGUILayout.BeginHorizontal();

            float x = points.paths[currentPath].points[rightClickPoint].x;
            float y = points.paths[currentPath].points[rightClickPoint].y;

            Color textColor = new Color();
            if (handleColor.r < 0.5f && handleColor.g < 0.5f && handleColor.b < 0.5f)
                textColor = Color.white;
            else
                textColor = Color.black;

            GUISkin skin = (GUISkin)Resources.Load("2DCollisionSkin", typeof(GUISkin));
            GUIStyle labelStyle = new GUIStyle(skin.GetStyle("label"));
            GUIStyle textStyle = new GUIStyle(skin.GetStyle("textField"));

            EditorGUI.BeginChangeCheck();

            GUI.contentColor = textColor;
            EditorGUILayout.LabelField("x", labelStyle, GUILayout.Width(15));
            GUI.contentColor = Color.white;
            x = EditorGUILayout.FloatField(x);
            GUI.contentColor = textColor;
            EditorGUILayout.LabelField("y", labelStyle, GUILayout.Width(15));
            GUI.contentColor = Color.white;
            y = EditorGUILayout.FloatField(y);
            points.paths[currentPath].points[rightClickPoint] = new Vector3(x, y);

            if (GUI.changed)
            {
                if (autoBuild)
                    points.BuildCollider();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.EndArea();

            Handles.EndGUI();
        }
        #endregion
    }

    private void Draw(Vector3 mouse)
    {
        Points points = (Points)target;

        mouseOverPoint = null;

        handleSize = points.useGlobal ? points.settings.handleSize : points.handleSize;
        lineColor = points.useGlobal ? points.settings.lineColor : points.lineColor;
        handleColor = points.useGlobal ? points.settings.handleColor : points.handleColor;
        unselectedColor = points.useGlobal ? points.settings.unselectedColor : points.unselectedColor;

        #region Draw Lines
        for (int i = 0; i < points.paths.Count; i++)
        {
            if (i == currentPath)
                Handles.color = lineColor;
            else
                Handles.color = unselectedColor;

            for (int j = 0; j < points.paths[i].points.Count; j++)
            {
                Vector3 p1 = new Vector3(points.paths[i].points[j].x, points.paths[i].points[j].y, points.paths[i].points[j].z);
                Vector3 p2 = Vector2.zero;
                if (j > 0)
                    p2 = new Vector3(points.paths[i].points[j - 1].x, points.paths[i].points[j - 1].y, points.paths[i].points[j - 1].z);
                else
                    p2 = new Vector3(points.paths[i].points[points.paths[i].points.Count - 1].x, points.paths[i].points[points.paths[i].points.Count - 1].y,
                        points.paths[i].points[points.paths[i].points.Count - 1].z);
                Handles.DrawLine(p1, p2);
            }
        }
        #endregion

        #region Draw Points
        for (int i = 0; i < points.paths.Count; i++)
        {
            if (i == currentPath)
                Handles.color = handleColor;
            else
                Handles.color = unselectedColor;

            for (int j = 0; j < points.paths[i].points.Count; j++)
            {
                Handles.DrawSolidDisc(points.paths[i].points[j], Vector3.back, handleSize);
                if (MouseOver(mouse, points.paths[i].points[j]))
                {
                    mouseOverPoint = j;
                    currentPath = i;
                }
            }
        }
        #endregion

        if (mouseOverPoint == null)
            dragging = false;

        if (currentPath != rightClickPath)
            showPositionSetting = false; 
    }

    private void Input(Event e, Vector3 mouse)
    {
        Points points = (Points)target;
        Rect rect = new Rect(5, 5, width + 10, height + 10);

        #region Input while over point
        if (!rect.Contains(e.mousePosition) && mouseOverPoint != null)
        {
            if (mouseOverPoint != null)
            {
                if (dragging)
                {
                    Undo.RecordObject(points, "Point Moved");
                    points.paths[currentPath].points[(int)mouseOverPoint] = mouse;
                    if (autoBuild)
                        points.BuildCollider();
                }

                if (e.type == EventType.MouseDown && e.button == 0 && e.modifiers != EventModifiers.Shift)
                {
                    if (e.modifiers != EventModifiers.Control)
                        dragging = true;
                }

                if (e.type == EventType.MouseUp && e.button == 0)
                    dragging = false;

                if (e.type == EventType.MouseDown && e.button == 0 && e.modifiers == EventModifiers.Control)
                {
                    Undo.RecordObject(points, "Point Removed");
                    points.paths[currentPath].points.RemoveAt((int)mouseOverPoint);
                    if (autoBuild)
                        points.BuildCollider();
                }

                if (e.type == EventType.MouseDown && e.button == 1 && e.modifiers != EventModifiers.Control)
                {
                    positionSettingStart = e.mousePosition;

                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Set Position"), false, Callback, 1);

                    string str = showPositionSetting ? "Close" : "Cancel";
                    menu.AddItem(new GUIContent(str), false, Callback, 2);
                    menu.ShowAsContext();
                }

                if (e.type == EventType.MouseDown && e.button == 1 && e.modifiers == EventModifiers.Control)
                {
                    Undo.RecordObject(points, "Remove Path");
                    points.RemovePath(currentPath);
                    if (autoBuild)
                        points.BuildCollider();
                }

                if (e.type == EventType.MouseDown)
                {
                    if ((int)mouseOverPoint != rightClickPoint)
                        showPositionSetting = false;
                }
            }
        }
        #endregion

        #region Input while not over point
        if (!rect.Contains(e.mousePosition) && mouseOverPoint == null)
        {
            if (e.type == EventType.MouseDown && e.button == 0 && e.modifiers == EventModifiers.Shift)
            {
                handleSize = points.useGlobal ? points.settings.handleSize : points.handleSize;

                float closestLine = handleSize;
                int index = -1;
                Vector2 m2 = new Vector2(mouse.x, mouse.y);
                for (int i = 0; i < points.paths.Count; i++)
                {
                    for (int j = 0; j < points.paths[i].points.Count; j++)
                    {
                        Vector2 p1 = new Vector2(points.paths[i].points[j].x, points.paths[i].points[j].y);
                        Vector2 p2 = Vector2.zero;
                        if (j > 0)
                            p2 = new Vector2(points.paths[i].points[j-1].x, points.paths[i].points[j-1].y);
                        else
                            p2 = new Vector2(points.paths[i].points[points.paths[i].points.Count - 1].x, points.paths[i].points[points.paths[i].points.Count - 1].y);

                        float dist = HandleUtility.DistancePointToLineSegment(m2, p1, p2);

                        if (dist < closestLine)
                        {
                            closestLine = dist;
                            index = j;
                        }
                    }
                }

                Undo.RecordObject(points, "Add Point");

                if (index > -1)
                    points.InsertPoint(m2, index, currentPath);
                else
                    points.AddPoint(m2, currentPath);

                SceneView.RepaintAll();
                if(autoBuild)
                    points.BuildCollider();
            }

            if (e.type == EventType.MouseDown && e.button == 1 && e.modifiers == EventModifiers.Shift)
            {
                Undo.RecordObject(points, "Add Path");
                points.AddPath();
                currentPath = points.paths.Count - 1;
                points.AddPoint(mouse, currentPath);
                if (autoBuild)
                    points.BuildCollider();
            }
        }
        #endregion
    }

    #region Utility Methods
    private void Callback(object obj)
    {
        Points points = (Points)target;
        // Do something
        switch ((int)obj)
        {
            case 1:
                rightClickPoint = (int)mouseOverPoint;
                rightClickPath = currentPath;
                showPositionSetting = true; 
                break;
            case 2:
                showPositionSetting = false;
                break;
            default:
                break;
        }
    }

    private Texture2D CreateTexture(int width, int height, Color col)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; ++i)
        {
            pixels[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }

    private bool MouseOver(Vector3 mouse, Vector3 pos)
    {
        Points points = (Points)target; 

        bool x = false;
        bool y = false;

        if (mouse.x > pos.x - handleSize && mouse.x < pos.x + handleSize)
            x = true;
        if (mouse.y > pos.y - handleSize && mouse.y < pos.y + handleSize)
            y = true;

        if (x && y)
            return true;
        else
            return false;
    }
    #endregion
}