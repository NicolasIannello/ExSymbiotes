using System;
using System.Collections.Generic;
using ExSymbiotes.Utils;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExSymbiotes;

public class JobDriver_RemoveSymbiote : JobDriver
{
  private const TargetIndex CorpseInd = TargetIndex.A;
  public const int RemoveTicks = 1200;
  protected Corpse Corpse => (Corpse) this.job.GetTarget(TargetIndex.A).Thing;

  public override bool TryMakePreToilReservations(bool errorOnFailed)
  {
    return this.pawn.Reserve((LocalTargetInfo) (Thing) this.Corpse, this.job, errorOnFailed: errorOnFailed);
  }

  public override string GetReport()
  {
    return (string) "ExSymbiotes.ReportRemovingSymbiote".Translate();
  }

  protected override IEnumerable<Toil> MakeNewToils()
  {
    this.FailOnDestroyedOrNull<JobDriver_RemoveSymbiote>(TargetIndex.A);
    Hediff symbiosis = SymbioteUtility.HasSymbiosis(this.Corpse.InnerPawn, false);
    this.FailOn<JobDriver_RemoveSymbiote>((Func<bool>) (() => SymbioteUtility.HasSymbiosis(this.Corpse.InnerPawn, false)==null));
    yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
    Toil toil1 = Toils_General.Wait(RemoveTicks, TargetIndex.A).WithProgressBarToilDelay(TargetIndex.A).WithEffect((Func<EffecterDef>) (() => EffecterDefOf.Surgery), TargetIndex.A).PlaySustainerOrSound(SoundDefOf.Recipe_Surgery).PlaySoundAtEnd(SoundDefOf.FleshmassHeart_Throb);
    toil1.handlingFacing = true;
    yield return toil1;
    Toil toil2 = ToilMaker.MakeToil(nameof (MakeNewToils));
    toil2.initAction = (Action) (() =>
    {
      Pawn innerPawn = this.Corpse.InnerPawn;
      List<Hediff> hediffs = innerPawn.health.hediffSet.hediffs;
      for (int index = hediffs.Count - 1; index >= 0; --index)
      {
        if (hediffs[index].def == symbiosis.def)
          innerPawn.health.RemoveHediff(hediffs[index]);
        //remove revive one
      }
      ThingDef coreDef = ExSymbiotesDefOf.ExSymbiotes_SymbioticCore;
      if (symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Red) coreDef = ExSymbiotesDefOf.ExSymbiotes_SymbioticCore_Red;
      if (symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_White) coreDef = ExSymbiotesDefOf.ExSymbiotes_SymbioticCore_White;
      if (symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Yellow) coreDef = ExSymbiotesDefOf.ExSymbiotes_SymbioticCore_Yellow;
      if (symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Purple) coreDef = ExSymbiotesDefOf.ExSymbiotes_SymbioticCore_Purple;
      Thing core = ThingMaker.MakeThing(coreDef);
      GenPlace.TryPlaceThing(core, this.Corpse.Position, this.Map, ThingPlaceMode.Near);
    });
    toil2.defaultCompleteMode = ToilCompleteMode.Instant;
    yield return toil2;
  }
}