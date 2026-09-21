using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    [StaticConstructorOnStartup]
    public class Graphic_FleckSymbiote : Graphic_Fleck
    {
        public override void DrawFleck(FleckDrawData drawData, DrawBatch batch)
        {
            Color color;
            if (drawData.overrideColor.HasValue)
            {
                color = drawData.overrideColor.Value;
            }
            else
            {
                float alpha = drawData.alpha;
                if ((double) alpha <= 0.0)
                {
                    if (drawData.propertyBlock == null)
                        return;
                    batch.ReturnPropertyBlock(drawData.propertyBlock);
                    return;
                }
                color = this.Color * drawData.color;
                color.a *= alpha;
            }
            Vector3 scale = drawData.scale;
            int growthRate = 50;
            float offset = growthRate * drawData.ageSecs * this.data.drawSize.x * 0.5f;
            drawData.pos -= Quaternion.AngleAxis(drawData.rotation, Vector3.up) * Vector3.right * offset;
            scale.x *= this.data.drawSize.x;
            scale.z = this.data.drawSize.y;
            Mesh mesh = MeshPool.plane10;
            float rotation = drawData.rotation;
            if ((double) scale.x < 0.0 && (double) scale.z >= 0.0)
            {
                scale.x = -scale.x;
                mesh = MeshPool.plane10Flip;
            }
            else if ((double) scale.x >= 0.0 && (double) scale.z < 0.0)
            {
                scale.z = -scale.z;
                mesh = MeshPool.plane10Flip;
                rotation += 180f;
            }
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(drawData.pos, Quaternion.AngleAxis(rotation, Vector3.up), scale);
            Material matSingle = this.MatSingle;
            batch.DrawMesh(mesh, matrix, matSingle, drawData.drawLayer, new Color?(color), this.data.renderInstanced && this.AllowInstancing, drawData.propertyBlock);
        }

    }
}