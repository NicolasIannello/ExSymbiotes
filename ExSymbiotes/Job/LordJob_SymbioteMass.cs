using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class LordJob_SymbioteMass : LordJob
    {
      private int currentModeStartedTick;
      private const float StalkToAttackMTBDays = 0.2f;
      private const float AttackToStalkMTBHours = 4f;
      private const float ChanceToFleeOnDown = 0.075f;
      private const float MinTimeInModeTicks = 7500f;
      private const int MinTicksFleeing = 2500;
      public const string StalkToAttackMemo = "StalkToAttack";
      private const string AttackToStalkMemo = "AttackToStalk";

      private bool InAttackMode => this.lord.CurLordToil is LordToil_ChimeraAttack;

      private bool CanSwitchMode
      {
        get => (double) (Find.TickManager.TicksGame - this.currentModeStartedTick) > 7500.0;
      }

      public override StateGraph CreateGraph()
      {
        StateGraph graph = new StateGraph();
        LordToil_ChimeraStalk toilChimeraStalk = new LordToil_ChimeraStalk();
        graph.StartingToil = (LordToil) toilChimeraStalk;
        LordToil_ChimeraAttack toilChimeraAttack = new LordToil_ChimeraAttack();
        graph.AddToil((LordToil) toilChimeraAttack);
        Transition transition1 = new Transition((LordToil) toilChimeraStalk, (LordToil) toilChimeraAttack);
        transition1.AddPostAction((TransitionAction) new TransitionAction_Custom((Action) (() =>
        {
          this.currentModeStartedTick = Find.TickManager.TicksGame;
          this.SendAttackingLetter();
        })));
        transition1.triggers.Add((Trigger) new Trigger_Memo("StalkToAttack"));
        transition1.triggers.Add((Trigger) new LordJob_SymbioteMass.Trigger_SymbioteHarmed(requireInstigatorWithFaction: true, skipDuty: DutyDefOf.ChimeraStalkFlee, minTicks: new int?(2500)));
        graph.AddTransition(transition1);
        Transition transition2 = new Transition((LordToil) toilChimeraAttack, (LordToil) toilChimeraStalk);
        transition2.AddPostAction((TransitionAction) new TransitionAction_Custom((Action) (() =>
        {
          this.currentModeStartedTick = Find.TickManager.TicksGame;
          this.SendModeChangeMessage((string) "MessageChimeraWithdrawing".Translate());
        })));
        transition2.triggers.Add((Trigger) new Trigger_Memo("AttackToStalk"));
        graph.AddTransition(transition2);
        return graph;
      }

      public override bool ShouldRemovePawn(Pawn p, PawnLostCondition reason)
      {
        return reason != PawnLostCondition.Incapped;
      }

      public override void LordJobTick()
      {
        base.LordJobTick();
        if (!this.CanSwitchMode)
          return;
        if (this.InAttackMode && Rand.MTBEventOccurs(8f, 2500f, 1f))
        {
          this.lord.ReceiveMemo("AttackToStalk");
        }
        else
        {
          if (this.InAttackMode || !Rand.MTBEventOccurs(0.7f, 60000f, 1f))
            return;
          this.lord.ReceiveMemo("StalkToAttack");
        }
      }

      public void SwitchMode()
      {
        if (this.lord == null)
          return;
        switch (this.lord.CurLordToil)
        {
          case LordToil_ChimeraStalk _:
            this.lord.ReceiveMemo("StalkToAttack");
            break;
          case LordToil_ChimeraAttack _:
            this.lord.ReceiveMemo("AttackToStalk");
            break;
          default:
            Log.Error($"Chimera lord job tried switching from a toil which is not handled {this.lord.CurLordToil}");
            break;
        }
        this.currentModeStartedTick = Find.TickManager.TicksGame;
      }

      public override void Notify_PawnDowned(Pawn p)
      {
        base.Notify_PawnDowned(p);
        if (!this.CanSwitchMode || !Rand.Chance(0.075f) || !(this.lord.CurLordToil is LordToil_ChimeraAttack))
          return;
        this.SwitchMode();
      }

      public override void Notify_PawnLost(Pawn p, PawnLostCondition condition)
      {
        if (!this.CanSwitchMode || condition != PawnLostCondition.Killed || !Rand.Chance(0.075f) || !(this.lord.CurLordToil is LordToil_ChimeraAttack))
          return;
        this.SwitchMode();
      }

      private void SendModeChangeMessage(string verb)
      {
        if (this.NoActivePawns())
          return;
        Messages.Message((string) (this.lord.ownedPawns.Count > 1 ? "MessageChimeraModeChangePlural".Translate((NamedArgument) verb) : "MessageChimeraModeChangeSingular".Translate((NamedArgument) verb)), new LookTargets((IEnumerable<Pawn>) this.lord.ownedPawns), MessageTypeDefOf.NeutralEvent);
      }

      private void SendAttackingLetter()
      {
        if (this.NoActivePawns())
          return;
        Find.LetterStack.ReceiveLetter("LetterChimerasAttackingLabel".Translate(), "LetterChimerasAttacking".Translate(), LetterDefOf.ThreatBig, new LookTargets((IEnumerable<Pawn>) this.lord.ownedPawns));
      }

      private bool NoActivePawns()
      {
        if (this.lord.ownedPawns.Count == 0)
          return true;
        bool flag = false;
        foreach (Pawn ownedPawn in this.lord.ownedPawns)
          flag = !ownedPawn.Downed;
        return !flag;
      }

      public override void ExposeData()
      {
        base.ExposeData();
        Scribe_Values.Look<int>(ref this.currentModeStartedTick, "currentModeStartedTick");
      }

      private class Trigger_SymbioteHarmed : Trigger_PawnHarmed
      {
        public Trigger_SymbioteHarmed(
          float chance = 1f,
          bool requireInstigatorWithFaction = false,
          Faction requireInstigatorWithSpecificFaction = null,
          DutyDef skipDuty = null,
          int? minTicks = null)
          : base(
            chance,
            requireInstigatorWithFaction,
            requireInstigatorWithSpecificFaction,
            skipDuty,
            minTicks)
        {
        }
        
        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
          return (!(lord.LordJob is LordJob_SymbioteMass lordJob) || lordJob.CanSwitchMode) && base.ActivateOn(lord, signal);
        }
      }
    }
}