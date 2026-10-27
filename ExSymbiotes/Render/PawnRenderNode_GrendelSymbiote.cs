using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNode_GrendelSymbiote : PawnRenderNode_AnimalPart_Body
    {
        public PawnRenderNode_GrendelSymbiote(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree) { }
        
        public override Graphic GraphicFor(Pawn pawn)
        {
            CompGrendelSymbiote comp;
            if (!pawn.TryGetComp<CompGrendelSymbiote>(out comp) || !comp.flying)
                return base.GraphicFor(pawn);
            Graphic graphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
            return GraphicDatabase.Get<Graphic_Multi>(graphic.path + "_Flying", ShaderDatabase.Cutout, graphic.drawSize, Color.white);
        }
    }
}