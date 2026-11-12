using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbioticRenewal : CompAbilityEffect
    {
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            bool isThrall = SymbioteUtility.HasSymbiosis(target.Pawn)?.def == ExSymbiotesDefOf.ExSymbiotes_Thrall;
            
            if(!isThrall)
                Messages.Message((string) "ExSymbiotes.MessageRenewalSymbiote".Translate(target.Pawn.Named("PAWN")), (LookTargets) (Thing) target.Pawn, MessageTypeDefOf.NeutralEvent);

            return target.Pawn != null && isThrall && base.CanApplyOn(target, dest);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Hediff symbiosis = SymbioteUtility.HasSymbiosis(target.Pawn);
            symbiosis.Severity = 0.01f;
        }
    }
}