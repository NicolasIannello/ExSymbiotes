using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbioticConsume : CompAbilityEffect
    {
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return target.Pawn != null && (target.Pawn.def==ExSymbiotesDefOf.ExSymbiotes_Symbiote || target.Pawn.def==ExSymbiotesDefOf.ExSymbiotes_SymbioteRed) && base.CanApplyOn(target, dest);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            target.Pawn.Kill(null);

            this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_HivemindConnection).Severity += 0.05f;
        }
    }
}