using ExSymbiotes.Utils;
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
            return Check(target.Pawn);
        }
        
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return Check(target.Pawn) && base.Valid(target, throwMessages);
        }

        private static bool Check(Pawn pawn)
        {
            if(pawn==null || pawn.def==ExSymbiotesDefOf.ExSymbiotes_Symbiote || pawn.def==ExSymbiotesDefOf.ExSymbiotes_SymbioteRed) return false;

            Hediff hediff = SymbioteUtility.HasSymbiosis(pawn);
            if(hediff==null) return true;
                
            if (hediff.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis || hediff.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Red || hediff.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_White)
            {
                int chance = Rand.RangeInclusive(1, 10);
                return chance <= 2;
            }

            return hediff.def != ExSymbiotesDefOf.ExSymbiotes_SymbioteControl && hediff.def != ExSymbiotesDefOf.ExSymbiotes_SymbioteControlRed;
        }
    }
}