using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;
using RimWorld;
using Verse.AI;

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
                faction = Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes),
                points = points
            };
            parms.points = Mathf.Max(parms.points, parms.faction.def.MinPointsToGeneratePawnGroup(parms.groupKind) * 1.05f);
            return PawnGroupMakerUtility.GeneratePawns(parms).ToList<Pawn>();
        }

        public static void SymbioteHorde(Map map, Building_SymbioteMass mass, float points, string label, string text)
        {
            PawnGroupKindDef group = mass.texture == 0 ? ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKind : ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKindRed;
            List<Pawn> fleshbeastsForPoints = GetSymbiotesForPoints(points, map, group);
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
                lord = GetSymbioteLord(map)
            });
            SoundDefOf.Pawn_Fleshbeast_EmergeFromPitGate.PlayOneShot((SoundInfo) (Thing) mass);
            mass.TakeDamage(new DamageInfo(DamageDefOf.Blunt, 1500));
            EffecterDefOf.VoidNodeDisrupted.SpawnMaintained(mass, map);
            EffecterDefOf.VoidStructureActivated.Spawn(mass, map);
            Find.LetterStack.ReceiveLetter(label.Translate(), text.Translate(), LetterDefOf.ThreatBig, new TargetInfo(mass.Position, map));
        }

        public static void SingleSymbiote(Map map, Pawn symbiote)
        {
            Pawn symbiotePawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(symbiote.kindDef, faction: symbiote.Faction));
            CellRect cellRect = GenAdj.OccupiedRect(symbiote.Position, Rot4.North, ThingDefOf.PitGate.Size).ContractedBy(2);
            IntVec3 randomCell = cellRect.RandomCell;
            GenSpawn.Spawn((Thing) symbiotePawn, randomCell, map);
            CellFinder.TryFindRandomCellNear(symbiote.Position, map, ThingDefOf.PitGate.size.x / 2 + 1, (Predicate<IntVec3>) (cell => !cell.Fogged(map) && cell.Walkable(map) && !cell.Impassable(map)), out IntVec3 result);
            symbiotePawn.rotationTracker.FaceCell(result);
            PawnFlyer source = PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer_Stun, symbiotePawn, result, (EffecterDef) null, (SoundDef) null, overrideStartVec: new Vector3?(randomCell.ToVector3() + new Vector3(0.0f, 0.0f, -1f)));
            IntVec3 spawnPositions = randomCell;
            
            map.deferredSpawner.AddRequest(new SpawnRequest(new List<Thing> { source }, new List<IntVec3> { spawnPositions }, 1, 0f)
            {
                lord = GetSymbioteLord(map)
            });
            SoundDefOf.Pawn_Fleshbeast_EmergeFromPitGate.PlayOneShot((SoundInfo) (Thing) symbiote);
        }

        public static Lord GetSymbioteLord(Map map)
        {
            foreach (var lord in map.lordManager.lords)
            { 
                if (lord.LordJob is LordJob_SymbioteMass) return lord;
            }
            return LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes), (LordJob) new LordJob_SymbioteMass(), map);
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
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
    
            for (int i = 0; i < hediffs.Count; i++)
            {
                HediffDef def = hediffs[i].def;
                if (def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis || def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Red ||
                    def == ExSymbiotesDefOf.ExSymbiotes_SymbioteControl || def == ExSymbiotesDefOf.ExSymbiotes_SymbioteControlRed)
                    return hediffs[i];
            }
            
            return null;
        }
        
        public static class TargetFinder
        {
            
            public static bool CanReach(
                Thing searcher,
                Thing target,
                bool canBashDoors,
                bool canBashFences)
            {
                if (searcher is Pawn pawn)
                {
                    if (!pawn.CanReach((LocalTargetInfo) target, PathEndMode.Touch, Danger.Some, canBashDoors, canBashFences))
                        return false;
                }
                else
                {
                    TraverseMode mode = canBashDoors ? TraverseMode.PassDoors : TraverseMode.NoPassClosedDoors;
                    if (!searcher.Map.reachability.CanReach(searcher.Position, (LocalTargetInfo) target, PathEndMode.Touch, TraverseParms.For(mode)))
                        return false;
                }
                return true;
            }
            
            public static bool CanShootAtFromCurrentPosition(
                IAttackTarget target,
                IAttackTargetSearcher searcher,
                Verb verb)
            {
                return verb != null && verb.CanHitTargetFrom(searcher.Thing.Position, (LocalTargetInfo) target.Thing);
            }
        }
    }
}