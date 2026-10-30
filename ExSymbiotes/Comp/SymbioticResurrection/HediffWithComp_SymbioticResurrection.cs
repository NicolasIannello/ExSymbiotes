using ExSymbiotes.Utils;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ExSymbiotes
{

  public class HediffWithComp_SymbioticResurrection : HediffWithComps
  {
    private Effecter riseEffecter;
    private Sustainer riseSustainer;
    public float headRotation;
    private float resurrectTimer = -99999;

    public override void PostMake()
    {
      base.PostMake();
      this.headRotation = Rand.RangeSeeded(-20f, 20f, this.pawn.thingIDNumber);
      this.resurrectTimer = Find.TickManager.TicksGame + 120000 - 15;
    }

    public override void TickInterval(int delta)
    {
      base.TickInterval(delta);
      if (Find.TickManager.TicksGame > this.resurrectTimer)
        this.FinishRising();
      if (!this.pawn.Spawned)
        return;
      if (Find.TickManager.TicksGame > this.resurrectTimer - 15)
        this.riseSustainer?.End();
      else
      {
        if (this.riseSustainer == null || this.riseSustainer.Ended)
        {
          SoundInfo info = SoundInfo.InMap((TargetInfo)(Thing)this.pawn, MaintenanceType.PerTick);
          this.riseSustainer = SoundDefOf.Pawn_Shambler_Rise.TrySpawnSustainer(info);
        }

        if (this.riseEffecter == null)
          this.riseEffecter = EffecterDefOf.ShamblerRaise.Spawn((Thing)this.pawn, this.pawn.Map);
        if (this.pawn.Drawer.renderer.CurAnimation != AnimationDefOf.ShamblerRise)
          this.pawn.Drawer.renderer.SetAnimation(AnimationDefOf.ShamblerRise);
        this.riseSustainer.Maintain();
        this.riseEffecter.EffectTick((TargetInfo)(Thing)this.pawn, TargetInfo.Invalid);
      }

      if (Rand.MTBEventOccurs(1f, 60f, 1f))
        FleckMaker.ThrowShamblerParticles((Thing)this.pawn);
      if (this.pawn.IsBurning() && !this.pawn.Downed && Rand.MTBEventOccurs(45f, 60f, 1f))
      {
        this.pawn.GetAttachment(ThingDefOf.Fire).Destroy();
        this.pawn.records.Increment(RecordDefOf.FiresExtinguished);
      }
    }

    public override void PostRemoved()
    {
      base.PostRemoved();
      this.riseSustainer?.End();
      this.pawn.Drawer.renderer.SetAnimation((AnimationDef)null);
    }

    private void FinishRising(bool stun = true)
    {
      this.riseSustainer?.End();
      this.pawn.health.RemoveHediff(this);
      if (this.pawn.ParentHolder is Pawn_CarryTracker parentHolder)
      {
        parentHolder.TryDropCarriedThing(parentHolder.pawn.Position, ThingPlaceMode.Near, out Thing _);
        parentHolder.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
      }

      this.pawn.Drawer.renderer.SetAnimation((AnimationDef)null);

      Hediff symbiosis = SymbioteUtility.HasSymbiosis(this.pawn, false);
      ThingDef filth = ThingDefOf.Filth_RevenantBloodPool;
      if (symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Red ||
          symbiosis.def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Purple)
        filth = ExSymbiotesDefOf.ExSymbiotes_Filth_RevenantBloodPool_Red;
      FilthMaker.TryMakeFilth(this.pawn.PositionHeld, this.pawn.MapHeld, filth);
      HediffComp_Symbiosis symbiosisComp = symbiosis.TryGetComp<HediffComp_Symbiosis>();
      int chance = Rand.RangeInclusive(1, 4);
      TaggedString textResult;
      LetterDef letterDef;
      string symbioteName = symbiosisComp.name;
      int symbioteBond = symbiosisComp.bond;
      this.pawn.health.RemoveHediff(symbiosis);
      if (symbioteBond == 1 && chance == 1)
      {
        letterDef = LetterDefOf.NegativeEvent;
        textResult = "ExSymbiotes.SymbioticResurrectionTextFailed".Translate(symbioteName, this.pawn.LabelShort);
        this.pawn.Kill(null);
      }
      else
      {
        letterDef = LetterDefOf.PositiveEvent;
        textResult = "ExSymbiotes.SymbioticResurrectionTextSuccess".Translate(symbioteName, this.pawn.LabelShort);
        this.pawn.needs.mood.thoughts.memories.TryGainMemory(ExSymbiotesDefOf.ExSymbiotes_BondedSymbioteSacrifice);
      }

      Find.LetterStack.ReceiveLetter("ExSymbiotes.SymbioticResurrectionLabel".Translate(), textResult, letterDef,
        (LookTargets)(Thing)this.pawn);
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look<float>(ref this.resurrectTimer, "resurrectTimer");
      Scribe_Values.Look<float>(ref this.headRotation, "headRotation");
    }
  }
}