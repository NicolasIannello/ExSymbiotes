using RimWorld.Planet;
using Verse;

namespace ExSymbiotes
{
    public class SpaceGrendelMapParent : BasicAsteroidMapParent
    {
        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            alsoRemoveWorldObject = false;
            foreach (Pawn pawn in this.Map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.def == ExSymbiotesDefOf.ExSymbiotes_SymbioteGrendel) return false;
            }
            return base.ShouldRemoveMapNow(out alsoRemoveWorldObject);
        }
    }
}