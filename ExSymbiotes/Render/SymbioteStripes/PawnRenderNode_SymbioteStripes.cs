using UnityEngine;
using Verse;

namespace ExSymbiotes
{

    public class PawnRenderNode_SymbioteStripes : PawnRenderNode_Body
    {
        public PawnRenderNode_SymbioteStripes(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(
            pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            string maskPath = pawn.story?.bodyType?.bodyNakedGraphicPath;
            return GraphicDatabase.Get<Graphic_Multi>(TexPathFor(pawn), ShaderDatabase.CutoutSkinOverlay, Vector2.one,
                ColorFor(pawn), Color.white, null, maskPath);
        }
    }
}