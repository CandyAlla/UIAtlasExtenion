using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class AtlasSettingWindow : EditorWindow
{
    UIAtlas mLastAtlas;
    
	struct FolderSetting
    {
	    public  bool isSelected;
	    public  string path;
    }

    private static string path = "Assets/Result";
    private  static AtlasSettingWindow _window;
    
    public static bool showLog= false;
    static List<FolderSetting> allFolder = new List<FolderSetting>();
    static List<FolderSetting> exceptFolder = new List<FolderSetting>();
    public static AtlasSettingWindow GetAtlasSettingWindow(string name,List<string> paths)
    {
	    //todo read json to select folders 
	    for (int i = 0; i < paths.Count; i++)
	    {
		    FolderSetting s = new FolderSetting();
		    var parent = Directory.GetParent(paths[i]).ToString();
		    if (paths.Contains(parent))
		    {
			    
		    }
		    
		    s.isSelected = true;
		    s.path = paths[i];
		    allFolder.Add(s);
	    }
	    exceptFolder.Clear();
        _window = (AtlasSettingWindow)EditorWindow.GetWindow(typeof(AtlasSettingWindow), false, name);
        return _window;
        path += name;
        
    }

    private void OnGUI()
    {
        if (mLastAtlas != NGUISettings.atlas)
        {
            mLastAtlas = NGUISettings.atlas;
        }
        
        

        #region folder setting

        if (allFolder.Count > 0)
        {
	        NGUIEditorTools.DrawHeader("Folder Edit", true);
	        NGUIEditorTools.BeginContents(false);

	        for (int i = 0; i < allFolder.Count; i++)
	        {
		        EditorGUILayout.BeginHorizontal();
		        // EditorGUILayout.Toggle("Trim Alpha", NGUISettings.atlasTrimming, GUILayout.Width(100f));
		        
		        var folderSetting = allFolder[i];
		        bool sel = EditorGUILayout.ToggleLeft(folderSetting.path, folderSetting.isSelected);
		        folderSetting.isSelected = sel;
		        allFolder[i] = folderSetting;
		        if (sel)
		        {
			        //choose
			        if (exceptFolder.Contains(allFolder[i]))
				        exceptFolder.Remove(allFolder[i]);
		        }
		        else
		        {
			        //un choose
			        if(!exceptFolder.Contains(allFolder[i]))
				        exceptFolder.Add(allFolder[i]);
		        }
		        EditorGUILayout.EndHorizontal();
	        }
	        
	        
	        NGUIEditorTools.EndContents();
        }
        

        #endregion
        
      
		#region atlas texutre setting

		NGUIEditorTools.DrawHeader("Atlas Texture Setting", true);
		NGUIEditorTools.BeginContents(true);

		//图集的最大限制尺寸
		GUILayout.BeginHorizontal();
		GUILayout.Label("Max texture size :",GUILayout.Width(150f));
		Vector2 maxSize = NGUISettings.GetTextureMax;
		maxSize.x = EditorGUILayout.IntField("x", (int)maxSize.x,GUILayout.Width(300f));
		maxSize.y = EditorGUILayout.IntField("y", (int)maxSize.y,GUILayout.Width(300f));
		NGUISettings.GetTextureMax = maxSize;
		GUILayout.EndHorizontal();
		
		if (NGUISettings.atlas != null)
		{
			Material mat = NGUISettings.atlas.spriteMaterial;
			Texture tex = NGUISettings.atlas.texture;
			string path = AssetDatabase.GetAssetPath(tex);
			TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;

			if (texture != null)
			{
				//texture type
				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Type:",GUILayout.Width(150f));
				texture.textureType = (TextureImporterType)EditorGUILayout.Popup((int)texture.textureType, System.Enum.GetNames(typeof(TextureImporterType)));
				GUILayout.EndHorizontal();
				
				//texture shape
				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Shape:",GUILayout.Width(150f));
				texture.textureShape = (TextureImporterShape)EditorGUILayout.Popup((int)texture.textureShape, System.Enum.GetNames(typeof(TextureImporterShape)));
				GUILayout.EndHorizontal();
				
				//texture sRGB
				GUILayout.BeginHorizontal();
				texture.sRGBTexture = EditorGUILayout.Toggle("Texture sRGB", texture.sRGBTexture, GUILayout.Width(100f));
				GUILayout.EndHorizontal();
				
				//texture wrap Mode
				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture WrapMode:",GUILayout.Width(150f));
				texture.wrapMode = (TextureWrapMode)EditorGUILayout.Popup((int)texture.wrapMode, System.Enum.GetNames(typeof(TextureWrapMode)));
				GUILayout.EndHorizontal();
			
				//texture filter mode
				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Filter Mode",GUILayout.Width(150f));
				texture.filterMode =
					(FilterMode) EditorGUILayout.Popup((int) texture.filterMode, Enum.GetNames(typeof(FilterMode)));
				GUILayout.EndHorizontal();
			
				//texture mipmap
				GUILayout.BeginHorizontal();
				texture.mipmapEnabled = EditorGUILayout.Toggle("Texture Mipmap", texture.mipmapEnabled, GUILayout.Width(400));
				GUILayout.EndHorizontal();

				if (texture.mipmapEnabled)
				{
					GUILayout.BeginHorizontal();
					texture.borderMipmap = EditorGUILayout.Toggle("Texture borderMipmap", texture.borderMipmap, GUILayout.Width(300));
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("Texture MipMap Filtering",GUILayout.Width(200));
					texture.mipmapFilter =
						(TextureImporterMipFilter) EditorGUILayout.Popup((int) texture.mipmapFilter, Enum.GetNames(typeof(TextureImporterMipFilter)));
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					texture.mipMapsPreserveCoverage = EditorGUILayout.Toggle("Texture mipMapsPreserveCoverage", texture.mipMapsPreserveCoverage, GUILayout.Width(300));
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					texture.fadeout = EditorGUILayout.Toggle("Texture mipMapsfadeout", texture.fadeout, GUILayout.Width(300));
					GUILayout.EndHorizontal();
					
				}

				bool isApply = false;
				isApply = GUILayout.Button("Apply");
				if (isApply)
				{
					texture.SaveAndReimport();
				}
			}
			
			
		}
		NGUIEditorTools.EndContents();

		#endregion
		

		#region sprite settings
		//sprite  设置
		NGUIEditorTools.DrawHeader("sprite setting :", true);
		NGUIEditorTools.BeginContents(true);
		
		//图片是否关闭mipmap
		GUILayout.BeginHorizontal();
		NGUISettings.IsSpriteMipmap= EditorGUILayout.Toggle("sprite Mipmap", NGUISettings.IsSpriteMipmap, GUILayout.Width(400));
		GUILayout.EndHorizontal();
		
		NGUIEditorTools.EndContents();

		#endregion
		
		#region altas maker setting 
		NGUIEditorTools.DrawHeader("Atlas Maker Setting", true);
        NGUIEditorTools.BeginContents(false);
        NGUIEditorTools.SetLabelWidth(84f);
        GUILayout.Space(3f);
        if (NGUISettings.atlas != null)
        {
            Material mat = NGUISettings.atlas.spriteMaterial;
            Texture tex = NGUISettings.atlas.texture;

            // Material information
            GUILayout.BeginHorizontal();
            {
                if (mat != null)
                {
                    if (GUILayout.Button("Material", GUILayout.Width(76f))) Selection.activeObject = mat;
                    GUILayout.Label(" " + mat.name);
                }
                else
                {
                    GUI.color = Color.grey;
                    GUILayout.Button("Material", GUILayout.Width(76f));
                    GUI.color = Color.white;
                    GUILayout.Label(" N/A");
                }
            }
            GUILayout.EndHorizontal();

            // Texture atlas information
            GUILayout.BeginHorizontal();
            {
                if (tex != null)
                {
                    if (GUILayout.Button("Texture", GUILayout.Width(76f))) Selection.activeObject = tex;
                    GUILayout.Label(" " + tex.width + "x" + tex.height);
                }
                else
                {
                    GUI.color = Color.grey;
                    GUILayout.Button("Texture", GUILayout.Width(76f));
                    GUI.color = Color.white;
                    GUILayout.Label(" N/A");
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        NGUISettings.atlasPadding = Mathf.Clamp(EditorGUILayout.IntField("Padding", NGUISettings.atlasPadding, GUILayout.Width(100f)), 0, 8);
        GUILayout.Label((NGUISettings.atlasPadding == 1 ? "pixel" : "pixels") + " between sprites");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        NGUISettings.atlasTrimming = EditorGUILayout.Toggle("Trim Alpha", NGUISettings.atlasTrimming, GUILayout.Width(100f));
        GUILayout.Label("Remove empty space");
        #region SLG
        NGUISettings.alphaThreshold = EditorGUILayout.IntSlider(NGUISettings.alphaThreshold, 0, 20);
        GUILayout.Label("Alpha threshold");
        #endregion
        GUILayout.EndHorizontal();
        
        bool fixedShader = false;

		if (NGUISettings.atlas != null)
		{
			Material mat = NGUISettings.atlas.spriteMaterial;

			if (mat != null)
			{
				Shader shader = mat.shader;

				if (shader != null)
				{
					if (shader.name == "Unlit/Transparent Colored")
					{
						NGUISettings.atlasPMA = false;
						fixedShader = true;
					}
					else if (shader.name == "Unlit/Premultiplied Colored")
					{
						NGUISettings.atlasPMA = true;
						fixedShader = true;
					}
				}
			}
		}

		if (!fixedShader)
		{
			GUILayout.BeginHorizontal();
			NGUISettings.atlasPMA = EditorGUILayout.Toggle("PMA Shader", NGUISettings.atlasPMA, GUILayout.Width(100f));
			GUILayout.Label("Pre-multiplied alpha", GUILayout.MinWidth(70f));
			GUILayout.EndHorizontal();
		}

		//GUILayout.BeginHorizontal();
		//NGUISettings.keepPadding = EditorGUILayout.Toggle("Keep Padding", NGUISettings.keepPadding, GUILayout.Width(100f));
		//GUILayout.Label("or replace with trimmed pixels", GUILayout.MinWidth(70f));
		//GUILayout.EndHorizontal();

		#if !UNITY_5_6
		GUILayout.BeginHorizontal();
		NGUISettings.unityPacking = EditorGUILayout.Toggle("Unity Packer", NGUISettings.unityPacking, GUILayout.Width(100f));
		GUILayout.Label("or custom packer", GUILayout.MinWidth(70f));
		GUILayout.EndHorizontal();
		#endif

		GUILayout.BeginHorizontal();
		NGUISettings.trueColorAtlas = EditorGUILayout.Toggle("Truecolor", NGUISettings.trueColorAtlas, GUILayout.Width(100f));
		GUILayout.Label("force ARGB32 textures", GUILayout.MinWidth(70f));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		NGUISettings.autoUpgradeSprites = EditorGUILayout.Toggle("Auto-upgrade", NGUISettings.trueColorAtlas, GUILayout.Width(100f));
		GUILayout.Label("replace textures with sprites", GUILayout.MinWidth(70f));
		GUILayout.EndHorizontal();

		#if !UNITY_5_6
		if (!NGUISettings.unityPacking)
		{
			GUILayout.BeginHorizontal();
			NGUISettings.forceSquareAtlas = EditorGUILayout.Toggle("Force Square", NGUISettings.forceSquareAtlas, GUILayout.Width(100f));
			GUILayout.Label("if on, forces a square atlas texture", GUILayout.MinWidth(70f));
			GUILayout.EndHorizontal();
		}
		#endif

#if UNITY_IPHONE || UNITY_ANDROID
		GUILayout.BeginHorizontal();
		NGUISettings.allow4096 = EditorGUILayout.Toggle("4096x4096", NGUISettings.allow4096, GUILayout.Width(100f));
		GUILayout.Label("if off, limit atlases to 2048x2048");
		GUILayout.EndHorizontal();
#endif
		NGUIEditorTools.EndContents();
        

        #endregion
       

		GUILayout.BeginHorizontal();
		bool update = false;
		bool create = false;
		create = GUILayout.Button("Add Atlas");
		update = GUILayout.Button("Update Atlas");
		GUILayout.EndHorizontal();
		if (create)
		{
			showLog = false;
			AtlasMakerExtention.NewAtlas();
		}else if (update)
		{
			showLog = false;
			AtlasMakerExtention.UpdateAtlas();
		}

		if (showLog)
		{
			NGUIEditorTools.DrawHeader("Out put", true);
			NGUIEditorTools.BeginContents(true);
			GUILayout.BeginHorizontal();
			string str1 = "Output:The result is in the folder :'Assets/Result',name :" + NGUISettings.atlas.name;
			string str2 = "Output:The sprite mipmap setting:" + NGUISettings.IsSpriteMipmap;
			GUILayout.Label(str1);
			GUILayout.Label(str2);
			
			GUILayout.EndHorizontal();
			NGUIEditorTools.EndContents();
		}

		
    }
}
