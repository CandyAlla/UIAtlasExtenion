//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UIBasicSprite), true)]
public class UIBasicSpriteEditor : UIWidgetInspector
{
	/// <summary>
	/// Draw all the custom properties such as sprite type, flip setting, fill direction, etc.
	/// </summary>

	protected override void DrawCustomProperties ()
	{
		GUILayout.Space(6f);

		SerializedProperty sp = NGUIEditorTools.DrawProperty("Type", serializedObject, "mType", GUILayout.MinWidth(20f));
		UIBasicSprite bs = target as UIBasicSprite;

		UISprite.Type type = (UISprite.Type)sp.intValue;

		if (type == UISprite.Type.Simple)
		{
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
			NGUIEditorTools.DrawProperty("DrawRound", serializedObject, "_drawRound");

			if (bs.IsRound)
			{
				NGUIEditorTools.DrawProperty("RoundType", serializedObject, "_customRoundType");
				NGUIEditorTools.DrawProperty("PointCount", serializedObject, "_pointCount");
				NGUIEditorTools.DrawProperty("AngleOffset", serializedObject, "_angleOffset");
			}
		}
		else if (type == UISprite.Type.Tiled)
		{
			NGUIEditorTools.DrawBorderProperty("Trim", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
		}
		else if (type == UISprite.Type.Sliced)
		{
			NGUIEditorTools.DrawBorderProperty("Border", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");

			EditorGUI.BeginDisabledGroup(sp.hasMultipleDifferentValues);
			{
				sp = serializedObject.FindProperty("centerType");
				bool val = (sp.intValue != (int)UISprite.AdvancedType.Invisible);

				if (val != EditorGUILayout.Toggle("Fill Center", val))
				{
					sp.intValue = val ? (int)UISprite.AdvancedType.Invisible : (int)UISprite.AdvancedType.Sliced;
				}
			}
			EditorGUI.EndDisabledGroup();
		}
		else if (type == UISprite.Type.Filled)
		{
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
			NGUIEditorTools.DrawProperty("Fill Dir", serializedObject, "mFillDirection", GUILayout.MinWidth(20f));
			GUILayout.BeginHorizontal();
			GUILayout.Space(4f);
			NGUIEditorTools.DrawProperty("Fill Amount", serializedObject, "mFillAmount", GUILayout.MinWidth(20f));
			GUILayout.Space(4f);
			GUILayout.EndHorizontal();
			NGUIEditorTools.DrawProperty("Invert Fill", serializedObject, "mInvert", GUILayout.MinWidth(20f));
		}
		else if (type == UISprite.Type.Advanced)
		{
			NGUIEditorTools.DrawBorderProperty("Border", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("  Left", serializedObject, "leftType");
			NGUIEditorTools.DrawProperty("  Right", serializedObject, "rightType");
			NGUIEditorTools.DrawProperty("  Top", serializedObject, "topType");
			NGUIEditorTools.DrawProperty("  Bottom", serializedObject, "bottomType");
			NGUIEditorTools.DrawProperty("  Center", serializedObject, "centerType");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
		}
		
		NGUIEditorTools.DrawProperty("MirrorType", serializedObject, "_mirrorType");
		if (UIBasicSprite.DrawMirrorType.None != bs.MirrorType)
		{
			NGUIEditorTools.DrawProperty("OffsetInPixel", serializedObject, "_offsetInPixel");
			if (bs.OffsetInPixel)
            {
                NGUIEditorTools.DrawProperty("MirrorOffset", serializedObject, "_mirrorOffset");
            }
			else
            {
				switch (bs.MirrorType)
                {
					case UIBasicSprite.DrawMirrorType.Horizontal:
                        NGUIEditorTools.DrawProperty("MirrorHOffset", serializedObject, "_mirrorHOffset");
                        break;

					case UIBasicSprite.DrawMirrorType.Vertical:
                        NGUIEditorTools.DrawProperty("MirrorVOffset", serializedObject, "_mirrorVOffset");
						break;

					case UIBasicSprite.DrawMirrorType.Both:
                        NGUIEditorTools.DrawProperty("MirrorHOffset", serializedObject, "_mirrorHOffset");
                        NGUIEditorTools.DrawProperty("MirrorVOffset", serializedObject, "_mirrorVOffset");
                        break;
                }
            }
		}

		if ((type == UIBasicSprite.Type.Simple || type == UIBasicSprite.Type.Sliced) && (!bs.IsRound)) // Gradients get too complicated for tiled and filled.
		{
			GUILayout.BeginHorizontal();
			SerializedProperty gr = NGUIEditorTools.DrawProperty("Gradient", serializedObject, "mApplyGradient", GUILayout.Width(95f));

			EditorGUI.BeginDisabledGroup(!gr.hasMultipleDifferentValues && !gr.boolValue);
			{
				NGUIEditorTools.SetLabelWidth(30f);
				serializedObject.DrawProperty("mGradientTop", "Top", GUILayout.MinWidth(40f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				NGUIEditorTools.SetLabelWidth(50f);
				GUILayout.Space(79f);

				serializedObject.DrawProperty("mGradientBottom", "Bottom", GUILayout.MinWidth(40f));
				NGUIEditorTools.SetLabelWidth(80f);
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}
		base.DrawCustomProperties();
	}
}
