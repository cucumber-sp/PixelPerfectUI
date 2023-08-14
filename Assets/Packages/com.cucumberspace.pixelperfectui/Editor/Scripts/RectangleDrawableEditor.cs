using System;
using UnityEditor;
using UnityEngine;

namespace PixelPerfectUI
{
    [CustomEditor(typeof(RectangleDrawable))]
    [CanEditMultipleObjects]
    public class RectangleDrawableEditor : Editor
    {
        bool raycastFoldoutOpen = false;

        public override void OnInspectorGUI()
        {
            RectangleDrawable drawable = target as RectangleDrawable;
            if (drawable == null)
                return;
            

            float minSize = Mathf.Min(drawable.rectTransform.rect.width, drawable.rectTransform.rect.height) / 2;
            float cornerRadius = UnityEditor.EditorGUILayout.Slider("Corner Radius", drawable.cornerRadius, 0, minSize);
            if (Math.Abs(cornerRadius - drawable.cornerRadius) > RectangleDrawable.Threshold)
            {
                UnityEditor.Undo.RecordObject(drawable, "Change Corner Radius");
                drawable.cornerRadius = cornerRadius;
                drawable.SetVerticesDirty();
                UnityEditor.EditorUtility.SetDirty(drawable);
            }

            Color color = UnityEditor.EditorGUILayout.ColorField("Color", drawable.color);
            if (color != drawable.color)
            {
                UnityEditor.Undo.RecordObject(drawable, "Change Color");
                drawable.color = color;
                drawable.SetMaterialDirty();
                UnityEditor.EditorUtility.SetDirty(drawable);
            }

            Color borderColor = UnityEditor.EditorGUILayout.ColorField("Border Color", drawable.borderColor);
            if (borderColor != drawable.borderColor)
            {
                UnityEditor.Undo.RecordObject(drawable, "Change Border Color");
                drawable.borderColor = borderColor;
                drawable.SetMaterialDirty();
                UnityEditor.EditorUtility.SetDirty(drawable);
            }

            float borderWidth = UnityEditor.EditorGUILayout.Slider("Border Width", drawable.borderWidth, 0, minSize);
            if (Math.Abs(borderWidth - drawable.borderWidth) > RectangleDrawable.Threshold)
            {
                UnityEditor.Undo.RecordObject(drawable, "Change Border Width");
                drawable.borderWidth = borderWidth;
                drawable.SetVerticesDirty();
                UnityEditor.EditorUtility.SetDirty(drawable);
            }

            bool raycastTarget = UnityEditor.EditorGUILayout.Toggle("Raycast Target", drawable.raycastTarget);
            if (raycastTarget != drawable.raycastTarget)
            {
                UnityEditor.Undo.RecordObject(drawable, "Change Raycast Target");
                drawable.raycastTarget = raycastTarget;
                UnityEditor.EditorUtility.SetDirty(drawable);
            }

            // Drawing raycast padding as foldout with 4 float fields
            Vector4 raycastPadding = drawable.raycastPadding;
            raycastFoldoutOpen = UnityEditor.EditorGUILayout.Foldout(raycastFoldoutOpen, "Raycast Padding");
            if (raycastFoldoutOpen)
            {
                UnityEditor.EditorGUI.indentLevel++;
                raycastPadding.x = UnityEditor.EditorGUILayout.FloatField("Left", raycastPadding.x);
                raycastPadding.w = UnityEditor.EditorGUILayout.FloatField("Bottom", raycastPadding.y);
                raycastPadding.y = UnityEditor.EditorGUILayout.FloatField("Right", raycastPadding.z);
                raycastPadding.z = UnityEditor.EditorGUILayout.FloatField("Top", raycastPadding.w);
                UnityEditor.EditorGUI.indentLevel--;
            }

            if (raycastPadding != drawable.raycastPadding)
            {
                UnityEditor.Undo.RecordObject(drawable, "Change Raycast Padding");
                drawable.raycastPadding = raycastPadding;
                UnityEditor.EditorUtility.SetDirty(drawable);
            }

            bool maskable = UnityEditor.EditorGUILayout.Toggle("Maskable", drawable.maskable);
            if (maskable != drawable.maskable)
            {
                UnityEditor.Undo.RecordObject(drawable, "Change Maskable");
                drawable.maskable = maskable;
                UnityEditor.EditorUtility.SetDirty(drawable);
                drawable.RecalculateMasking();
                drawable.RecalculateClipping();
            }
            // Saving changes to the object
            if (GUI.changed)
            {
                UnityEditor.EditorUtility.SetDirty(drawable);
            }
        }
    }
}