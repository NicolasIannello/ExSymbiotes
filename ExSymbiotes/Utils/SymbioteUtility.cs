using System;
using System.Collections.Generic;
using System.Linq;
using ExSymbiotes.Misc;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;
using RimWorld;
using Verse.AI;
using Verse.Noise;

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

        public static void SymbioteHorde(Map map, Building mass, float points, string label, string text)
        {
            bool symbioteMass = mass is Building_SymbioteMass;
            PawnGroupKindDef group =  
                symbioteMass ? ((Building_SymbioteMass)mass).texture == 0 ? ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKind : ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKindRed
                : mass is Building_SymbioteCocoonRed ? ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKindRed : ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKind;
            List<Pawn> fleshbeastsForPoints = GetSymbiotesForPoints(points, map, group);
            List<PawnFlyer> source = new List<PawnFlyer>();
            List<IntVec3> spawnPositions = new List<IntVec3>();
            CellRect cellRect = GenAdj.OccupiedRect(mass.Position, Rot4.North, ThingDefOf.PitGate.Size).ContractedBy(2);
            List<PawnKindDef> animals = new List<PawnKindDef>();
            if (!symbioteMass)
                animals = DefDatabase<PawnKindDef>.AllDefs.Where(d => d.race.race != null && (d.race.race.Animal || d.race.race.IsMechanoid) && !d.race.race.Dryad && !d.race.IsCorpse && !d.isBoss).ToList();
            Lord lord = GetSymbioteLord(map);//lord for cocoons just attack
            for (int i = 0; i < fleshbeastsForPoints.Count; i++)
            {
                IntVec3 result;
                CellFinder.TryFindRandomCellNear(mass.PositionHeld, map, 6, (Predicate<IntVec3>) (cell => !cell.Fogged(map) && cell.Walkable(map) && !cell.Impassable(map)), out result);
                Pawn symbiote = (Pawn)GenSpawn.Spawn((Thing) fleshbeastsForPoints[i], result, map);
                fleshbeastsForPoints[i].rotationTracker.FaceCell(result);
                lord.AddPawn(symbiote);
                if (!symbioteMass)
                {
                    Pawn newThing = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animals.RandomElement()));
                    Hediff hediff = newThing.health.AddHediff(fleshbeastsForPoints[i].def == ExSymbiotesDefOf.ExSymbiotes_SymbioteRed ? ExSymbiotesDefOf.ExSymbiotes_SymbioteControlRed : ExSymbiotesDefOf.ExSymbiotes_SymbioteControl);
                    HediffComp_SymbioteControl comp = hediff.TryGetComp<HediffComp_SymbioteControl>();
                    bool flag = i == fleshbeastsForPoints.Count - 1;
                    GenSpawn.Spawn(newThing, result, map);
                    comp.AddThing(symbiote, flag);
                    source.Add(PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer_Stun, newThing, result, (EffecterDef) null, (SoundDef) null, overrideStartVec: new Vector3?(mass.PositionHeld.ToVector3() + new Vector3(0.0f, 0.0f, -1f))));
                }
                if(symbioteMass) source.Add(PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer_Stun, fleshbeastsForPoints[i], result, (EffecterDef) null, (SoundDef) null, overrideStartVec: new Vector3?(mass.PositionHeld.ToVector3() + new Vector3(0.0f, 0.0f, -1f))));
                spawnPositions.Add(result);
            }
            float intervalSeconds = symbioteMass ? 600.TicksToSeconds() : 60.TicksToSeconds() / (float) fleshbeastsForPoints.Count;
            map.deferredSpawner.AddRequest(new SpawnRequest(source.Cast<Thing>().ToList<Thing>(), spawnPositions, 1, intervalSeconds));
            SoundDefOf.Pawn_Fleshbeast_EmergeFromPitGate.PlayOneShot((SoundInfo) (Thing) mass);
            if(symbioteMass)
            {
                mass.TakeDamage(new DamageInfo(DamageDefOf.Blunt, 1500));
                EffecterDefOf.VoidNodeDisrupted.SpawnMaintained(mass, map);
            }
            EffecterDefOf.VoidStructureActivated.SpawnMaintained(mass, map);
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
        
        public static void MeatSplatter(int filthCount, IntVec3 pos, Map map, FleshbeastUtility.MeatExplosionSize size = FleshbeastUtility.MeatExplosionSize.Normal)
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
        
        //check CompAbilityEffect_SymbiosisLeap
        //ExSymbiotes_Recipe_RemoveSymbiosis
        //CompSymbiote.StartDigesting
        //CompAbilityEffect_SymbioticConsume
        //JobDriver_RemoveSymbiote
        public static Hediff HasSymbiosis(Pawn pawn, bool all = true)
        {
            if (pawn.health == null) return null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
    
            for (int i = 0; i < hediffs.Count; i++)
            {
                HediffDef def = hediffs[i].def;
                if (def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis || 
                    def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Red ||
                    def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_White || 
                    def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Yellow ||
                    def == ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Purple) 
                    return hediffs[i];
                if (all && (def == ExSymbiotesDefOf.ExSymbiotes_SymbioteControl || 
                            def == ExSymbiotesDefOf.ExSymbiotes_SymbioteControlRed ||
                            def == ExSymbiotesDefOf.ExSymbiotes_Thrall))
                    return hediffs[i];
            }
            
            return null;
        }
        
        public static bool CheckAddGene(Pawn pawn, GeneDef gene)
        {
            if (!pawn.genes.HasActiveGene(gene))
            {
                pawn.genes.AddGene(gene, false);
                return true;
            }
            return false;
        }
        
        public static void CheckRemoveGene(Pawn pawn, GeneDef gene, bool added)
        {
            if (pawn.genes.HasActiveGene(gene) && added) pawn.genes.RemoveGene(pawn.genes.GetGene(gene));
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

        public static class SymbioteInteraction
        {
            public static bool CanInteractNowWith(Pawn pawn, Pawn recipient, InteractionDef interactionDef = null)
            {
                return (pawn.IsCarryingPawn(recipient) || recipient.Spawned && SocialInteractionUtility.IsGoodPositionForInteraction(pawn, recipient)) && SocialInteractionUtility.CanInitiateInteraction(pawn, interactionDef) && SocialInteractionUtility.CanReceiveInteraction(recipient, interactionDef);
            }
            
            public static bool TryInteractWith(Pawn pawn, Pawn recipient, InteractionDef intDef, string symbiote, Color symbioteColor, bool initiator)
            {
                if (DebugSettings.alwaysSocialFight)
                  intDef = InteractionDefOf.Insult;
                if (!SymbioteInteraction.CanInteractNowWith(pawn, recipient, intDef))
                  return false;
                List<RulePackDef> extraSentencePacks = new List<RulePackDef>();
                if (intDef.initiatorThought != null)
                  Pawn_InteractionsTracker.AddInteractionThought(pawn, recipient, intDef.initiatorThought);
                if (intDef.recipientThought != null && recipient.needs.mood != null)
                  Pawn_InteractionsTracker.AddInteractionThought(recipient, pawn, intDef.recipientThought);
                // if (intDef.initiatorXpGainSkill != null)
                  // pawn.skills.Learn(intDef.initiatorXpGainSkill, (float) intDef.initiatorXpGainAmount);
                if (intDef.recipientXpGainSkill != null && recipient.RaceProps.Humanlike)
                  recipient.skills.Learn(intDef.recipientXpGainSkill, (float) intDef.recipientXpGainAmount);
                intDef.Worker.Interacted(pawn, recipient, extraSentencePacks, out var letterText, out var letterLabel, out var letterDef, out var lookTargets);
                MoteMaker.MakeInteractionBubble(pawn, recipient, intDef.interactionMote, intDef.GetSymbol(pawn.Faction, pawn.Ideo), intDef.GetSymbolColor(pawn.Faction));
                bool symbioteInitiator = initiator || Rand.RangeInclusive(0, 1)==1;
                PlayLogEntry_SymbiotePawn entry = new PlayLogEntry_SymbiotePawn(intDef, pawn, recipient, extraSentencePacks, symbiote, symbioteInitiator, symbioteColor);
                Find.PlayLog.Add((LogEntry) entry);
                if (letterDef != null)
                {
                  string text = entry.ToGameStringFromPOV((Thing) pawn);
                  if (!letterText.NullOrEmpty())
                    text = $"{text}\n\n{letterText}";
                  Find.LetterStack.ReceiveLetter((TaggedString) letterLabel, (TaggedString) text, letterDef, lookTargets ?? (LookTargets) (Thing) pawn);
                }
                if (intDef == InteractionDefOf.OccultTeaching)
                {
                  Find.ResearchManager.ApplyKnowledge(KnowledgeCategoryDefOf.Basic, 1f);
                }
                SymbioteInteraction.MetalhorrorDetectionCheck(pawn, recipient, symbiote);
                return true;
            }
            
            private static void MetalhorrorDetectionCheck(Pawn pawn, Pawn recipient, string symbiote)
            {
                Hediff_MetalhorrorImplant firstHediff = recipient.health.hediffSet.GetFirstHediff<Hediff_MetalhorrorImplant>();
                if (!MetalhorrorUtility.IsInfected(recipient) || firstHediff == null || firstHediff.Visible || !Rand.Chance(1f / 10f) && !firstHediff.debugDiscoverNextInteraction)
                    return;
                TaggedString reason = "ExSymbiotes.MetalhorrorReasonInteraction".Translate(symbiote, recipient.Named("INFECTED"));
                TaggedString taggedString = "ExSymbiotes.MetalhorrorNoticedDetailsAppended".Translate();
                TaggedString subtleMessage = "ExSymbiotes.InteractionDetectedDesc".Translate(symbiote, recipient.Named("INFECTED")) + $"\n\n{taggedString}";
                MetalhorrorUtility.Detect(recipient, (string) reason, (string) subtleMessage, 0.1f);
            }
        }

        public static class MapGrendelCavesUtility
        {
            private static readonly HashSet<IntVec3> groupSet = new HashSet<IntVec3>();
            private static readonly HashSet<IntVec3> groupVisited = new HashSet<IntVec3>();
            private static readonly List<IntVec3> subGroup = new List<IntVec3>();
            private static List<IntVec3> caves = new List<IntVec3>();
            
            public static void GenerateCaves(Map map, BoolGrid visited, List<IntVec3> group, ModuleBase directionNoise, MapGenCavesUtility.CaveGenParms parms, Predicate<IntVec3> isRock)
            {
                caves.Clear();
                foreach (IntVec3 allCell in map.AllCells)
                {
                    if (!visited[allCell] && isRock(allCell))
                    {
                        group.Clear();
                        map.floodFiller.FloodFill(allCell, isRock, (Action<IntVec3>) (x =>
                        {
                            visited[x] = true;
                            group.Add(x);
                        }));
                        Trim(group, map);
                        RemoveSmallDisconnectedSubGroups(group, map, parms);
                        if (group.Count >= parms.minRocksToGenerateAnyTunnel)
                        {
                            DoOpenTunnels(group, map, directionNoise, parms, isRock);
                        }
                    }
                }
                MapGenerator.SetVar("ExSymbiotes_caves", caves);
            }
            
            private static void Trim(List<IntVec3> group, Map map) => GenMorphology.Open(group, 6, map);

            private static void DoOpenTunnels(List<IntVec3> group, Map map, ModuleBase directionNoise, MapGenCavesUtility.CaveGenParms parms, Predicate<IntVec3> isRock)
            {
                int maxInclusive1 = Mathf.Min(GenMath.RoundRandom((float) ((double) group.Count * (double) Rand.Range(0.9f, 1.1f) * (double) parms.openTunnelsPer10k / 10000.0)), parms.maxOpenTunnelsPerRockGroup);
                if (maxInclusive1 > 0)
                    maxInclusive1 = Rand.RangeInclusive(1, maxInclusive1);
                float maxInclusive2 = parms.tunnelsWidthPerRockCount.Evaluate((float) group.Count);
                for (int index1 = 0; index1 < maxInclusive1; ++index1)
                {
                    IntVec3 start = IntVec3.Invalid;
                    float b = -1f;
                    float dir = -1f;
                    float num = -1f;
                    for (int index2 = 0; index2 < 10; ++index2)
                    {
                        IntVec3 edgeCellForTunnel = MapGenCavesUtility.FindRandomEdgeCellForTunnel(group, map);
                        float distToCave = MapGenCavesUtility.GetDistToCave(edgeCellForTunnel, group, map, 40f, false);
                        float dist;
                        float bestInitialDir = MapGenCavesUtility.FindBestInitialDir(edgeCellForTunnel, group, out dist);
                        if (!start.IsValid || (double) distToCave > (double) b || Mathf.Approximately(distToCave, b) && (double) dist > (double) num)
                        {
                            start = edgeCellForTunnel;
                            b = distToCave;
                            dir = bestInitialDir;
                            num = dist;
                        }
                    }
                    float width = Rand.Range(maxInclusive2 * 0.8f, maxInclusive2);
                    caves.Add(start);
                    MapGenCavesUtility.Dig(start, dir, width, group, map, false, directionNoise, parms, isRock);
                }
            }
            
            private static void RemoveSmallDisconnectedSubGroups(List<IntVec3> group, Map map, MapGenCavesUtility.CaveGenParms parms)
            {
                groupSet.Clear();
                groupSet.AddRange<IntVec3>(group);
                groupVisited.Clear();
                for (int index1 = 0; index1 < group.Count; ++index1)
                {
                    if (!groupVisited.Contains(group[index1]) && groupSet.Contains(group[index1]))
                    {
                        subGroup.Clear();
                        map.floodFiller.FloodFill(group[index1], (Predicate<IntVec3>) (x => groupSet.Contains(x)), (Action<IntVec3>) (x =>
                        {
                            subGroup.Add(x);
                            groupVisited.Add(x);
                        }));
                        if (subGroup.Count < parms.minRocksToGenerateAnyTunnel || (double) subGroup.Count < 0.05000000074505806 * (double) group.Count)
                        {
                            for (int index2 = 0; index2 < subGroup.Count; ++index2)
                                groupSet.Remove(subGroup[index2]);
                        }
                    }
                }
                group.Clear();
                group.AddRange((IEnumerable<IntVec3>) groupSet);
            }
        }
    }
}