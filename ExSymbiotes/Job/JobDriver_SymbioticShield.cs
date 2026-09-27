using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
    public class JobDriver_SymbioticShield : JobDriver_CastAbility
    {
      private const int DurationTicks = 60;
      private MechShield mechShield;
      private CompSymbioticInterceptor projectileInterceptor;
      private CompSymbioticInterceptor ProjectileInterceptor
      {
        get
        {
          if (this.projectileInterceptor == null && this.mechShield != null)
            this.projectileInterceptor = this.mechShield.GetComp<CompSymbioticInterceptor>();
          return this.projectileInterceptor;
        }
      }

      protected override IEnumerable<Toil> MakeNewToils()
      {
        foreach (Toil makeNewToil in base.MakeNewToils())
          yield return makeNewToil;
        Toil toil = ToilMaker.MakeToil(nameof (MakeNewToils));
        toil.initAction = (Action) (() =>
        {
          this.pawn.pather.StopDead();
          this.mechShield = (MechShield) GenSpawn.Spawn(ExSymbiotesDefOf.ExSymbiotes_MechShield, this.pawn.Position, this.pawn.Map);
          this.mechShield.SetTarget(this.pawn);
          int statValue = 500;
          this.ProjectileInterceptor.maxHitPointsOverride = new int?(statValue);
          this.ProjectileInterceptor.currentHitPoints = statValue;
        });
        toil.PlaySustainerOrSound(SoundDefOf.ShieldMech);
        yield return toil;
      }

      public override void ExposeData()
      {
        base.ExposeData();
        Scribe_References.Look<MechShield>(ref this.mechShield, "mechShield");
      }
    }
}