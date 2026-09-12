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
      private const float StalkToAttackMTBDays = 0.7f;
      private const float AttackToStalkMTBHours = 6f;
      private const float DefendToAttackMTBHours = 10f;
      private const float ChanceToFleeOnDown = 0.0f;
      private const float MinTimeInModeTicks = 7500f;
      private const int MinTicksFleeing = 2500;
      public const string StalkToAttackMemo = "StalkToAttack";
      private const string AttackToStalkMemo = "AttackToStalk";
      private const string ToDefendMemo = "ToDefend";
      private const string DefendToAttackMemo = "DefendToAttackMemo";
      private LordToil_DefendPoint toilChimeraDefend;
      private bool InAttackMode => this.lord.CurLordToil is LordToil_ChimeraAttack;
      private bool InDefendMode => this.lord.CurLordToil is LordToil_DefendPoint;
      private bool CanSwitchMode
      {
        get => (double) (Find.TickManager.TicksGame - this.currentModeStartedTick) > MinTimeInModeTicks;
      }
      
      public override StateGraph CreateGraph()
      {
        StateGraph graph = new StateGraph();
        LordToil_SymbioteStalk toilChimeraStalk = new LordToil_SymbioteStalk();
        graph.StartingToil = (LordToil) toilChimeraStalk;
        LordToil_ChimeraAttack toilChimeraAttack = new LordToil_ChimeraAttack();
        graph.AddToil((LordToil) toilChimeraAttack);
        Transition transition1 = new Transition((LordToil) toilChimeraStalk, (LordToil) toilChimeraAttack);
        transition1.AddPostAction((TransitionAction) new TransitionAction_Custom((Action) (() =>
        {
          this.currentModeStartedTick = Find.TickManager.TicksGame;
          this.SendAttackingLetter();
        })));
        transition1.triggers.Add((Trigger) new Trigger_Memo(StalkToAttackMemo));
        transition1.triggers.Add((Trigger) new LordJob_SymbioteMass.Trigger_SymbioteHarmed(requireInstigatorWithFaction: true, skipDuty: DutyDefOf.ChimeraStalkFlee, minTicks: new int?(MinTicksFleeing)));
        graph.AddTransition(transition1);
        Transition transition2 = new Transition((LordToil) toilChimeraAttack, (LordToil) toilChimeraStalk);
        transition2.AddPostAction((TransitionAction) new TransitionAction_Custom((Action) (() =>
        {
          this.currentModeStartedTick = Find.TickManager.TicksGame;
          this.SendModeChangeMessage((string) "MessageSymbioteWithdrawing".Translate());
        })));
        transition2.triggers.Add((Trigger) new Trigger_Memo(AttackToStalkMemo));
        graph.AddTransition(transition2);
        
        toilChimeraDefend = new LordToil_DefendPoint();
        graph.AddToil((LordToil) toilChimeraDefend);
        Transition transition3 = new Transition((LordToil) toilChimeraAttack, (LordToil) toilChimeraDefend);
        transition3.AddPostAction((TransitionAction) new TransitionAction_Custom((Action) (() =>
        {
          this.currentModeStartedTick = Find.TickManager.TicksGame;
          this.SendModeChangeMessage((string) "MessageSymbioteDefending".Translate());
        })));
        transition3.triggers.Add((Trigger) new Trigger_Memo(ToDefendMemo));
        graph.AddTransition(transition3);
        Transition transition4 = new Transition((LordToil) toilChimeraStalk, (LordToil) toilChimeraDefend);
        transition4.AddPostAction((TransitionAction) new TransitionAction_Custom((Action) (() =>
        {
          this.currentModeStartedTick = Find.TickManager.TicksGame;
          this.SendModeChangeMessage((string) "MessageSymbioteDefending".Translate());
        })));
        transition4.triggers.Add((Trigger) new Trigger_Memo(ToDefendMemo));
        graph.AddTransition(transition4);
        Transition transition5 = new Transition((LordToil) toilChimeraDefend, (LordToil) toilChimeraAttack);
        transition5.AddPostAction((TransitionAction) new TransitionAction_Custom((Action) (() =>
        {
          this.currentModeStartedTick = Find.TickManager.TicksGame;
          this.SendAttackingLetter();
        })));
        transition5.triggers.Add((Trigger) new Trigger_Memo(DefendToAttackMemo));
        transition5.triggers.Add((Trigger) new LordJob_SymbioteMass.Trigger_SymbioteHarmed(requireInstigatorWithFaction: true, skipDuty: DutyDefOf.ChimeraStalkFlee, minTicks: new int?(MinTicksFleeing)));
        graph.AddTransition(transition5);
        
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
        if (this.InAttackMode && Rand.MTBEventOccurs(AttackToStalkMTBHours, 2500f, 1f))
        {
          this.lord.ReceiveMemo(AttackToStalkMemo);
        }
        else if (this.InDefendMode && Rand.MTBEventOccurs(DefendToAttackMTBHours, 2500f, 1f))
        {
          this.lord.ReceiveMemo(DefendToAttackMemo);
        }
        else
        {
          if (this.InAttackMode || !Rand.MTBEventOccurs(StalkToAttackMTBDays, 60000f, 1f))
            return;
          this.lord.ReceiveMemo(StalkToAttackMemo);
        }
      }

      public void SwitchMode()
      {
        if (this.lord == null)
          return;
        switch (this.lord.CurLordToil)
        {
          case LordToil_SymbioteStalk _:
            this.lord.ReceiveMemo(StalkToAttackMemo);
            break;
          case LordToil_ChimeraAttack _:
            this.lord.ReceiveMemo(AttackToStalkMemo);
            break;
          case LordToil_DefendPoint _:
            this.lord.ReceiveMemo(DefendToAttackMemo);
            break;
          default:
            Log.Error($"Symbiote lord job tried switching from a toil which is not handled {this.lord.CurLordToil}");
            break;
        }
        this.currentModeStartedTick = Find.TickManager.TicksGame;
      }

      public void DefendMode(IntVec3 defendPoint)
      {
        if (this.lord == null)
          return;
        
        toilChimeraDefend.SetDefendPoint(defendPoint);
        
        if(this.lord.CurLordToil is LordToil_DefendPoint) toilChimeraDefend.UpdateAllDuties();
        else this.lord.ReceiveMemo(ToDefendMemo);
      }

      public override void Notify_PawnDowned(Pawn p)
      {
        base.Notify_PawnDowned(p);
        if (!this.CanSwitchMode || !Rand.Chance(ChanceToFleeOnDown) || !(this.lord.CurLordToil is LordToil_ChimeraAttack))
          return;
        this.SwitchMode();
      }

      public override void Notify_PawnLost(Pawn p, PawnLostCondition condition)
      {
        if (!this.CanSwitchMode || condition != PawnLostCondition.Killed || !Rand.Chance(ChanceToFleeOnDown) || !(this.lord.CurLordToil is LordToil_ChimeraAttack))
          return;
        this.SwitchMode();
      }

      private void SendModeChangeMessage(string verb)
      {
        if (this.NoActivePawns())
          return;
        Messages.Message((string) (this.lord.ownedPawns.Count > 1 ? "MessageSymbioteModeChangePlural".Translate((NamedArgument) verb) : "MessageSymbioteModeChangeSingular".Translate((NamedArgument) verb)), new LookTargets((IEnumerable<Pawn>) this.lord.ownedPawns), MessageTypeDefOf.NeutralEvent);
      }

      private void SendAttackingLetter()
      {
        if (this.NoActivePawns())
          return;
        Find.LetterStack.ReceiveLetter("LetterSymbiotesAttackingLabel".Translate(), "LetterSymbiotesAttacking".Translate(), LetterDefOf.ThreatBig, new LookTargets((IEnumerable<Pawn>) this.lord.ownedPawns));
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