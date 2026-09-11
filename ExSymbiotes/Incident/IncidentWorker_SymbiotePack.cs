using System.Collections.Generic;
using System.Linq;
using ExSymbiotes.Utils;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class IncidentWorker_SymbiotePack : IncidentWorker
    {
      protected virtual PawnGroupKindDef groupDef => ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKindBlackRed;
      protected HediffDef hediffDef = ExSymbiotesDefOf.ExSymbiotes_SymbioteControl;
      protected HediffDef hediffDefRed = ExSymbiotesDefOf.ExSymbiotes_SymbioteControlRed;

      protected override bool TryExecuteWorker(IncidentParms parms)
      {
        Map target = (Map) parms.target;
        IntVec3 result = parms.spawnCenter;
        if (!result.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out result, target, CellFinder.EdgeRoadChance_Animal))
          return false;
        Rot4 rot = Rot4.FromAngleFlat((target.Center - result).AngleFlat);
        
        List<Pawn> symbiotes = SymbioteUtility.GetSymbiotesForPoints(parms.points, target, groupDef);
        List<ThingDef> animals = DefDatabase<ThingDef>.AllDefs.Where(d => d.race != null && (d.race.Animal || d.race.IsMechanoid) && !d.race.Dryad && !d.IsCorpse).ToList();
        Lord symbioteLord = SymbioteUtility.GetSymbioteLord(target);
        for (int index = 0; index < symbiotes.Count; ++index)
        {
          PawnKindDef animalKindDef = DefDatabase<PawnKindDef>.AllDefs.FirstOrDefault(pawnKind => pawnKind.race == animals[Rand.RangeInclusive(0,animals.Count-1)]);
          if (animalKindDef == null) continue;
          Pawn newThing = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animalKindDef));
          QuestUtility.AddQuestTag((object) GenSpawn.Spawn((Thing) newThing, CellFinder.RandomClosewalkCellNear(result, target, 10), target, rot), parms.questTag);
          Hediff hediff = newThing.health.AddHediff(symbiotes[index].def==ExSymbiotesDefOf.ExSymbiotes_Symbiote ? hediffDef : hediffDefRed);
          HediffComp_SymbioteControl comp = hediff.TryGetComp<HediffComp_SymbioteControl>();
          
          GenSpawn.Spawn((Thing)symbiotes[index], CellFinder.RandomClosewalkCellNear(result, target, 10), target, rot);
          symbioteLord.AddPawn(symbiotes[index]);
          comp.AddThing(symbiotes[index]);
        }
        this.SendStandardLetter("LetterLabelSymbiotePackArrived".Translate(), "SymbiotePackArrived".Translate(), LetterDefOf.ThreatBig, parms, (LookTargets) new TargetInfo(result, target));
        Find.TickManager.slower.SignalForceNormalSpeedShort();
        return true;
      }
    }
    
    public class IncidentWorker_SymbiotePackRed : IncidentWorker_SymbiotePack
    {
      protected override PawnGroupKindDef groupDef => ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKindRedBlack;
    }
}