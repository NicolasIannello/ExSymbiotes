using RimWorld;
using Verse;
using Verse.AI.Group;
using System.Collections.Generic;

namespace ExSymbiotes
{
    public class PsychicRitualDef_SummonSymbioticMass : PsychicRitualDef_InvocationCircle
    {

        public override List<PsychicRitualToil> CreateToils(
            PsychicRitual psychicRitual, PsychicRitualGraph graph)
        {
            List<PsychicRitualToil> toils = base.CreateToils(psychicRitual, graph);
            toils.Add((PsychicRitualToil) new PsychicRitualToil_SummonSymbioticMass(this.InvokerRole));
            return toils;
        }

        public override IEnumerable<string> BlockingIssues(
            PsychicRitualRoleAssignments assignments,
            Map map)
        {
            foreach (string blockingIssue in base.BlockingIssues(assignments, map))
                yield return blockingIssue;
            if (map.listerThings.ThingsOfDef(ExSymbiotesDefOf.ExSymbiotes_SymbioteMass_Black).Count > 0 || map.listerThings.ThingsOfDef(ExSymbiotesDefOf.ExSymbiotes_SymbioteMassIncoming).Count > 0)
                yield return (string) "PitGateAlreadyExists".Translate();
        }
    }
}