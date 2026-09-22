using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNodeWorker_VenomLeap : PawnRenderNodeWorker
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            HediffComp_VenomLeap comp;
            if (!node.hediff.TryGetComp<HediffComp_VenomLeap>(out comp))
                return false;
            return base.CanDrawNow(node, parms) && comp.GetStage();
        }
    }
}