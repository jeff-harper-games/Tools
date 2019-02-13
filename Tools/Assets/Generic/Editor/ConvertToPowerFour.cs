using UnityEngine;
using UnityEditor;

public class ConvertToPowerFour : Editor
{
    [MenuItem("Tools/Power of Four #&p")]
    public static void Convert()
    {
        Texture2D tex = Selection.activeObject as Texture2D;
        if (tex)
        {
            string texPath = AssetDatabase.GetAssetPath(tex);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
            importer.crunchedCompression = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.isReadable = true;
            importer.SaveAndReimport();

            int width = tex.width;
            int height = tex.height;

            while (width % 4 != 0)
            {
                width++;
            }

            while (height % 4 != 0)
            {
                height++;
            }

            Texture2D newTex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    Color col = tex.GetPixel(x, y);
                    newTex.SetPixel(x, y, col);
                }
            }

            newTex.Apply();

            string path = AssetDatabase.GetAssetPath(tex);
            string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
            System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

            importer.crunchedCompression = true;
            importer.compressionQuality = 50;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.isReadable = false;
            importer.SaveAndReimport();

            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("Must have a image selected");
        }
    }
}