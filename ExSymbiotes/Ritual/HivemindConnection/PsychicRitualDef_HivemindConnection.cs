using System.Collections.Generic;
using RimWorld;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class PsychicRitualDef_HivemindConnection : PsychicRitualDef_InvocationCircle
    {

        public override List<PsychicRitualToil> CreateToils(
            PsychicRitual psychicRitual,
            PsychicRitualGraph parent)
        {
            List<PsychicRitualToil> toils = base.CreateToils(psychicRitual, parent);
            toils.Add((PsychicRitualToil) new PsychicRitualToil_HivemindConnection(this.invokerRole, this.TargetRole));
            toils.Add((PsychicRitualToil) new PsychicRitualToil_TargetCleanup(this.InvokerRole, this.TargetRole));
            return toils;
        }
    }
}