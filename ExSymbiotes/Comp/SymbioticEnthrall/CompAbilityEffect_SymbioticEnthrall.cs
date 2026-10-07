using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbioticEnthrall : CompAbilityEffect
    {
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return target.Pawn != null && SymbioteUtility.HasSymbiosis(target.Pawn)==null && base.CanApplyOn(target, dest) &&
                target.Pawn.def!=ExSymbiotesDefOf.ExSymbiotes_Symbiote && target.Pawn.def!=ExSymbiotesDefOf.ExSymbiotes_SymbioteRed;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            target.Pawn.health.AddHediff(ExSymbiotesDefOf.ExSymbiotes_Thrall).
                TryGetComp<HediffComp_SymbioteThrall>().hivemind=this.parent.pawn;

            ((HediffWithComps_Hivemind)this.parent.pawn.health.hediffSet.
                GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_HivemindConnection)).controlledPawns.Add(target.Pawn);
            MoteMaker.MakeThoughtBubble(this.parent.pawn, "Things/Pawn/Symbiote/Attachments/SymbioteSpiral/SymbioteSpiral_south");
        }
    }
}