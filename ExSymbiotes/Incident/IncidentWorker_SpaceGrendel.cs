using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class IncidentWorker_SpaceGrendel : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Current.Game.entityCodex.DiscoveredCount(ExSymbiotesDefOf.Advanced)>14;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            QuestUtility.GenerateQuestAndMakeAvailable(ExSymbiotesDefOf.ExSymbiotes_GrendelAsteroid, 1f);
            return true;
        }
    
    }
}