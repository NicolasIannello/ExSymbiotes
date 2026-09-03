using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
    public class CompSymbiote : ThingComp, IThingHolder
    {
      private ThingOwner<Thing> innerContainer;
      private int ticksDigesting;
      private int ticksToDigestFully;
      private bool wasDrafted;
      
      public CompProperties_Symbiote Props => (CompProperties_Symbiote) this.props;

      public Thing DigestingThing
      {
        get
        {
          return this.innerContainer.InnerListForReading.Count <= 0 ? (Thing) null : this.innerContainer.InnerListForReading[0];
        }
      }

      public Pawn DigestingPawn
      {
        get
        {
          Thing digestingThing = this.DigestingThing;
          if (digestingThing == null)
            return (Pawn) null;
          return digestingThing is Corpse corpse ? corpse.InnerPawn : digestingThing as Pawn;
        }
      }

      public bool Digesting => this.DigestingThing != null;

      public Pawn Pawn => this.parent as Pawn;

      public void GetChildHolders(List<IThingHolder> outChildren)
      {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, (IList<Thing>) this.GetDirectlyHeldThings());
      }

      public ThingOwner GetDirectlyHeldThings() => (ThingOwner) this.innerContainer;

      public CompSymbiote()
      {
        this.innerContainer = new ThingOwner<Thing>((IThingHolder) this, LookMode.Deep, false);
      }

      public override void CompTick()
      {
        if (this.Digesting)
          ++this.ticksDigesting;
        if (!this.Digesting || !this.DigestingPawn.Dead)
          return;
        this.CompleteDigestion();
      }

      public override string CompInspectStringExtra()
      {
        if (!this.Digesting)
          return (string) null;
        int digestionTicks = this.GetDigestionTicks();
        int ticksLeftThisToil = this.Pawn.jobs.curDriver.ticksLeftThisToil;
        int num1 = ticksLeftThisToil < 0 ? digestionTicks : ticksLeftThisToil;
        float num2 = (float) (digestionTicks - (digestionTicks - num1)) / 60f;
        return (string) this.Props.digestingInspector.Formatted(this.DigestingThing.Named("PAWN"), num2.Named("SECONDS"));
      }

      public override void Notify_Downed() => this.AbortDigestion(this.Pawn.MapHeld);

      public override void Notify_Killed(Map prevMap, DamageInfo? _ = null)
      {
        this.AbortDigestion(prevMap);
      }

      public void DigestJobFinished()
      {
        if (this.Pawn.BeingTransportedOnGravship)
          return;
        if (this.ticksDigesting >= this.ticksToDigestFully)
          this.CompleteDigestion();
        else
          this.AbortDigestion(this.Pawn.MapHeld);
      }

      public override void PostSwapMap()
      {
        if (!this.Digesting)
          return;
        this.Pawn.jobs.StartJob(JobMaker.MakeJob(ExSymbiotesDefOf.ExSymbiotes_SymbioteDigest), JobCondition.InterruptForced);
      }

      private void AbortDigestion(Map map)
      {
        if (!this.Digesting)
          return;
        Pawn subject = this.DropPawn(map);
        Find.BattleLog.Add((LogEntry) new BattleLogEntry_Event((Thing) subject, RulePackDefOf.Event_DevourerDigestionAborted, (Thing) this.Pawn));
        if (subject.Faction == Faction.OfPlayer)
        {
          string str = this.Pawn.Dead ? this.Props.messageEmergedCorpse : this.Props.messageEmerged;
          if (!str.NullOrEmpty())
            Messages.Message((string) str.Formatted(subject.Named("PAWN")), (LookTargets) (Thing) subject, MessageTypeDefOf.NeutralEvent);
        }
        this.EndDigestingJob();
        this.Pawn.Drawer.renderer.SetAllGraphicsDirty();
        if (this.Pawn.Drawer.renderer.CurAnimation != AnimationDefOf.DevourerDigesting)
          return;
        this.Pawn.Drawer.renderer.SetAnimation((AnimationDef) null);
      }

      private void CompleteDigestion()
      {
        if (!this.Digesting)
          return;
        Pawn subject = this.DropPawn(this.Pawn.MapHeld);
        Find.BattleLog.Add((LogEntry) new BattleLogEntry_Event((Thing) subject, RulePackDefOf.Event_DevourerDigestionCompleted, (Thing) this.Pawn));
        if (!this.Props.messageDigestionCompleted.NullOrEmpty() && !subject.Dead && subject.Faction == Faction.OfPlayer)
          Messages.Message((string) this.Props.messageDigestionCompleted.Formatted(subject.Named("PAWN")), (LookTargets) (Thing) subject, MessageTypeDefOf.NegativeEvent);
        this.Pawn.Drawer.renderer.SetAllGraphicsDirty();
        if (this.Pawn.Drawer.renderer.CurAnimation != AnimationDefOf.DevourerDigesting)
          return;
        this.Pawn.Drawer.renderer.SetAnimation((AnimationDef) null);
        Hediff hediff = subject.health.AddHediff(ExSymbiotesDefOf.ExSymbiotes_SymbioteControl);
        HediffComp_SymbioteControl comp = hediff.TryGetComp<HediffComp_SymbioteControl>();
        comp.AddThing(this.Pawn);
      }

      public void StartDigesting(IntVec3 origin, LocalTargetInfo target)
      {
        if (!target.HasThing || !(target.Thing is Pawn thing) || !thing.Spawned)
        {
          this.Pawn.abilities.GetAbility(ExSymbiotesDefOf.ExSymbiotes_SymbiosisLeap).ResetCooldown();
        }
        else
        {
          if (thing.drafter != null)
            this.wasDrafted = thing.drafter.Drafted;
          thing.DeSpawn(DestroyMode.Vanish);
          this.ticksDigesting = 0;
          this.innerContainer.TryAdd((Thing) thing, true);
          this.ticksToDigestFully = this.GetDigestionTicks();
          this.Pawn.jobs.StartJob(JobMaker.MakeJob(ExSymbiotesDefOf.ExSymbiotes_SymbioteDigest), JobCondition.InterruptForced);
          if (!this.Props.messageDigested.NullOrEmpty() && thing.Faction == Faction.OfPlayer)
            Messages.Message((string) this.Props.messageDigested.Formatted(thing.Named("PAWN")), (LookTargets) (Thing) this.Pawn, MessageTypeDefOf.NegativeEvent);
          this.Pawn.Rotation = Rot4.FromAngleFlat((this.parent.Position - origin).AngleFlat);
          this.Pawn.Drawer.renderer.SetAllGraphicsDirty();
          if (this.Pawn.Drawer.renderer.CurAnimation != AnimationDefOf.DevourerDigesting)
            this.Pawn.Drawer.renderer.SetAnimation(AnimationDefOf.DevourerDigesting);
          Find.BattleLog.Add((LogEntry) new BattleLogEntry_Event((Thing) thing, RulePackDefOf.Event_DevourerConsumeLeap, (Thing) this.Pawn));
        }
      }

      private Pawn DropPawn(Map map)
      {
        if (!this.Digesting)
          return (Pawn) null;
        Thing lastResultingThing;
        if (!this.innerContainer.TryDrop(this.DigestingThing, this.Pawn.PositionHeld, map, ThingPlaceMode.Near, out lastResultingThing, (Action<Thing, int>) null, (Predicate<IntVec3>) null))
        {
          IntVec3 result;
          if (RCellFinder.TryFindRandomCellNearWith(this.Pawn.PositionHeld, (Predicate<IntVec3>) (c => c.Standable(map)), map, out result, 1))
          {
            lastResultingThing = GenSpawn.Spawn(this.innerContainer.Take(this.DigestingThing), result, map);
          }
          else
          {
            Debug.LogError((object) "Could not drop controlling pawn from symbiote!");
            return (Pawn) null;
          }
        }
        if (lastResultingThing is Corpse corpse)
          return corpse.InnerPawn;
        Pawn pawn = (Pawn) lastResultingThing;
        pawn.stances.stunner.StunFor(10, (Thing) this.Pawn, false, false);
        if (pawn.drafter != null)
          pawn.drafter.Drafted = this.wasDrafted;
        return pawn;
      }

      public int GetDigestionTicks()
      {
        return this.DigestingThing == null ? 0 : this.Props.digestTime * 60;
      }

      private void EndDigestingJob()
      {
        if (this.Pawn.Dead || this.Pawn.CurJobDef != ExSymbiotesDefOf.ExSymbiotes_SymbioteDigest || this.Pawn.jobs.curDriver == null || this.Pawn.jobs.curDriver.ended)
          return;
        this.Pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
      }

      public override void PostExposeData()
      {
        base.PostExposeData();
        Scribe_Values.Look<int>(ref this.ticksDigesting, "ticksDigesting");
        Scribe_Values.Look<int>(ref this.ticksToDigestFully, "ticksToDigestFully");
        Scribe_Values.Look<bool>(ref this.wasDrafted, "wasDrafted");
        Scribe_Deep.Look<ThingOwner<Thing>>(ref this.innerContainer, "innerContainer", (object) this);
        if (Scribe.mode != LoadSaveMode.PostLoadInit || !this.innerContainer.removeContentsIfDestroyed)
          return;
        this.innerContainer.removeContentsIfDestroyed = false;
      }
    }
}