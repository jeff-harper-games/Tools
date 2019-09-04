using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[AddComponentMenu("Tools/Points")]
public class Points : MonoBehaviour
{
    public PolygonCollider2D polyCol;
    public Color handleColor = Color.cyan;
    public Color lineColor = Color.white;
    public Color unselectedColor = Color.white;
    public float handleSize = 0.1f;
    public bool edit = true;
    public bool autoBuild = true;
    public bool showControls = true;
    public bool useGlobal = false;
    public PointsSettings settings; 
    public List<Paths> paths = new List<Paths>();

    [System.Serializable]
    public class Paths
    {
        public List<Vector3> points = new List<Vector3>();
    }

    public void AddPath()
    {
        if (!polyCol)
            polyCol = GetComponent<PolygonCollider2D>();

        paths.Add(new Paths());
    }

    public void RemovePath(int index)
    {
        if (!polyCol)
            polyCol = GetComponent<PolygonCollider2D>();

        paths.RemoveAt(index);
    }

    public void AddPoint(Vector3 pos, int path = 0)
    {
        if (!polyCol)
            polyCol = GetComponent<PolygonCollider2D>();

        if (path > paths.Count - 1)
            paths.Add(new Paths());

        pos = new Vector3(pos.x, pos.y, polyCol.transform.position.z);
        paths[path].points.Add(pos);
    }

    public void InsertPoint(Vector3 pos, int index, int path = 0)
    {
        if (!polyCol)
            polyCol = GetComponent<PolygonCollider2D>();

        pos = new Vector3(pos.x, pos.y, polyCol.transform.position.z);
        paths[path].points.Insert(index, pos);
    }

    public void BuildCollider()
    {
        if (!polyCol)
            polyCol = GetComponent<PolygonCollider2D>();

        polyCol.pathCount = paths.Count;

        for (int i = 0; i < paths.Count; i++)
        {
            Vector2[] verts = new Vector2[paths[i].points.Count];
            for (int j = 0; j < paths[i].points.Count; j++)
            {
                //Vector2 pos = new Vector2(paths[i].points[j].x - polyCol.transform.position.x, paths[i].points[j].y - polyCol.transform.position.y);
                verts[j] = paths[i].points[j] - polyCol.transform.position;
            }
            polyCol.SetPath(i, verts);
        }
    }
}