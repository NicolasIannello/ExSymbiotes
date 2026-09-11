using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNodeWorker_SpasticSymbiote : PawnRenderNodeWorker
    {
        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 vector3 = base.OffsetFor(node, parms, out pivot);
            PawnRenderNode_SpasticSymbiote.SpasmData dat;
            float progress;
            if (node is PawnRenderNode_SpasticSymbiote renderNodeSpastic && renderNodeSpastic.CheckAndDoSpasm(parms, out dat, out progress))
                vector3 += Vector3.Lerp(dat.offsetStart, dat.offsetTarget, progress);
            return vector3;
        }
        
        public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Quaternion quaternion = base.RotationFor(node, parms);
            if (!(node is PawnRenderNode_SpasticSymbiote renderNodeSpastic))
                return quaternion;
            float ang = 0.0f;
            if (node.Props is PawnRenderNodeProperties_SpasticSymbiote props && props.rotateFacing)
                ang += parms.facing.AsAngle;
            PawnRenderNode_SpasticSymbiote.SpasmData dat;
            float progress;
            if (renderNodeSpastic.CheckAndDoSpasm(parms, out dat, out progress))
                ang += Mathf.Lerp(dat.rotationStart, dat.rotationTarget, progress);
            return quaternion * ang.ToQuat();
        }
        
        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Vector3 a = Vector3.one;
            a.x *= node.Props.drawSize.x * Mathf.Max(parms.pawn.BodySize * 0.6f, node.debugScale);
            a.z *= node.Props.drawSize.y * Mathf.Max(parms.pawn.BodySize * 0.6f, node.debugScale);
            if (!parms.flags.FlagSet(PawnRenderFlags.Portrait))
            {
                Vector3 offset;
                if (node.TryGetAnimationScale(parms, out offset))
                    a = a.ScaledBy(offset);
                GraphicStateDef graphicState = this.GetGraphicState(node, parms);
                Graphic graphic;
                if (graphicState != null && graphicState.TryGetDefaultGraphic(out graphic))
                    a = a.ScaledBy(new Vector3(graphic.drawSize.x, 1f, graphic.drawSize.y));
            }
            if (node.Props.drawData != null)
                a *= node.Props.drawData.ScaleFor(parms.pawn);
            
            PawnRenderNode_SpasticSymbiote.SpasmData dat;
            float progress;
            if (node is PawnRenderNode_SpasticSymbiote renderNodeSpastic && renderNodeSpastic.CheckAndDoSpasm(parms, out dat, out progress))
            {
                a *= Mathf.Lerp(dat.scaleStart, dat.scaleTarget, progress);
                a.y = 1f;
            }
            return a;
        }
    }
}