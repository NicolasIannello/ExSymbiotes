using System;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace ExSymbiotes;

public class QuestNode_Root_SpaceGrendel : QuestNode
{
  public PlanetLayerDef layerDef;
  public WorldObjectDef worldObjectDef ;
  private const int MaxDistanceFromColony = 3;
  private const int MinDistanceFromColony = 1;
  private static readonly IntRange TimeoutDays = new IntRange(10, 20);

  protected override bool TestRunInt(Slate slate) => this.TryFindSiteTile(out PlanetTile _);

  protected override void RunInt()
  {
    if (!ModsConfig.OdysseyActive)
      return;
    Quest quest = RimWorld.QuestGen.QuestGen.quest;
    Slate slate = RimWorld.QuestGen.QuestGen.slate;
    PlanetTile tile;
    this.TryFindSiteTile(out tile);
    SpaceMapParent var = (SpaceMapParent) WorldObjectMaker.MakeWorldObject(this.worldObjectDef);
    var.Tile = tile;
    slate.Set<SpaceMapParent>("worldObject", var);
    quest.SpawnWorldObject((WorldObject) var);
    string inSignal = QuestGenUtility.HardcodedSignalWithQuestID("worldObject.MapRemoved");
    int delayTicks = QuestNode_Root_SpaceGrendel.TimeoutDays.RandomInRange * 60000;
    quest.WorldObjectTimeout((WorldObject) var, delayTicks);
    quest.Delay(delayTicks, (Action) (() => QuestGen_End.End(quest, QuestEndOutcome.Fail)));
    quest.End(QuestEndOutcome.Success, inSignal: inSignal);
    Find.LetterStack.ReceiveLetter("ExSymbiotes.SpaceGrendelLetter".Translate(), "ExSymbiotes.SpaceGrendelText".Translate(), LetterDefOf.ThreatBig, null, null, quest);
  }

  private bool TryFindSiteTile(out PlanetTile tile)
  {
    tile = PlanetTile.Invalid;
    if (!ModsConfig.OdysseyActive)
      return false;
    PlanetTile tile1;
    PlanetTile origin = TileFinder.TryFindRandomPlayerTile(out tile1, false, canBeSpace: true) ? tile1 : new PlanetTile(0, (PlanetLayer) Find.WorldGrid.Surface);
    PlanetLayer layer;
    if (!Find.WorldGrid.TryGetFirstAdjacentLayerOfDef(origin, this.layerDef, out layer))
      return false;
    FastTileFinder.TileQueryParams query = new FastTileFinder.TileQueryParams(origin, MinDistanceFromColony, MaxDistanceFromColony);
    return layer.FastTileFinder.Query(query).TryRandomElement<PlanetTile>(out tile);
  }
}