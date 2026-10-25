using RimWorld;

namespace ExSymbiotes
{
    public class IncidentWorker_SpaceGrendel : IncidentWorker
    {

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            QuestUtility.GenerateQuestAndMakeAvailable(ExSymbiotesDefOf.ExSymbiotes_GrendelAsteroid, 1f);
            return true;
        }
    
    }
}