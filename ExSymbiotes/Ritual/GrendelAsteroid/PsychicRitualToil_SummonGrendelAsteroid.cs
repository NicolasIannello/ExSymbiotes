using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class PsychicRitualToil_SummonGrendelAsteroid : PsychicRitualToil
    {
        private PsychicRitualRoleDef invokerRole;

        protected PsychicRitualToil_SummonGrendelAsteroid() { }

        public PsychicRitualToil_SummonGrendelAsteroid(PsychicRitualRoleDef invokerRole)
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
            Find.Storyteller.incidentQueue.Add(ExSymbiotesDefOf.ExSymbiotes_SpaceGrendel, Find.TickManager.TicksGame, new IncidentParms()
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