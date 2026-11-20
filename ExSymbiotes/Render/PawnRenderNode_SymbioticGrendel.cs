using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNode_SymbioticGrendel : PawnRenderNode
    {
        public PawnRenderNode_SymbioticGrendel(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree) { }
        
        public override Graphic GraphicFor(Pawn pawn)
        {
            if (!((HediffWithComps_SymbioticGrendel)hediff).flying) return base.GraphicFor(pawn);
            string path = this.TexPathFor(pawn)+"_Flying";
            return GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, Vector2.one, this.ColorFor(pawn));
        }
    }
}