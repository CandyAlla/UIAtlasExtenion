using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class AtlasMakerExtention : Editor
{
    private static List<string> exceptPath;

    [MenuItem("NGUI/AltasMakerExtention")]
    static void MakeAltas()
    {
        AtlasWindow window = AtlasWindow.GetAtlasWindow(); 
        window.Show();
        
    }

    [MenuItem("Assets/AtlasMaker/Setting")]
    static void SettingAtlas()
    {
        var Select = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(Select); //"Assets/AltasFolder"
        string settingPath = "";
        if (!string.IsNullOrEmpty(path)&&!Path.HasExtension(path)) // 判断路径存在，且没有名字后缀
        {
            Debug.Log("选择到了文件夹");
            string name = Path.GetFileName(path);
            string prefabPath = "Assets/Result/"+name+".prefab";
            GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath,typeof(GameObject)) as GameObject;
            if(null == go)
                go = PrefabUtility.SaveAsPrefabAsset(new GameObject(),prefabPath);
            NGUISettings.atlas = go.AddComponent<UIAtlas>();
            // settingPath ="Assets/Result/" + Path.GetFileName(path)+".asset";
            
            // get folders for edit
            var paths = exceptPath= GetDirectionsFoldersPath(new DirectoryInfo(path));
            AtlasSettingWindow settingWindow = AtlasSettingWindow.GetAtlasSettingWindow(name,paths);
            settingWindow.Show();
        }
        else
        {
            Debug.LogError("Please select a folder!");
        }
    }
    
    [MenuItem("Assets/AtlasMaker/NewAtlas")]
    public static void NewAtlas()
    {
        var Select = Selection.activeObject;

        var path = AssetDatabase.GetAssetPath(Select);

        if (!string.IsNullOrEmpty(path)&&!Path.HasExtension(path)) // 判断路径存在，且没有名字后缀
        {
            Debug.Log("选择到了文件夹");
        }

        string newName = Path.GetFileName(path);
        
        // 读文件
        List<FileInfo> files = GetDirectionsFolders(new DirectoryInfo(path));
        Debug.Log(files.Count);
        CreateAtlas(newName);
        var texures = GetSelectedTextures(files);
        UpdateAtlas(texures, true);
    }
    
    [MenuItem("Assets/AtlasMaker/UpdateAtlas")]
    public static void UpdateAtlas()
    {
        var Select = Selection.activeObject;

        var path = AssetDatabase.GetAssetPath(Select);

        if (!string.IsNullOrEmpty(path)&&!Path.HasExtension(path)) // 判断路径存在，且没有名字后缀
        {
            Debug.Log("选择到了文件夹");
            string newName = Path.GetFileName(path);
            // 读文件
            List<FileInfo> files = GetDirectionsFolders(new DirectoryInfo(path));
            var texures = GetSelectedTextures(files);
            UpdateAtlas(texures, false);
        }
        
    }

    static List<string> GetDirectionsFoldersPath(DirectoryInfo dir)
    {
        List<string> paths = new List<string>();

        DirectoryInfo[] dii = dir.GetDirectories();
        for (int i = 0; i < dii.Length; i++)
        {
            paths.Add(dii[i].FullName);
            var r = GetDirectionsFoldersPath(dii[i]);
            if(r.Count>0)
                paths.AddRange(r);
        }
        return paths;
    }

    static List<FileInfo> GetDirectionsFolders(DirectoryInfo dir)
    {
        List<FileInfo> infos = new List<FileInfo>();

        List<FileInfo> results1 = GetDirectionFiles(dir);
        if(results1.Count>0)
            infos.AddRange(results1);

        DirectoryInfo[] dii = dir.GetDirectories();
        for (int i = 0; i < dii.Length; i++)
        {
            List<FileInfo> result2 = GetDirectionsFolders(dii[i]);
            if(result2.Count>0)
                infos.AddRange(result2);
        }
        
        return infos;
    }
    static List<FileInfo> GetDirectionFiles(DirectoryInfo dir)
    {
        List<FileInfo> infos = new List<FileInfo>();
        var files = dir.GetFiles("*.png");
        return files.ToList();
    }

    static void CreateAtlas(string name)
    {
        string path = "Assets/Result/"+name+".prefab";
        string savePath = System.IO.Path.GetDirectoryName(path);
        NGUISettings.currentPath = savePath;
        GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        string matPath = path.Replace(".prefab", ".mat");
        

        // Try to load the material
        Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

        // If the material doesn't exist, create it
        if (mat == null)
        {
            Shader shader = Shader.Find(NGUISettings.atlasPMA ? "Hidden/Unlit/Premultiplied Colored 1" : "Hidden/Unlit/Transparent Colored 1");
            mat = new Material(shader);

            // Save the material
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // Load the material so it's usable
            mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
        }

        // Create a new prefab for the atlas
        Object prefab = (go != null) ? go : PrefabUtility.SaveAsPrefabAsset(new GameObject(),path);

        // Create a new game object for the atlas
        string atlasName = path.Replace(".prefab", "");
        atlasName = atlasName.Substring(path.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
        go = new GameObject(atlasName);
        go.AddComponent<UIAtlas>().spriteMaterial = mat;

        // Update the prefab
        PrefabUtility.ReplacePrefab(go, prefab);
        DestroyImmediate(go);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        // Select the atlas
        go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        NGUISettings.atlas = go.GetComponent<UIAtlas>();
        Selection.activeGameObject = go;
    }
    
    static List<Texture> GetSelectedTextures (List<FileInfo> infos)
    {
        var textures = new List<Texture>();
        var names = new List<string>();

        if (infos != null && infos.Count> 0)
        {
            var objects = infos;

            foreach (FileInfo o in objects)
            {
                string mp = o.FullName;
                mp = mp.Substring(mp.IndexOf("Assets"));
                mp = mp.Replace('\\', '/');
                var tex =  AssetDatabase.LoadAssetAtPath(mp, typeof(Texture)) as Texture ;
                if (tex == null || tex.name == "Font Texture") continue;
                if (names.Contains(tex.name)) continue;
                var texImporter = AssetImporter.GetAtPath(mp) as TextureImporter;
                if(null == texImporter)
                    continue;
                texImporter.mipmapEnabled = NGUISettings.IsSpriteMipmap;
                AssetDatabase.ImportAsset(mp);
                if (NGUISettings.atlas == null || NGUISettings.atlas.texture != tex)
                {
                    names.Add(tex.name);
                    textures.Add(tex);
                }
            }
        }
        return textures;
    }
    static void UpdateAtlas (List<Texture> textures, bool keepSprites)
    {
        // Create a list of sprites using the collected textures
        List<UIAtlasMaker.SpriteEntry> sprites = UIAtlasMaker.CreateSprites(textures);

        if (sprites.Count > 0)
        {
            // Extract sprites from the atlas, filling in the missing pieces
            if (keepSprites) UIAtlasMaker.ExtractSprites(NGUISettings.atlas, sprites);

            // NOTE: It doesn't seem to be possible to undo writing to disk, and there also seems to be no way of
            // detecting an Undo event. Without either of these it's not possible to restore the texture saved to disk,
            // so the undo process doesn't work right. Because of this I'd rather disable it altogether until a solution is found.

            // The ability to undo this action is always useful
            //NGUIEditorTools.RegisterUndo("Update Atlas", UISettings.atlas, UISettings.atlas.texture, UISettings.atlas.material);

            // Update the atlas
            UIAtlasMaker. UpdateAtlas(NGUISettings.atlas, sprites);
        }
        else if (!keepSprites)
        {
            UIAtlasMaker.UpdateAtlas(NGUISettings.atlas, sprites);
        }
    }
    
    [InitializeOnLoadMethod]
    static void InitializedOnLoadMethod()
    {
        EditorApplication.projectChanged+= delegate
        {
            Debug.Log("change");
        };
    }
}
