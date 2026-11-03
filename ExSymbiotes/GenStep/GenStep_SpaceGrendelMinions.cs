using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{

  public class GenStep_SpaceGrendelMinion : GenStep
  {
    public override int SeedPart => 2127885;

    public override void Generate(Map map, GenStepParams parms)
    {
      if (!ModsConfig.OdysseyActive)
        return;
      
      List<IntVec3> cocoons = new List<IntVec3>();
      List<IntVec3> caves = MapGenerator.GetVar<List<IntVec3>>("ExSymbiotes_caves");
      
      for (int i = 0; i < caves.Count-1; i++)
        if(caves[i].DistanceTo(map.Center)>=100f && cocoons.Count<4)
          cocoons.Add(caves[i]);
        
      if(cocoons.Count<4)
        for (int i = 0; i < caves.Count-1; i++)
          if(caves[i].DistanceTo(map.Center)>65f && caves[i].DistanceTo(map.Center)<100f && cocoons.Count<4)
            cocoons.Add(caves[i]);

      for (int i = 0; i < cocoons.Count - 1; i++)
      {
        IntVec3 spot = Vector3.MoveTowards(cocoons[i].ToVector3(), map.Center.ToVector3(), 15).ToIntVec3();
        
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(spot, 10, true))
        {
          List<Thing> things = cell.GetThingList(map);
          for (int j = things.Count - 1; j >= 0; j--) 
            if(things[j].def == ThingDefOf.VoidmetalMassSmall) 
              things[j].DeSpawn();
        }
        
        GenSpawn.Spawn(ThingDefOf.Fleshbulb, spot, map);
      }
    }
  }
}
