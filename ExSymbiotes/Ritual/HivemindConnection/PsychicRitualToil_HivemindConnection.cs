using RimWorld;
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
      PsychicRitualDef_HivemindConnection def = (PsychicRitualDef_HivemindConnection) psychicRitual.def;
      Pawn invoker = psychicRitual.assignments.FirstAssignedPawn(this.invokerRole);
      Pawn target = psychicRitual.assignments.FirstAssignedPawn(this.targetRole);
      if (invoker == null || target == null) return;
      this.ApplyOutcome(psychicRitual, target);
    }

    private void ApplyOutcome(PsychicRitual psychicRitual, Pawn pawn)
    {
      pawn.health.AddHediff(ExSymbiotesDefOf.ExSymbiotes_HivemindConnection);

      if (!PawnUtility.ShouldSendNotificationAbout(pawn))
        return;
      Find.LetterStack.ReceiveLetter("PsychicRitualCompleteLabel".Translate((NamedArgument)psychicRitual.def.label),
        "ExSymbiotes.HivemindConnectionCompleteText".Translate((NamedArgument)(Thing)pawn),
        LetterDefOf.NeutralEvent, (LookTargets)(Thing)pawn);
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Defs.Look<PsychicRitualRoleDef>(ref this.targetRole, "targetRole");
    }
  }
}