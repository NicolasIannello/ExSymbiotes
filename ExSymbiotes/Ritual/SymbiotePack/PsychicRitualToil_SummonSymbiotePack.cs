using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class PsychicRitualToil_SummonSymbiotePack : PsychicRitualToil
    {
        private PsychicRitualRoleDef invokerRole;

        protected PsychicRitualToil_SummonSymbiotePack() { }

        public PsychicRitualToil_SummonSymbiotePack(PsychicRitualRoleDef invokerRole)
        {
            this.invokerRole = invokerRole;
        }

        public override void Start(PsychicRitual psychicRitual, PsychicRitualGraph parent)
        {
            base.Start(psychicRitual, parent);
            Pawn invoker = psychicRitual.assignments.FirstAssignedPawn(this.invokerRole);
            psychicRitual.ReleaseAllPawnsAndBuildings();
            if (invoker == null)
                return;
            
            this.ApplyOutcome(psychicRitual, invoker);
        }

        private void ApplyOutcome(PsychicRitual psychicRitual, Pawn invoker)
        {
            IncidentDef pack = Rand.Chance(1f / 5f) ? ExSymbiotesDefOf.ExSymbiotes_SymbiotePackRed : ExSymbiotesDefOf.ExSymbiotes_SymbiotePack;
            Find.Storyteller.incidentQueue.Add(pack, Find.TickManager.TicksGame + Rand.RangeInclusive(2500, 7500), new IncidentParms()
            {
                target = (IIncidentTarget) invoker.Map,
                pointMultiplier = StorytellerUtility.DefaultThreatPointsNow((IIncidentTarget) invoker.Map),
                forced = true
            });
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<PsychicRitualRoleDef>(ref this.invokerRole, "invokerRole");
        }
    }
}