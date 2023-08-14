using System;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace PixelPerfectUI
{
    public class RectangleDrawable : MaskableGraphic
    {
        public float CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = value;
                SetVerticesDirty();
            }
        }
        [SerializeField] internal float cornerRadius = 0;
        
        public float BorderWidth
        {
            get => borderWidth;
            set
            {
                borderWidth = value;
                SetVerticesDirty();
            }
        }
        [SerializeField] internal float borderWidth = 0;

        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                SetMaterialDirty();
            }
        }
        [SerializeField] internal Color borderColor = Color.white;

        static readonly int MainColorProperty = Shader.PropertyToID("_MainColor");
        static readonly int BorderColorProperty = Shader.PropertyToID("_BorderColor");
        static readonly int BorderWidthProperty = Shader.PropertyToID("_BorderWidth");
        static readonly int CornerRadiusProperty = Shader.PropertyToID("_CornerRadius");
        static readonly int RectWidthProperty = Shader.PropertyToID("_RectWidth");
        static readonly int RectHeightProperty = Shader.PropertyToID("_RectHeight");

        internal const float Threshold = float.Epsilon * 10;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Rect rect = GetPixelAdjustedRect();
            float width = rect.width;
            float height = rect.height;
            float minSize = Mathf.Min(width, height);
            float cornerRadiusClamped = Mathf.Clamp(cornerRadius, 0, minSize / 2);
            
            vh.Clear();
            
            AddVertex(-width / 2, -height / 2, -1, -1); // 0
            AddVertex(-width / 2, height / 2, -1, 1); // 1
            AddVertex(width / 2, height / 2, 1, 1); // 2
            AddVertex(width / 2, -height / 2, 1, -1); // 3
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
            
            material.SetFloat(BorderWidthProperty, borderWidth);
            material.SetFloat(CornerRadiusProperty, cornerRadiusClamped);
            material.SetFloat(RectWidthProperty, width);
            material.SetFloat(RectHeightProperty, height);

            void AddVertex(float x, float y, float uvX, float uvY)
            {
                UIVertex vertex = UIVertex.simpleVert;
                vertex.position = new (x, y);
                vertex.uv0 = new (uvX, uvY, 1, 1);
                vh.AddVert(vertex);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (material == null || material.shader.name != "PPUI/Rectangle")
                material = new (Shader.Find("PPUI/Rectangle"));
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
            material.SetColor(MainColorProperty, color);
            material.SetColor(BorderColorProperty, borderColor);
        }
    }
}