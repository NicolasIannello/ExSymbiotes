using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbioticEnthrall : CompAbilityEffect
    {
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return target.Pawn != null && target.Pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Thrall)==null && base.CanApplyOn(target, dest);
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