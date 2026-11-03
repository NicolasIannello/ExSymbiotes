using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace ExSymbiotes
{

  public class GenStep_SpaceGrendelMinion : GenStep
  {
    public override int SeedPart => 2127885;
    private List<IntVec3> cocoons = new List<IntVec3>();

    public override void Generate(Map map, GenStepParams parms)
    {
      if (!ModsConfig.OdysseyActive)
        return;
      
      cocoons.Clear();
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
    
    public override void PostMapInitialized(Map map, GenStepParams parms)
    {
      base.PostMapInitialized(map, parms);
      
      List<PawnKindDef> animals = DefDatabase<PawnKindDef>.AllDefs.Where(d => d.race.race != null && (d.race.race.Animal || d.race.race.IsMechanoid) && !d.race.race.Dryad && !d.race.IsCorpse && !d.isBoss).ToList();
      for (int i = 0; i < cocoons.Count-1; i++)
      {
        IntVec3 spot = Vector3.MoveTowards(cocoons[i].ToVector3(), map.Center.ToVector3(), 15).ToIntVec3();
        int chance = Rand.RangeInclusive(7, 12);
        Lord lord = LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes), (LordJob)new LordJob_DefendPoint(spot, 9, 20, addFleeToil: false), map);
        for (int j = 0; j < chance - 1; j++)
        {
          bool chanceRed = Rand.RangeInclusive(1, 5) == 1;
          Rot4 rot = Rot4.FromAngleFlat((map.Center - spot).AngleFlat);
          IntVec3 spawnPos = CellFinder.RandomClosewalkCellNear(spot, map, 8);
          Pawn newSymbiote = PawnGenerator.GeneratePawn(new PawnGenerationRequest(
            chanceRed ? ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnKindRed : ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnKind,
            Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes)));
          Pawn newThing = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animals.RandomElement()));
          Hediff hediff = newThing.health.AddHediff(chanceRed ? ExSymbiotesDefOf.ExSymbiotes_SymbioteControlRed : ExSymbiotesDefOf.ExSymbiotes_SymbioteControl);
          HediffComp_SymbioteControl comp = hediff.TryGetComp<HediffComp_SymbioteControl>();
          bool flag = j == chance - 1;

          Pawn symbiote = (Pawn)GenSpawn.Spawn(newSymbiote, spawnPos, map, rot);
          lord.AddPawn(symbiote);
          GenSpawn.Spawn(newThing, spawnPos, map, rot);
          comp.AddThing(symbiote, flag);
        }
      }
    }
  }
}
