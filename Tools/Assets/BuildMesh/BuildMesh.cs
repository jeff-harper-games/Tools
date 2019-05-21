using System.Collections.Generic;
using UnityEngine;

public static class BuildMesh
{
    public static void Build(GameObject container, PolygonCollider2D poly)
    {
        List<Mesh> meshes = new List<Mesh>();

        for (int i = 0; i < poly.pathCount; i++)
        {
            Vector2[] verts = poly.GetPath(i);

            // http://wiki.unity3d.com/index.php/Triangulator
            Triangulator tr = new Triangulator(verts);
            int[] indices = tr.Triangulate();

            Vector3[] vertices = new Vector3[verts.Length];
            for (int j = 0; j < verts.Length; j++)
            {
                vertices[j] = new Vector3(verts[j].x, verts[j].y, 0);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            meshes.Add(mesh);
        }

        CombineInstance[] combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < combine.Length; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = container.transform.localToWorldMatrix;
        }
        
        
        MeshRenderer mr = container.GetComponent<MeshRenderer>();
        if (!mr)
            mr = container.AddComponent<MeshRenderer>();        

        MeshFilter mf = container.GetComponent<MeshFilter>();
        if(!mf)
            mf = container.AddComponent<MeshFilter>();

        mf.sharedMesh = new Mesh();
        mf.sharedMesh.CombineMeshes(combine);
    }
}