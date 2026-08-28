using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
    public class CompSymbioticMassDeactivationInteractor : CompInteractable
    {
      private CompObelisk obeliskComp;

      private CompObelisk ObeliskComp
      {
        get => this.obeliskComp ?? (this.obeliskComp = this.parent.GetComp<CompObelisk>());
      }

      private CompProperties_SymbioticMassDeactivationInteractor Props2
      {
        get => (CompProperties_SymbioticMassDeactivationInteractor) this.props;
      }

      public override string ExposeKey => "Deactivation";

      public override bool CanCooldown => false;

      public override AcceptanceReport CanInteract(Pawn activateBy = null, bool checkOptionalItems = true)
      {
        if (!this.ObeliskComp.StudyFinished || this.ObeliskComp.Activated || this.ObeliskComp.ActivityComp.Deactivated)
          return (AcceptanceReport) false;
        if (activateBy != null)
        {
          if (checkOptionalItems && !activateBy.HasReserved(ThingDefOf.Shard) && !ReservationUtility.ExistsUnreservedAmountOfDef(this.parent.MapHeld, ThingDefOf.Shard, Faction.OfPlayer, this.Props2.shardsRequired, (Predicate<Thing>) (t => activateBy.CanReserveAndReach((LocalTargetInfo) t, PathEndMode.Touch, Danger.None))))
            return (AcceptanceReport) "ExSymbiotes.SymbioticMassMutation".Translate((NamedArgument) this.Props2.shardsRequired);
        }
        else if (checkOptionalItems && !ReservationUtility.ExistsUnreservedAmountOfDef(this.parent.MapHeld, ThingDefOf.Shard, Faction.OfPlayer, this.Props2.shardsRequired))
          return (AcceptanceReport) "ExSymbiotes.SymbioticMassMutation".Translate((NamedArgument) this.Props2.shardsRequired);
        return base.CanInteract(activateBy, checkOptionalItems);
      }

      public override IEnumerable<Gizmo> CompGetGizmosExtra()
      {
        if (this.ObeliskComp.StudyFinished && !this.ObeliskComp.Activated && !this.ObeliskComp.ActivityComp.Deactivated)
        {
          foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            yield return gizmo;
        }
      }

      public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
      {
        if (this.ObeliskComp.StudyFinished && !this.ObeliskComp.Activated && !this.ObeliskComp.ActivityComp.Deactivated)
        {
          foreach (FloatMenuOption compFloatMenuOption in base.CompFloatMenuOptions(selPawn))
            yield return compFloatMenuOption;
        }
      }

      public override void OrderForceTarget(LocalTargetInfo target)
      {
        if (!this.ValidateTarget(target, false))
          return;
        this.OrderDeactivation(target.Pawn);
      }

      private void OrderDeactivation(Pawn pawn)
      {
        List<Thing> fixedIngredientCount = HaulAIUtility.FindFixedIngredientCount(pawn, ThingDefOf.Shard, this.Props2.shardsRequired);
        if (fixedIngredientCount.NullOrEmpty<Thing>())
          return;
        Job job = JobMaker.MakeJob(JobDefOf.InteractThing, (LocalTargetInfo) (Thing) this.parent, (LocalTargetInfo) fixedIngredientCount[0]);
        job.targetQueueB = fixedIngredientCount.Skip<Thing>(1).Select<Thing, LocalTargetInfo>((Func<Thing, LocalTargetInfo>) (i => new LocalTargetInfo(i))).ToList<LocalTargetInfo>();
        job.count = this.Props2.shardsRequired;
        job.playerForced = true;
        job.interactableIndex = 1;
        pawn.jobs.TryTakeOrderedJob(job);
      }

      protected override void OnInteracted(Pawn caster)
      {
        if (this.obeliskComp.Activated)
          return;
        this.obeliskComp.ActivityComp.Deactivate();
        this.parent.GetComp<CompObeliskTriggerInteractor>()?.ResetCooldown();
        ((CompSymbioteMass_Black)this.ObeliskComp).SpawnRedMass();
      }
    }
}