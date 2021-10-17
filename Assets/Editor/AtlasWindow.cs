using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class  AtlasWindow : EditorWindow
{
   private  static AtlasWindow _window;
   public static AtlasWindow GetAtlasWindow()
   {
      _window = (AtlasWindow)EditorWindow.GetWindow(typeof(AtlasWindow), false, "NGUIMaker");
      return _window;
   }
}
