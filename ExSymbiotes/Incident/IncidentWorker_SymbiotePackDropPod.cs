using System.Collections.Generic;
using System.Linq;
using ExSymbiotes.Utils;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class IncidentWorker_SymbiotePackDropPod : IncidentWorker
    {
      protected virtual PawnGroupKindDef groupDef => ExSymbiotesDefOf.ExSymbiotes_Symbiote_PawnGroupKindRedBlack;
      protected HediffDef hediffDef = ExSymbiotesDefOf.ExSymbiotes_SymbioteControl;
      protected HediffDef hediffDefRed = ExSymbiotesDefOf.ExSymbiotes_SymbioteControlRed;

      protected override bool TryExecuteWorker(IncidentParms parms)
      {
        Map target = (Map) parms.target;
        
        List<Pawn> symbiotes = SymbioteUtility.GetSymbiotesForPoints(parms.points, target, groupDef);
        List<PawnKindDef> animals = DefDatabase<PawnKindDef>.AllDefs.Where(d => d.race != null && (d.race.race.Animal || d.race.race.IsMechanoid) && !d.race.race.Dryad && !d.race.IsCorpse && !d.isBoss).ToList();
        Lord symbioteLord = SymbioteUtility.GetSymbioteLord(target);
        for (int index = 0; index < symbiotes.Count; ++index)
        {
          IntVec3 result = DropCellFinder.RandomDropSpot(target);
          if (!result.IsValid)
            continue;
          Rot4 rot = Rot4.FromAngleFlat((target.Center - result).AngleFlat);
          PawnKindDef animalKindDef = animals[Rand.RangeInclusive(0, animals.Count - 1)];
          Pawn newThing = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animalKindDef));
          Hediff hediff = newThing.health.AddHediff(symbiotes[index].def==ExSymbiotesDefOf.ExSymbiotes_Symbiote ? hediffDef : hediffDefRed);
          hediff.Severity = Rand.RangeInclusive(15, 75) / 100f;
          HediffComp_SymbioteControl comp = hediff.TryGetComp<HediffComp_SymbioteControl>();
          
          symbioteLord.AddPawn(symbiotes[index]);
          bool flag = index == symbiotes.Count - 1;
          comp.AddThing(symbiotes[index], flag);
          DropPodUtility.DropThingsNear(result, target, Gen.YieldSingle<Thing>((Thing) newThing), parms.podOpenDelay, leaveSlag: false, canRoofPunch: true, faction: symbiotes[index].Faction);
        }
        this.SendStandardLetter("ExSymbiotes.LetterLabelSymbiotePackArrived".Translate(), "ExSymbiotes.SymbiotePackArrived".Translate(), LetterDefOf.ThreatBig, parms, new LookTargets((IEnumerable<Pawn>) symbioteLord.ownedPawns));
        Find.TickManager.slower.SignalForceNormalSpeedShort();
        return true;
      }
    }
    
}