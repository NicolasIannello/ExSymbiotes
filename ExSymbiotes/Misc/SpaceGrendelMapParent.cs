using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ExSymbiotes
{
    public class SpaceGrendelMapParent : BasicAsteroidMapParent
    {
        public Quest quest;
        
        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            alsoRemoveWorldObject = false;
            if(quest != null && quest.State != QuestState.Ongoing)
                return base.ShouldRemoveMapNow(out alsoRemoveWorldObject);
            foreach (Pawn pawn in this.Map.mapPawns.AllPawnsSpawned)
                if (pawn.def == ExSymbiotesDefOf.ExSymbiotes_SymbioteGrendel) 
                    return false;
            return base.ShouldRemoveMapNow(out alsoRemoveWorldObject);
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.quest, "quest");
        }
    }
}