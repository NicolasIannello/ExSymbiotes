using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbioticConsume : CompAbilityEffect
    {
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            bool isSymbiote = (target.Pawn.def == ExSymbiotesDefOf.ExSymbiotes_Symbiote || target.Pawn.def == ExSymbiotesDefOf.ExSymbiotes_SymbioteRed);
            bool hasSymbiosis = SymbioteUtility.HasSymbiosis(target.Pawn) != null;
            
            if(!hasSymbiosis && !isSymbiote)
                Messages.Message((string) "ExSymbiotes.MessageConsumeSymbiote".Translate(target.Pawn.Named("PAWN")), (LookTargets) (Thing) target.Pawn, MessageTypeDefOf.NeutralEvent);

            return target.Pawn != null && (isSymbiote || hasSymbiosis) && base.CanApplyOn(target, dest);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Hediff symbiosis = SymbioteUtility.HasSymbiosis(target.Pawn);
            if(symbiosis == null)
                target.Pawn.Kill(null);
            else
                target.Pawn.health.RemoveHediff(symbiosis);

            if (symbiosis != null &&
                (symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_SymbioteControl ||
                 symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_SymbioteControlRed ||
                 symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_Thrall))
                return;
                
            this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_HivemindConnection).Severity += 0.05f;
        }
    }
}