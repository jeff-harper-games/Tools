using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

public class MultipleSprite : EditorWindow
{
    [MenuItem("Tools/Generate Multiple")]
    public static void GenerateMultiple()
    {
        string path = EditorUtility.OpenFolderPanel("Sprite Folder", "", "");
        string relativePath = "Assets" + path.Substring(Application.dataPath.Length);

        if (!AssetDatabase.IsValidFolder(relativePath))
            return;

        string[] folders = new string[] { relativePath };
        string[] textures = AssetDatabase.FindAssets("t:Texture2D", folders);

        for (int i = 0; i < textures.Length; i++)
        {
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(textures[i]));
            Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(textures[i]), typeof(Texture2D));
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.isReadable = true;
            importer.maxTextureSize = 8192;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            AssetDatabase.Refresh();

            int minimumSpriteSize = 16;
            int extrudeSize = 0;
            Rect[] rects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles(texture, minimumSpriteSize, extrudeSize);

            string p = AssetDatabase.GUIDToAssetPath(textures[i]);
            string relative = p.Replace("Assets/", "");
            string absolutePath = Application.dataPath + "/" + p;

            string filenameNoExtension = Path.GetFileNameWithoutExtension(absolutePath);
            List<SpriteMetaData> metas = new List<SpriteMetaData>();
            int rectNum = 0;

            foreach (Rect rect in rects)
            {
                SpriteMetaData meta = new SpriteMetaData();
                meta.rect = rect;
                meta.name = filenameNoExtension + "_" + rectNum++;
                Debug.Log(meta.name);
                metas.Add(meta);
            }

            importer.spritesheet = metas.ToArray();
            importer.SaveAndReimport();
            AssetDatabase.Refresh();
        }
    }    
}
