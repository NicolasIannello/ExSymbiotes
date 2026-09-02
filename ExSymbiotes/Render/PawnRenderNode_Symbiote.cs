using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNode_Symbiote : PawnRenderNode_AnimalPart_Body
    {
        public PawnRenderNode_Symbiote(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree) { }
        
        public override Graphic GraphicFor(Pawn pawn)
        {
            CompSymbiote comp;
            if (!pawn.TryGetComp<CompSymbiote>(out comp) || !comp.Digesting)
                return base.GraphicFor(pawn);
            Graphic graphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
            return GraphicDatabase.Get<Graphic_Multi>(graphic.path + "_Closed", ShaderDatabase.Cutout, graphic.drawSize, Color.white);
        }
    }
}