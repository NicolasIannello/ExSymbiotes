using System.Collections.Generic;
using System.Linq;
using ExSymbiotes.Utils;
using RimWorld;
using Verse;
using Verse.AI.Group;

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
        Scribe_Collections.Look<IntVec3>(ref cocoons, "cocoons", LookMode.Value);
        if (Scribe.mode != LoadSaveMode.PostLoadInit || this.cocoons != null)
          return;
        cocoons = new List<IntVec3>();
      }

      public void ExplodeCocoons()
      {
        List<Lord> lords = this.parent.Map.lordManager.lords;
        if (lords.Count > 0)
        {
          for (int i = lords.Count-1; i >= 0; i--)
          {
            if(lords[i].faction != this.parent.Faction) continue;

            List<Pawn> pawns = lords[i].ownedPawns.ToList();
            for (int j = 0; j < pawns.Count; j++)
            {
              HediffComp_SymbioteControl hediffComp = SymbioteUtility.HasSymbiosis(pawns[j])?.TryGetComp<HediffComp_SymbioteControl>();
              if(hediffComp!=null) lords[i].RemovePawn((Pawn)hediffComp.SymbioteControlling);
              lords[i].RemovePawn(pawns[j]);
            }
            
            LordMaker.MakeNewLord(this.parent.Faction, (LordJob) new LordJob_EscortGrendel((Pawn)this.parent), this.parent.Map).AddPawns(pawns);
          }
        }
        
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