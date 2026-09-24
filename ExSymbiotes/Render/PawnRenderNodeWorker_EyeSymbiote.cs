using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNodeWorker_EyeSymbiote : PawnRenderNodeWorker_Eye
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            return base.CanDrawNow(node, parms) && (ExSymbiotesMod.Visual == SymbiosisVisual.Always || (parms.pawn.Drafted && ExSymbiotesMod.Visual == SymbiosisVisual.Drafted));
        }
    }
}