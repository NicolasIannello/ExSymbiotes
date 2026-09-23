using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNode_VenomLeap : PawnRenderNode
    {
        public PawnRenderNode_VenomLeap(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree) { }
        
        public override Graphic GraphicFor(Pawn pawn)
        {
            HediffComp_VenomLeap comp;
            if (!hediff.TryGetComp<HediffComp_VenomLeap>(out comp)) return base.GraphicFor(pawn);
            string path = this.TexPathFor(pawn);
            if (comp.Landing) path += "_Landing";
            return GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, Vector2.one, this.ColorFor(pawn));
        }
    }
}