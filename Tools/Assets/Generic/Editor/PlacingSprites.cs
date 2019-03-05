using UnityEngine;
using UnityEditor;

public class PlacingSprites : Editor
{
    [MenuItem("Tools/Place Sprites")]
    public static void Place()
    {
        Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].GetType() == typeof(Texture2D))
            {
                Texture2D tex = (Texture2D)objs[i];
                string path = AssetDatabase.GetAssetPath(tex);
                Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
                float pixels = importer.spritePixelsPerUnit;
                GameObject parent = new GameObject(tex.name);
                for (int j = 0; j < sprites.Length; j++)
                {
                    if (sprites[j].GetType() == typeof(Sprite))
                    {
                        Sprite sprite = (Sprite)sprites[j];
                        GameObject go = new GameObject(sprite.name);
                        go.transform.parent = parent.transform;
                        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                        sr.sprite = sprite;
                        Rect rect = sprite.rect;
                        sr.transform.position = new Vector2((rect.x / pixels) - ((tex.width / 2) / pixels) + rect.width / pixels / 2,
                            (rect.y / pixels) - ((tex.height / 2) / pixels) + rect.height / pixels / 2);
                    }
                }
            }
        }
    }
}