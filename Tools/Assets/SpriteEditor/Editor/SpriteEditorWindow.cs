/*

    Author: Jeff Harper
    Twitter: @jeffdevsitall
    Website: jeffharper.games
 
    Copyright (c) 2019 by Jeff Harper

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
using System.IO;

public class SpriteEditorWindow : EditorWindow
{
    #region Variables
    private int tab = 0;
    public List<Texture2D> sprites = new List<Texture2D>();
    private bool powerOf4 = true;
    private bool autoSplice = true; 
    private bool placeInScene = false; 
    private SpriteImportMode mode = SpriteImportMode.Single;
    private bool showBackground;
    private string[] convertMaxSize = new string[] { "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192" };
    private int size = 6;
    private TextureImporterCompression compressionQuality = TextureImporterCompression.Compressed;
    private bool useCrunch = true;
    private int quality = 50;
    private bool alreadyPower4;
    private bool updateMode = true;
    private ReorderableList list;
    private bool dragging;
    private bool showList = true;
    private int splitSize = 6;
    private TextureImporterType splitImporterType = TextureImporterType.Sprite;
    private bool useImportSettings = true;
    private bool createSplitFolder = true; 
    #endregion

    [MenuItem("Tools/Sprite Editor")]
    public static void LaunchSpriteEditor()
    {
        SpriteEditorWindow window = GetWindow<SpriteEditorWindow>();
        window.Show();
    }

    private void OnEnable()
    {
        DrawList();
    }

    private void DrawList()
    {
        SerializedObject so = new SerializedObject(this);
        list = new ReorderableList(so, so.FindProperty("sprites"), true, false, false, true);

        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                if (sprites[index])
                {
                    string label = sprites[index].width + "px X " + sprites[index].height + "px";
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 + 50, rect.y, rect.width / 3 - 50, EditorGUIUtility.singleLineHeight), label);
                    if (GUI.Button(new Rect(rect.x + ((rect.width / 3) * 2 + 100), rect.y, rect.width / 3 - 100, EditorGUIUtility.singleLineHeight), "Focus"))
                        Selection.activeObject = sprites[index];
                }
            };

        list.onRemoveCallback = (ReorderableList removeList) => 
        {
            sprites.RemoveAt(removeList.index);
            ReorderableList.defaultBehaviours.DoRemoveButton(removeList);
            DrawList();
        };
    }

    private void OnGUI()
    {
        if (!dragging)
        {
            int previousTab = tab;
            tab = GUILayout.Toolbar(tab, new string[] { "Single", "Batch" });
            if (tab == 0)
                DisplaySingle();
            else
            {
                if (previousTab != tab)
                    DrawList();
                DisplayBatch();
            }
        }
        else
        {
            GUILayout.BeginArea(new Rect(0, position.height / 2 - 50, position.width, 100));
            GUIStyle style = new GUIStyle();
            style.fontSize = 50;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Drop Textures or Folder To Add", style);
            GUILayout.EndArea();
        }

        var e = Event.current.type;
        if (e == EventType.DragUpdated)
        {
            dragging = true;
            Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            if (position.Contains(mouse))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
        }
        else if (e == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                Object obj = DragAndDrop.objectReferences[i];
                if (obj.GetType() == typeof(Texture2D))
                {
                    if (tab != 0)
                    {
                        bool missing = false; 
                        for (int j = 0; j < sprites.Count; j++)
                        {
                            if (!sprites[j])
                            {
                                sprites[j] = (Texture2D)obj;
                                missing = true;
                                break;
                            }
                        }
                        if (!sprites.Contains((Texture2D)obj) && !missing)
                            sprites.Add((Texture2D)obj);
                    }
                    else
                    {
                        sprites[0] = (Texture2D)obj;
                        break;
                    }
                }
                else
                {
                    if (obj.GetType() == typeof(DefaultAsset))
                    {
                        string path = DragAndDrop.paths[i];
                        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new string[] { path });
                        for (int j = 0; j < guids.Length; j++)
                        {
                            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[j]), typeof(Texture2D));
                            if (!sprites.Contains(tex))
                                sprites.Add(tex);
                        }
                    }
                }
            }
            DrawList();
            dragging = false;
            Event.current.Use();
        }
        else if (e == EventType.DragExited)
        {
            dragging = false;
        }
    }

    private void DisplaySingle()
    {
        // make sure the list is not empty
        // if so add null sprite
        if (sprites.Count == 0)
            sprites.Add(null);

        
        EditorGUILayout.BeginHorizontal();

        // begin left column for variables and properties
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2));
        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        // label for sprite 
        EditorGUILayout.LabelField("Sprite", GUILayout.Width(130));
        // display the sprite object for the 0 index
        sprites[0] = (Texture2D)EditorGUILayout.ObjectField(sprites[0], typeof(Texture2D), true);
        if (sprites[0])
        {
            if (GUILayout.Button("Focus", GUILayout.Width(60)))
                Selection.activeObject = sprites[0];
        }
        // end horizontal row
        EditorGUILayout.EndHorizontal();

        if (!sprites[0])
            return;

        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(sprites[0]));

        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        // label for sprite 
        EditorGUILayout.LabelField("Show Checkers", GUILayout.Width(130));
        // toggle for power of 4
        showBackground = EditorGUILayout.Toggle(showBackground, GUILayout.Width(40));
        // end horizontal row
        EditorGUILayout.EndHorizontal();

        if (sprites[0].width % 4 == 0 && sprites[0].height % 4 == 0)
            alreadyPower4 = true;
        else
            alreadyPower4 = false;

        if (!alreadyPower4)
        {
            // start horizontal row
            EditorGUILayout.BeginHorizontal();
            // label for sprite 
            EditorGUILayout.LabelField("Power of 4", GUILayout.Width(130));
            // toggle for power of 4
            powerOf4 = EditorGUILayout.Toggle(powerOf4, GUILayout.Width(40));
            if (powerOf4)
            {
                EditorGUILayout.LabelField("Adds white space to make both width and height divisible by 4");
                if (GUILayout.Button("Apply"))
                {
                    MakePowerOf4(sprites[0]);
                }
            }
            // end horizontal row
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sprite Mode Settings", EditorStyles.boldLabel);

        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        // label for sprite mode 
        EditorGUILayout.LabelField("Sprite Mode", GUILayout.Width(130));
        // enum popup for sprite mode 
        mode = (SpriteImportMode)EditorGUILayout.EnumPopup(mode);
        // end horizontal row
        EditorGUILayout.EndHorizontal();

        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(130));
        if (mode != importer.spriteImportMode)
        {
            if (GUILayout.Button("Convert"))
            {
                importer.spriteImportMode = mode;
                importer.SaveAndReimport();
            }
        }
        else
        {
            if (mode == SpriteImportMode.Multiple)
            {
                /*
                if (GUILayout.Button("Launch Sprite Editor"))
                {
                    Selection.activeObject = sprites[0];
                    EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetTypes().Where(t => t.Name == "SpriteEditorWindow").FirstOrDefault());
                }
                */

                if (GUILayout.Button("Generate Multiple Sprites"))
                    GenerateMultiple(sprites[0]);

                if (GUILayout.Button("Layout"))
                    PlaceInScene(sprites[0]);
            }
        }

        // end horizontal row
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Import Settings", EditorStyles.boldLabel);

        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        // label for max size 
        EditorGUILayout.LabelField("Max Size", GUILayout.Width(130));
        size = EditorGUILayout.Popup(size, convertMaxSize);
        if (int.Parse(convertMaxSize[size]) != importer.maxTextureSize)
        {
            if (GUILayout.Button("Apply"))
            {
                importer.maxTextureSize = int.Parse(convertMaxSize[size]);
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
            }
        }
        // end horizontal row
        EditorGUILayout.EndHorizontal();

        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        // label for max size 
        EditorGUILayout.LabelField("Compression", GUILayout.Width(130));
        if (powerOf4 || alreadyPower4)
        {
            compressionQuality = (TextureImporterCompression)EditorGUILayout.EnumPopup(compressionQuality);
            if (compressionQuality != importer.textureCompression)
            {
                if (GUILayout.Button("Apply"))
                {
                    importer.textureCompression = compressionQuality;
                    importer.SaveAndReimport();
                    AssetDatabase.Refresh();
                }
            }
        }
        else
            EditorGUILayout.LabelField("Must be power of 4");
        // end horizontal row
        EditorGUILayout.EndHorizontal();

        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        // label for max size 
        EditorGUILayout.LabelField("Crunch Compression", GUILayout.Width(130));
        if (powerOf4 || alreadyPower4)
        {
            useCrunch = EditorGUILayout.Toggle(useCrunch);
            if (useCrunch != importer.crunchedCompression)
            {
                if (GUILayout.Button("Apply"))
                {
                    importer.crunchedCompression = useCrunch;
                    importer.SaveAndReimport();
                    AssetDatabase.Refresh();
                }
            }
        }
        else
            EditorGUILayout.LabelField("Must be power of 4");
        // end horizontal row
        EditorGUILayout.EndHorizontal();

        if (useCrunch || importer.crunchedCompression)
        {
            // start horizontal row
            EditorGUILayout.BeginHorizontal();
            // label for max size 
            EditorGUILayout.LabelField("Compressor Quality", GUILayout.Width(130));
            if (powerOf4 || alreadyPower4)
            {
                quality = EditorGUILayout.IntSlider(quality, 0, 100);
                if (quality != importer.compressionQuality)
                {
                    if (GUILayout.Button("Apply"))
                    {
                        importer.compressionQuality = quality;
                        importer.SaveAndReimport();
                        AssetDatabase.Refresh();
                    }
                }
            }
            else
                EditorGUILayout.LabelField("Must be power of 4");
            // end horizontal row
            EditorGUILayout.EndHorizontal();
        }

        bool changed = false;
        if (quality != importer.compressionQuality)
        {
            changed = true;
        }
        else if (useCrunch != importer.crunchedCompression)
        {
            changed = true;
        }
        else if (compressionQuality != importer.textureCompression)
        {
            changed = true;
        }
        else if (int.Parse(convertMaxSize[size]) != importer.maxTextureSize)
        {
            changed = true;
        }

        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(130));
        GUI.enabled = changed;
        if (GUILayout.Button("Revert All"))
        {
            quality = importer.compressionQuality;
            useCrunch = importer.crunchedCompression;
            compressionQuality = importer.textureCompression;
            for (int i = 0; i < convertMaxSize.Length; i++)
            {
                if (int.Parse(convertMaxSize[i]) == importer.maxTextureSize)
                    size = i;
            }
        }

        if (GUILayout.Button("Apply All"))
        {
            importer.compressionQuality = quality;
            importer.crunchedCompression = useCrunch;
            importer.textureCompression = compressionQuality;
            importer.compressionQuality = quality;
            importer.maxTextureSize = int.Parse(convertMaxSize[size]);
            importer.SaveAndReimport();
            AssetDatabase.Refresh();
        }

        GUI.enabled = true;
        // end horizontal row
        EditorGUILayout.EndHorizontal();


        GUILayout.Space(20);
        EditorGUILayout.LabelField("Split Settings", EditorStyles.boldLabel);

        // start horizontal row
        EditorGUILayout.BeginHorizontal();
        // label for max size 
        EditorGUILayout.LabelField("Max Size", GUILayout.Width(130));
        splitSize = EditorGUILayout.Popup(splitSize, convertMaxSize);
        // end horizontal row
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Texture Type", GUILayout.Width(130));
        splitImporterType = (TextureImporterType)EditorGUILayout.EnumPopup(splitImporterType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Use Import Settings", GUILayout.Width(130));
        useImportSettings = EditorGUILayout.Toggle(useImportSettings);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Create folder", GUILayout.Width(130));
        createSplitFolder = EditorGUILayout.Toggle(createSplitFolder);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(130));
        if (GUILayout.Button("Apply"))
            SplitSprite(sprites[0]);
        EditorGUILayout.EndHorizontal();

        // end vertical column
        EditorGUILayout.EndVertical();

        Rect rect = EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2));

        Texture2D tex = sprites[0];
        float width = tex.width;
        float height = tex.height;

        if (width > height)
        {
            width = width < (position.width / 2) - 130 ? width : (position.width / 2) - 130;
            float ratio = width / tex.width;
            height = height * ratio;
        }
        else
        {
            height = height < (position.height - 130) ? height : (position.height - 130);
            float ratio = height / tex.height;
            width = width * ratio;
        }

        float texWidth = tex.width;
        float texHeight = tex.height;
        if (powerOf4)
        {
            while (texWidth % 4 != 0)
            {
                texWidth++;
            }

            while (texHeight % 4 != 0)
            {
                texHeight++;
            }
        }

        // draw texture
        if(showBackground)
            EditorGUI.DrawTextureTransparent(new Rect(rect.x + 5, rect.y + 5, width, height), tex);
        else
            GUI.DrawTexture(new Rect(rect.x + 5, rect.y + 5, width, height), tex);

        GUIStyle style = new GUIStyle();
        float w = style.CalcSize(new GUIContent(texWidth + "px")).x;
        float lineW = (width - w) / 2.0f;

        GUI.DrawTexture(new Rect(rect.x + 5, rect.y + height + 10, lineW - 5, 2), CreateTexture(2, 2, Color.black));
        GUI.Label(new Rect(rect.x + 5 + (width / 2) - (w / 2), rect.y + height + 12 - 7.5f, w + 5, 15), texWidth + "px");
        GUI.DrawTexture(new Rect(rect.x + 5 + w + lineW + 5, rect.y + height + 10, lineW - 5, 2), CreateTexture(2, 2, Color.black));

        Vector2 h = style.CalcSize(new GUIContent(texHeight + "px"));
        float lineH = (height - h.y) / 2.0f;

        GUI.DrawTexture(new Rect(rect.x + 10 + width, rect.y + 5, 2, lineH - 5), CreateTexture(2, 2, Color.black));
        GUI.Label(new Rect(rect.x + 5 + width, rect.y + (height / 2) - (h.y/2) + 5, h.x + 5, h.y + 5), texHeight + "px");
        GUI.DrawTexture(new Rect(rect.x + 10 + width, rect.y + 5 + h.y + lineH + 5, 2, lineH - 5), CreateTexture(2, 2, Color.black));

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayBatch()
    {
        SerializedObject so = new SerializedObject(this);
        so.Update();

        EditorGUILayout.BeginHorizontal();
        showList = EditorGUILayout.Foldout(showList, "Textures", true);
        if (GUILayout.Button("Clear Textures", GUILayout.Width(100)))
        {
            sprites.Clear();
            DrawList();
        }
        EditorGUILayout.EndHorizontal();
        if (showList)
        {
            EditorGUILayout.Space();
            list.DoLayoutList();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Sprite Mode");
        updateMode = EditorGUILayout.Toggle(updateMode, GUILayout.Width(40));
        if(updateMode)
            mode = (SpriteImportMode)EditorGUILayout.EnumPopup(mode);
        EditorGUILayout.EndHorizontal();

        if (mode == SpriteImportMode.Multiple)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Auto Splice");
            autoSplice = EditorGUILayout.Toggle(autoSplice, GUILayout.Width(40));
            if (autoSplice)
                EditorGUILayout.LabelField("Caution! This will override any existing splices.");
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Power of 4");
        powerOf4 = EditorGUILayout.Toggle(powerOf4, GUILayout.Width(40));
        if (powerOf4)
            EditorGUILayout.LabelField("Adds white space to make both width and height divisible by 4.");
        EditorGUILayout.EndHorizontal();

        placeInScene = EditorGUILayout.Toggle("Layout In Scene", placeInScene);

        size = EditorGUILayout.Popup("Max Size", size, convertMaxSize);

        compressionQuality = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression", compressionQuality);
        useCrunch = EditorGUILayout.Toggle("Crunch Compression", useCrunch);
        quality = EditorGUILayout.IntSlider("Compressor Quality", quality, 0, 100);

        if (GUILayout.Button("Convert"))
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (powerOf4)
                    MakePowerOf4(sprites[i]);
                if (mode == SpriteImportMode.Multiple && autoSplice)
                    GenerateMultiple(sprites[i]);

                UpdateSettings(sprites[i]);

                if (placeInScene)
                    PlaceInScene(sprites[i]);
            }

            AssetDatabase.Refresh();
        }

        so.ApplyModifiedProperties();
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

    private void MakePowerOf4(Texture2D tex)
    {
        string texPath = AssetDatabase.GetAssetPath(tex);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);

        bool crunch = importer.crunchedCompression = false;
        TextureImporterCompression compression = importer.textureCompression;
        bool readable = importer.isReadable;

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

        importer.crunchedCompression = crunch;
        importer.textureCompression = compression;
        importer.isReadable = readable;
        importer.SaveAndReimport();

        AssetDatabase.Refresh();
    }

    private void PlaceInScene(Texture2D tex)
    {
        //Texture2D tex = (Texture2D)objs[i];
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

    private void GenerateMultiple(Texture2D tex)
    {
        Debug.Log("Generate Multiple: " + tex.name);

        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
        TextureImporterCompression compression = importer.textureCompression;
        bool readable = importer.isReadable;

        importer.isReadable = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spriteImportMode = SpriteImportMode.Multiple;

        importer.SaveAndReimport();
        AssetDatabase.Refresh();

        int minimumSpriteSize = 16;
        int extrudeSize = 0;
        Rect[] rects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles(tex, minimumSpriteSize, extrudeSize);

        List<SpriteMetaData> metas = new List<SpriteMetaData>();
        int rectNum = 0;

        foreach (Rect r in rects)
        {
            SpriteMetaData meta = new SpriteMetaData();
            meta.rect = r;
            meta.name = tex.name + "_" + rectNum++;
            Debug.Log(meta.name);
            metas.Add(meta);
        }

        importer.spritesheet = metas.ToArray();
        //importer.SaveAndReimport();
        //AssetDatabase.Refresh();

        importer.textureCompression = compression;
        importer.isReadable = readable;
        importer.SaveAndReimport();
        AssetDatabase.Refresh();
    }

    private void UpdateSettings(Texture2D tex)
    {
        string texPath = AssetDatabase.GetAssetPath(tex);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);

        if (updateMode)
            importer.spriteImportMode = mode;

        importer.maxTextureSize = int.Parse(convertMaxSize[size]);
        importer.textureCompression = compressionQuality;
        importer.crunchedCompression = useCrunch;
        importer.compressionQuality = quality;
        importer.SaveAndReimport();
    }

    private void UpdateSettings(string path)
    {
        string texPath = path;
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);

        if (updateMode)
            importer.spriteImportMode = mode;

        importer.maxTextureSize = int.Parse(convertMaxSize[size]);
        importer.textureCompression = compressionQuality;
        importer.crunchedCompression = useCrunch;
        importer.compressionQuality = quality;
        importer.SaveAndReimport();
    }

    private void SplitSprite(Texture2D tex)
    {
        string savePath = EditorUtility.OpenFolderPanel("Save Split Folder", "Assets", "");
        Debug.Log(savePath);

        if (createSplitFolder)
        {
            savePath = savePath + "/" + sprites[0].name;
            Directory.CreateDirectory(savePath);
        }

        string texPath = AssetDatabase.GetAssetPath(tex);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
        importer.isReadable = true;
        importer.maxTextureSize = 8192;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        importer.SaveAndReimport();

        int width = tex.width;
        int height = tex.height;

        int _size = int.Parse(convertMaxSize[splitSize]);

        int amountWidth = Mathf.CeilToInt((float)width / (float)_size);
        int amountHeight = Mathf.CeilToInt((float)height / (float)_size);

        Debug.Log(amountWidth + " / " + amountHeight);

        int x = 0;
        List<Rect> rects = new List<Rect>();
        List<Vector2> pivots = new List<Vector2>();
        for (int i = 0; i < amountWidth; i++)
        {
            int y = 0;
            for (int j = 0; j < amountHeight; j++)
            {
                float rectWidth = width - x >= _size ? _size : width - x;
                float rectHeight = height - y >= _size ? _size : height - y;
                Rect rect = new Rect(x, y, rectWidth, rectHeight);
                rects.Add(rect);
                y += _size;
            }
            x += _size;
        }

        Debug.Log(Application.dataPath);

        
        for (int i = 0; i < rects.Count; i++)
        {
            int w = (int)rects[i].width;
            int h = (int)rects[i].height;

            while (w % 4 != 0)
                w++;
            while (h % 4 != 0)
                h++;

            Vector2 pivot = new Vector2();
            pivot.x -= rects[i].x / w;
            pivot.y -= rects[i].y / h;

            Texture2D newTex = new Texture2D(w, h);

            for (int j = 0; j < w; j++)
            {
                for (int k = 0; k < h; k++)
                {
                    newTex.SetPixel(j, k, Color.clear);
                }
            }

            newTex.SetPixels(0, 0, (int)rects[i].width, (int)rects[i].height, tex.GetPixels((int)rects[i].x, (int)rects[i].y, (int)rects[i].width, (int)rects[i].height));
            newTex.Apply();
            string path = savePath + "/" + "texture" + i + ".png";
            File.WriteAllBytes(path, newTex.EncodeToPNG());
            string localPath = path.Replace(Application.dataPath, "Assets");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            TextureImporter _importer = (TextureImporter)AssetImporter.GetAtPath(localPath);
            _importer.isReadable = true;
            //_importer.spriteImportMode = SpriteImportMode.Single;
            TextureImporterSettings settings = new TextureImporterSettings();
            _importer.ReadTextureSettings(settings);
            settings.textureType = TextureImporterType.Default;
            settings.spriteMode = 1;
            settings.spriteAlignment = 9;
            settings.spritePivot = pivot;
            _importer.SetTextureSettings(settings);
            _importer.textureType = splitImporterType;
            _importer.spriteImportMode = SpriteImportMode.Single;
            _importer.SaveAndReimport();

            if(useImportSettings)
                UpdateSettings(localPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
}