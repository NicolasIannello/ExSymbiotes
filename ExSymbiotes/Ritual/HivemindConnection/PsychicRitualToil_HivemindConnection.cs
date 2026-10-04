using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace ExSymbiotes
{
  public class PsychicRitualToil_HivemindConnection : PsychicRitualToil
  {
    public PsychicRitualRoleDef targetRole;
    public PsychicRitualRoleDef invokerRole;

    protected PsychicRitualToil_HivemindConnection() { }

    public PsychicRitualToil_HivemindConnection(PsychicRitualRoleDef invokerRole, PsychicRitualRoleDef targetRole)
    {
      this.targetRole = targetRole;
      this.invokerRole = invokerRole;
    }

    public override void Start(PsychicRitual psychicRitual, PsychicRitualGraph graph)
    {
      PsychicRitualDef_Psychophagy def = (PsychicRitualDef_Psychophagy) psychicRitual.def;
      Pawn invoker = psychicRitual.assignments.FirstAssignedPawn(this.invokerRole);
      Pawn target = psychicRitual.assignments.FirstAssignedPawn(this.targetRole);
      if (invoker == null || target == null) return;
      this.ApplyOutcome(psychicRitual, target);
    }

    private void ApplyOutcome(PsychicRitual psychicRitual, Pawn pawn)
    {
      // Hediff_DeathRefusal hediffDeathRefusal =
        // (Hediff_DeathRefusal)HediffMaker.MakeHediff(HediffDefOf.DeathRefusal, pawn);
      // pawn.health.AddHediff((Hediff)hediffDeathRefusal);

      if (!PawnUtility.ShouldSendNotificationAbout(pawn))
        return;
      Find.LetterStack.ReceiveLetter("PsychicRitualCompleteLabel".Translate((NamedArgument)psychicRitual.def.label),
        "ImbueDeathRefuralCompleteText".Translate((NamedArgument)(Thing)pawn),//customn
        LetterDefOf.NeutralEvent, (LookTargets)(Thing)pawn);
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Defs.Look<PsychicRitualRoleDef>(ref this.targetRole, "targetRole");
    }
  }
}