using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbiosisCost : CompAbilityEffect
    {
      public CompProperties_AbilitySymbiosisCost Props2 => (CompProperties_AbilitySymbiosisCost) this.props;

      private bool HasEnoughSymbiosis
      {
        get
        {
          Hediff symbiosis = this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis);
          if (symbiosis != null)
          {
            HediffComp_Symbiosis comp = symbiosis.TryGetComp<HediffComp_Symbiosis>();
            return comp.Energy >= (double)this.Props2.symbiosisCost;
          }
          return false;
        }
      }

      public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
      {
        base.Apply(target, dest);
        Hediff symbiosis = this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis);
        if (symbiosis != null)
        {
          HediffComp_Symbiosis comp = symbiosis.TryGetComp<HediffComp_Symbiosis>();
          comp.RemoveSymbiosis(this.Props2.symbiosisCost);
        }
      }

      public override bool GizmoDisabled(out string reason)
      {
        Hediff symbiosis = this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis);
        if (symbiosis == null)
        {
          reason = (string) "ExSymbiotes.AbilityDisabledNoSymbiosisHediff".Translate((NamedArgument) (Thing) this.parent.pawn);
          return true;
        }
        HediffComp_Symbiosis comp = symbiosis.TryGetComp<HediffComp_Symbiosis>();
        if ((double) comp.Energy < (double) this.Props2.symbiosisCost)
        {
          reason = (string) "ExSymbiotes.AbilityDisabledNoSymbiosis".Translate((NamedArgument) (Thing) this.parent.pawn);
          return true;
        }
        float num = this.Props2.symbiosisCost + this.TotalSymbiosisCostOfQueuedAbilities();
        if ((double) this.Props2.symbiosisCost > 1.401298464324817E-45 && (double) num > (double) comp.Energy)
        {
          reason = (string) "ExSymbiotes.AbilityDisabledNoSymbiosis".Translate((NamedArgument) (Thing) this.parent.pawn);
          return true;
        }
        reason = (string) null;
        return false;
      }

      public override bool AICanTargetNow(LocalTargetInfo target) => this.HasEnoughSymbiosis;

      private float TotalSymbiosisCostOfQueuedAbilities()
      {
        double num1;
        if (!(this.parent.pawn.jobs?.curJob?.verbToUse is Verb_CastAbility verbToUse1))
        {
          num1 = 0.0;
        }
        else
        {
          Ability ability = verbToUse1.ability;
          num1 = ability != null ? ability.CompOfType<CompAbilityEffect_SymbiosisCost>().Props2.symbiosisCost : 0.0;
        }
        float num2 = (float) num1;
        if (this.parent.pawn.jobs != null)
        {
          for (int index = 0; index < this.parent.pawn.jobs.jobQueue.Count; ++index)
          {
            if (this.parent.pawn.jobs.jobQueue[index].job.verbToUse is Verb_CastAbility verbToUse2)
            {
              double num3 = (double) num2;
              Ability ability = verbToUse2.ability;
              double num4 = ability != null ? ability.CompOfType<CompAbilityEffect_SymbiosisCost>().Props2.symbiosisCost : 0.0;
              num2 = (float) (num3 + num4);
            }
          }
        }
        return num2;
      }
    }
}