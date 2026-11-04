using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompGrendelSymbiote : ThingComp
    {
      public bool flying = false;
      public List<IntVec3> cocoons = new List<IntVec3>();
      
      public override void PostExposeData()
      {
        base.PostExposeData();
        Scribe_Values.Look<bool>(ref flying, "flying");
        Scribe_Values.Look<List<IntVec3>>(ref cocoons, "cocoons");
      }

      public void ExplodeCocoons()
      {
        if(cocoons.Count > 0)
        {
          EffecterDefOf.ChimeraRage.SpawnAttached(this.parent, this.parent.Map).Cleanup();
          ExSymbiotesDefOf.AgonyPulseExplosion.SpawnAttached(this.parent, this.parent.Map).Cleanup();
          for (int i = cocoons.Count - 1; i >= 0; i--)
          {
            List<Thing> things = cocoons[i].GetThingList(this.parent.Map);
            for (int j = things.Count - 1; j >= 0; j--)
              if (things[j] is Building_SymbioteCocoon cocoon)
                cocoon.Explode((Pawn)this.parent);
          }
        }
      }
    }
}