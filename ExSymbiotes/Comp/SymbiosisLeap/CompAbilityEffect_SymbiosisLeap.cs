using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbiosisLeap : CompAbilityEffect, ICompAbilityEffectOnJumpCompleted
    {
        public void OnJumpCompleted(IntVec3 origin, LocalTargetInfo target)
        {
            CompSymbiote comp;
            if (!this.parent.pawn.TryGetComp<CompSymbiote>(out comp)) return;
            comp.StartDigesting(origin, target);
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            Pawn pawn = target.Pawn;
            return pawn != null;
        }
    }
}