using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class LordToil_SymbioteStalk : LordToil
    {
        public override void UpdateAllDuties()
        {
            for (int index = 0; index < this.lord.ownedPawns.Count; ++index)
            {
                if (this.lord.ownedPawns[index].mindState.duty?.def != DutyDefOf.ChimeraStalkFlee)
                {
                    this.lord.ownedPawns[index].mindState.duty = new PawnDuty(DutyDefOf.ChimeraStalkFlee);
                    this.lord.ownedPawns[index].jobs.EndCurrentJob(JobCondition.InterruptForced);
                }
            }
        }

        public override void Notify_PawnJobDone(Pawn p, JobCondition condition)
        {
            base.Notify_PawnJobDone(p, condition);
            if (p.mindState.duty?.def != DutyDefOf.ChimeraStalkFlee || p.CurJobDef != JobDefOf.Wait_Wander)
                return;
            p.mindState.duty = new PawnDuty(ExSymbiotesDefOf.ExSymbiotes_SymbioteStalkWander);
        }
    }
}