using System.Collections.Generic;
using Verse;

namespace ExSymbiotes;

public class GenStep_SpaceGrendelSpawn : GenStep
{
  public override int SeedPart => 1927415;

  public override void Generate(Map map, GenStepParams parms)
  {
    if (!ModsConfig.OdysseyActive)
      return;

    foreach (IntVec3 cell in GenRadial.RadialCellsAround(map.Center, 30, true))
    {
      List<Thing> things = cell.GetThingList(map);
      for (int i = things.Count - 1; i >= 0; i--) things[i].DeSpawn();
    }
    
    GenSpawn.Spawn(PawnGenerator.GeneratePawn(ExSymbiotesDefOf.ExSymbiotes_GrendelSymbiote_PawnKind, Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes)), map.Center, map);
  }
  
}
