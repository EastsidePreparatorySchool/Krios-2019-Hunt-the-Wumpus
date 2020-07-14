using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.Selection
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class GuiGradient : BaseMeshEffect
    {
        public Color mColor1 = Color.white;
        public Color mColor2 = Color.white;
        [Range(-180f, 180f)]
        public float mAngle;
        public bool mIgnoreRatio = true;

        public override void ModifyMesh(VertexHelper vh)
        {
            if(enabled)
            {
                Rect rect = graphic.rectTransform.rect;
                Vector2 dir = GuiGradientTools.RotationDir(mAngle);

                if (!mIgnoreRatio)
                    dir = GuiGradientTools.CompensateAspectRatio(rect, dir);

                GuiGradientTools.Matrix2X3 localPositionMatrix = GuiGradientTools.LocalPositionMatrix(rect, dir);

                UIVertex vertex = default(UIVertex);
                for (int i = 0; i < vh.currentVertCount; i++) {
                    vh.PopulateUIVertex (ref vertex, i);
                    Vector2 localPosition = localPositionMatrix * vertex.position;
                    vertex.color *= Color.Lerp(mColor2, mColor1, localPosition.y);
                    vh.SetUIVertex (vertex, i);
                }
            }
        }
    }
}

