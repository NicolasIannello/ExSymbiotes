using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbioticEnthrall : CompAbilityEffect
    {
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            bool isSymbiote = (target.Pawn.def == ExSymbiotesDefOf.ExSymbiotes_Symbiote || target.Pawn.def == ExSymbiotesDefOf.ExSymbiotes_SymbioteRed);
            bool hasSymbiosis = SymbioteUtility.HasSymbiosis(target.Pawn) != null;
            
            if(isSymbiote)
                Messages.Message((string) "ExSymbiotes.MessageThrallSymbiote".Translate(), (LookTargets) (Thing) target.Pawn, MessageTypeDefOf.NeutralEvent);
            if(hasSymbiosis)
                Messages.Message((string) "ExSymbiotes.MessageThrallSymbiosis".Translate(), (LookTargets) (Thing) target.Pawn, MessageTypeDefOf.NeutralEvent);
            
            return target.Pawn != null && !isSymbiote && !hasSymbiosis && base.CanApplyOn(target, dest);
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