using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
    public class JobGiver_SymbioteAttackNearbyHumans : ThinkNode_JobGiver
    {
        public float attackRadius = 10f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.mindState?.duty?.def == DutyDefOf.ChimeraStalkFlee)
                return (Job) null;
            List<Pawn> humanlikeSpawned = pawn.Map.mapPawns.AllHumanlikeSpawned;
            for (int index = 0; index < humanlikeSpawned.Count; ++index)
            {
                if (!humanlikeSpawned[index].DeadOrDowned && pawn.Position.InHorDistOf(humanlikeSpawned[index].Position, this.attackRadius))
                    return JobMaker.MakeJob(ExSymbiotesDefOf.ExSymbiotes_SymbioteSwitchToAttackMode);
            }
            return (Job) null;
        }
    }
}