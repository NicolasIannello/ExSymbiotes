using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;
using RimWorld;

namespace ExSymbiotes.Utils
{
    public class SymbioteUtility
    {
        public static List<Pawn> GetSymbiotesForPoints(float points, Map map, PawnGroupKindDef group)
        {
            PawnGroupMakerParms parms = new PawnGroupMakerParms()
            {
                groupKind = group,
                tile = map.Tile,
                faction = Faction.OfEntities,
                points = (double) points > 0.0 ? points : StorytellerUtility.DefaultThreatPointsNow((IIncidentTarget) map)
            };
            parms.points = Mathf.Max(parms.points, parms.faction.def.MinPointsToGeneratePawnGroup(parms.groupKind) * 1.05f);
            return PawnGroupMakerUtility.GeneratePawns(parms).ToList<Pawn>();
        }

        public static void SymbioteHorde(Map map, Building_SymbioteMass mass, bool first = false)//INCREMENT POINTS PER MISSING HEALTH
        {
            PawnGroupKindDef group = mass.texture == 0 ? ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKind : ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKindRed;
            List<Pawn> fleshbeastsForPoints = GetSymbiotesForPoints(StorytellerUtility.DefaultThreatPointsNow(map), map, group);
            List<PawnFlyer> source = new List<PawnFlyer>();
            List<IntVec3> spawnPositions = new List<IntVec3>();
            CellRect cellRect = GenAdj.OccupiedRect(mass.Position, Rot4.North, ThingDefOf.PitGate.Size).ContractedBy(2);
            foreach (Pawn pawn in fleshbeastsForPoints)
            {
                IntVec3 randomCell = cellRect.RandomCell;
                GenSpawn.Spawn((Thing) pawn, randomCell, map);
                IntVec3 result;
                CellFinder.TryFindRandomCellNear(mass.Position, map, ThingDefOf.PitGate.size.x / 2 + 1, (Predicate<IntVec3>) (cell => !cell.Fogged(map) && cell.Walkable(map) && !cell.Impassable(map)), out result);
                pawn.rotationTracker.FaceCell(result);
                source.Add(PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer_Stun, pawn, result, (EffecterDef) null, (SoundDef) null, overrideStartVec: new Vector3?(randomCell.ToVector3() + new Vector3(0.0f, 0.0f, -1f))));
                spawnPositions.Add(randomCell);
            }
            float intervalSeconds = 600.TicksToSeconds() / (float) fleshbeastsForPoints.Count;
            map.deferredSpawner.AddRequest(new SpawnRequest(source.Cast<Thing>().ToList<Thing>(), spawnPositions, 1, intervalSeconds)
            {
                lord = LordMaker.MakeNewLord(Faction.OfEntities, (LordJob) new LordJob_FleshbeastAssault(), map)
            });
            SoundDefOf.Pawn_Fleshbeast_EmergeFromPitGate.PlayOneShot((SoundInfo) (Thing) mass);
            mass.TakeDamage(new DamageInfo(DamageDefOf.Blunt, 1500));
            EffecterDefOf.VoidNodeDisrupted.SpawnMaintained(mass, map);
            EffecterDefOf.VoidStructureActivated.Spawn(mass, map);
            Find.LetterStack.ReceiveLetter(
                first ? "ExSymbiotes.LabelSymbioteHorde".Translate() : "ExSymbiotes.LabelSymbioteHorde2".Translate(),
                first ? "ExSymbiotes.TextSymbioteHorde".Translate() : "ExSymbiotes.TextSymbioteHorde2".Translate(),
                LetterDefOf.ThreatBig,
                new TargetInfo(mass.Position, map)
            );
        }
        
        public static void MeatSplatter(
            int filthCount,
            IntVec3 pos,
            Map map,
            FleshbeastUtility.MeatExplosionSize size = FleshbeastUtility.MeatExplosionSize.Normal)
        {
            switch (size)
            {
                case FleshbeastUtility.MeatExplosionSize.Small:
                    ExSymbiotesDefOf.ExSymbiotes_MeatExplosion_Black.Spawn(pos, map).Cleanup();
                    break;
                case FleshbeastUtility.MeatExplosionSize.Normal:
                    ExSymbiotesDefOf.ExSymbiotes_MeatExplosion_Black.Spawn(pos, map).Cleanup();
                    break;
                case FleshbeastUtility.MeatExplosionSize.Large:
                    ExSymbiotesDefOf.ExSymbiotes_MeatExplosion_Black.Spawn(pos, map).Cleanup();
                    break;
            }
            CellRect cellRect = new CellRect(pos.x, pos.z, 3, 3).ClipInsideMap(map);
            for (int index = 0; index < filthCount; ++index)
            {
                IntVec3 randomCell = cellRect.RandomCell;
                ThingDef filthDef = ThingDefOf.Filth_RevenantBloodPool;
                if (randomCell.InBounds(map) && GenSight.LineOfSight(randomCell, pos, map))
                    FilthMaker.TryMakeFilth(randomCell, map, filthDef);
            }
        }
        
        public static Hediff HasSymbiosis(Pawn pawn)
        {
            Hediff symbiosisB = pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis);
            if (symbiosisB != null) return symbiosisB;
            Hediff symbiosisR = pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Red);
            if (symbiosisR != null) return symbiosisR;

            return null;
        }
    }
}