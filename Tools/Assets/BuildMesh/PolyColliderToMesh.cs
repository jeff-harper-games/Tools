using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PolygonCollider2D))]
public class PolyColliderToMesh : MonoBehaviour
{
    [SerializeField]
    private bool buildAtStart = true;

    public void Start()
    {
        if(buildAtStart)
            BuildMesh.Build(gameObject, GetComponent<PolygonCollider2D>());
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PolyColliderToMesh))]
public class PointsToMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PolyColliderToMesh go = target as PolyColliderToMesh;

        if (GUILayout.Button("Build Mesh"))
        {
            BuildMesh.Build(go.gameObject, go.GetComponent<PolygonCollider2D>());
        }
    }
}
#endif