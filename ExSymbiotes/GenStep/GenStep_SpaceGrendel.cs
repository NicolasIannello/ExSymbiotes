using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Noise;

namespace ExSymbiotes;

public class GenStep_SpaceGrendel : GenStep
{
  private ModuleBase innerNoise;

  public override int SeedPart => 1959414;

  protected virtual float Radius => 0.224f;

  public override void Generate(Map map, GenStepParams parms)
  {
    if (!ModsConfig.OdysseyActive)
      return;
    this.GenerateAsteroidElevation(map, parms);
    this.GenerateCaveElevation(map, parms);
    GenStep_SpaceGrendel.SpawnAsteroid(map);
    map.OrbitalDebris = OrbitalDebrisDefOf.Asteroid;//custom
  }

  private static void SpawnAsteroid(Map map)
  {
    using (map.pathing.DisableIncrementalScope())
    {
      foreach (IntVec3 allCell in map.AllCells)
      {
        double num = (double) MapGenerator.Elevation[allCell];
        float cave = MapGenerator.Caves[allCell];
        if (num > 0.5)
          map.terrainGrid.SetTerrain(allCell, TerrainDefOf.Voidmetal);
        if (num > 0.699999988079071 && (double) cave == 0.0)
          GenSpawn.Spawn(ThingDefOf.VoidmetalMassSmall, allCell, map);
        if (num > 0.699999988079071)
          map.roofGrid.SetRoof(allCell, RoofDefOf.RoofRockThin);
      }
      HashSet<IntVec3> mainIsland = new HashSet<IntVec3>();
      map.floodFiller.FloodFill(map.Center, (Predicate<IntVec3>) (x => x.GetTerrain(map) != TerrainDefOf.Space), (Action<IntVec3>) (x => mainIsland.Add(x)));
      foreach (IntVec3 allCell in map.AllCells)
      {
        if (!mainIsland.Contains(allCell))
        {
          map.terrainGrid.SetTerrain(allCell, TerrainDefOf.Space);
          map.roofGrid.SetRoof(allCell, (RoofDef) null);
          foreach (Thing thing in allCell.GetThingList(map).ToList<Thing>())
            thing.Destroy();
        }
      }
    }
  }

  private void GenerateAsteroidElevation(Map map, GenStepParams parms)
  {
    this.innerNoise = this.ConfigureNoise(map, parms);
    foreach (IntVec3 allCell in map.AllCells)
      MapGenerator.Elevation[allCell] = this.innerNoise.GetValue(allCell);
  }

  protected virtual ModuleBase ConfigureNoise(Map map, GenStepParams parms)
  {
    ModuleBase input1 = (ModuleBase) new Scale(0.6499999761581421, 1.0, 1.0, (ModuleBase) new ScaleBias(-1.0, 1.0, (ModuleBase) new DistFromPoint((float) map.Size.x * this.Radius)));
    ModuleBase input2 = (ModuleBase) new Rotate(0.0, (double) Rand.Range(0.0f, 360f), 0.0, input1);
    ModuleBase moduleBase = (ModuleBase) new Translate((double) -map.Center.x, 0.0, (double) -map.Center.z, input2);
    NoiseDebugUI.StoreNoiseRender(moduleBase, "Base asteroid shape");
    ModuleBase noise = (ModuleBase) new Power((ModuleBase) new Blend((ModuleBase) new Perlin(0.05000000074505806, 2.0, 0.5, 6, Rand.Int, QualityMode.Medium), (ModuleBase) new Blend((ModuleBase) new Perlin(3.0 / 500.0, 2.0, 2.0, 3, Rand.Int, QualityMode.Medium), moduleBase, (ModuleBase) new Const(0.800000011920929)), (ModuleBase) new Const(0.8500000238418579)), (ModuleBase) new Const(0.20000000298023224));
    NoiseDebugUI.StoreNoiseRender(noise, "Asteroid");
    return noise;
  }

  private void GenerateCaveElevation(Map map, GenStepParams parms)
  {
    Perlin directionNoise = new Perlin(1.0 / 500.0, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
    MapGenFloatGrid elevation = MapGenerator.Elevation;
    BoolGrid visited = new BoolGrid(map);
    List<IntVec3> group = new List<IntVec3>();
    MapGenCavesUtility.CaveGenParms parms1 = MapGenCavesUtility.CaveGenParms.Default with
    {
      widthOffsetPerCell = 0.005f,
      minTunnelWidth = 2f,
      branchChance = 0.5f,
      maxOpenTunnelsPerRockGroup = 30,
      maxClosedTunnelsPerRockGroup = 0,
      openTunnelsPer10k = 30,
      closedTunnelsPer10k = 0,
      tunnelsWidthPerRockCount = new SimpleCurve()
      {
        { new CurvePoint(100f, 3), true },
        { new CurvePoint(300f, 3.5f), true },
        { new CurvePoint(3000f, 4f), true }
      }
    };
    MapGenCavesUtility.GenerateCaves(map, visited, group, (ModuleBase) directionNoise, parms1, new Predicate<IntVec3>(Rock));

    bool Rock(IntVec3 cell) => this.IsRock(cell, elevation, map);
  }

  private bool IsRock(IntVec3 c, MapGenFloatGrid elevation, Map map)
  {
    return c.InBounds(map) && (double) elevation[c] > 0.699999988079071;
  }
  
}
