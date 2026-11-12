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
      protected virtual bool pod => false;

      protected override bool TryExecuteWorker(IncidentParms parms)
      {
        Map target = (Map) parms.target;
        IntVec3 result = IntVec3.Zero;
        Rot4 rot = Rot4.Random;
        if (!pod)
        {
          result = parms.spawnCenter;
          if (!result.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out result, target, CellFinder.EdgeRoadChance_Animal))
            return false;
          rot = Rot4.FromAngleFlat((target.Center - result).AngleFlat);
        }
        
        List<Pawn> symbiotes = SymbioteUtility.GetSymbiotesForPoints(parms.points, target, groupDef);
        List<PawnKindDef> animals = DefDatabase<PawnKindDef>.AllDefs.Where(d => d.race.race != null && (d.race.race.Animal || d.race.race.IsMechanoid) && !d.race.race.Dryad && !d.race.IsCorpse && !d.isBoss).ToList();
        Lord symbioteLord = SymbioteUtility.GetSymbioteLord(target);
        for (int index = 0; index < symbiotes.Count; ++index)
        {
          if (pod)
          {
            result = DropCellFinder.RandomDropSpot(target);
            if (!result.IsValid)
              continue;
            rot = Rot4.FromAngleFlat((target.Center - result).AngleFlat);
          }
          PawnKindDef animalKindDef = animals[Rand.RangeInclusive(0, animals.Count - 1)];
          Pawn newThing = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animalKindDef));
          if(!pod) 
            QuestUtility.AddQuestTag((object) GenSpawn.Spawn((Thing) newThing, CellFinder.RandomClosewalkCellNear(result, target, 10), target, rot), parms.questTag);
          Hediff hediff = newThing.health.AddHediff(symbiotes[index].def==ExSymbiotesDefOf.ExSymbiotes_Symbiote ? hediffDef : hediffDefRed);
          hediff.Severity = Rand.RangeInclusive(15, 75) / 100f;
          HediffComp_SymbioteControl comp = hediff.TryGetComp<HediffComp_SymbioteControl>();
          
          if(!pod) 
            GenSpawn.Spawn((Thing)symbiotes[index], CellFinder.RandomClosewalkCellNear(result, target, 10), target, rot);
          symbioteLord.AddPawn(symbiotes[index]);
          bool flag = index == symbiotes.Count - 1;
          comp.AddThing(symbiotes[index], flag);
          if(pod) 
            DropPodUtility.DropThingsNear(result, target, Gen.YieldSingle<Thing>((Thing) newThing), parms.podOpenDelay, leaveSlag: false, canRoofPunch: true, faction: symbiotes[index].Faction);
        }
        TaggedString letter = pod ? "ExSymbiotes.LetterLabelSymbiotePods".Translate() : "ExSymbiotes.LetterLabelSymbiotePackArrived".Translate();
        TaggedString text = pod ? "ExSymbiotes.SymbiotePods".Translate() : "ExSymbiotes.SymbiotePackArrived".Translate();
        this.SendStandardLetter(letter, text, LetterDefOf.ThreatBig, parms, new LookTargets((IEnumerable<Pawn>) symbioteLord.ownedPawns));
        Find.TickManager.slower.SignalForceNormalSpeedShort();
        return true;
      }
    }
    
    public class IncidentWorker_SymbiotePackRed : IncidentWorker_SymbiotePack
    {
      protected override PawnGroupKindDef groupDef => ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKindRedBlack;
    }

    public class IncidentWorker_SymbiotePackDropPod : IncidentWorker_SymbiotePackRed
    {
      protected override bool pod => true;
    }
}