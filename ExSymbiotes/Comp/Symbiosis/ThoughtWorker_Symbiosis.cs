using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class ThoughtWorker_Symbiosis : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            Hediff firstHediffOfDef = SymbioteUtility.HasSymbiosis(p, false);
            if (firstHediffOfDef?.def.stages == null)
                return ThoughtState.Inactive;
            return ThoughtState.ActiveAtStage(firstHediffOfDef.TryGetComp<HediffComp_Symbiosis>().GetStage());
        }

        public override string PostProcessDescription(Pawn p, string description)
        {
            string str = base.PostProcessDescription(p, description);
            Hediff firstHediffOfDef = SymbioteUtility.HasSymbiosis(p, false);
            return firstHediffOfDef == null || !firstHediffOfDef.Visible ? str : (string) (str + "\n\n" + "CausedBy".Translate() + ": " + firstHediffOfDef.LabelBase.CapitalizeFirst());
        }
    }
}