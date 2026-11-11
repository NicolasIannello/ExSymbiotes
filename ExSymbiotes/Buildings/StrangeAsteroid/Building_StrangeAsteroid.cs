using System;
using System.Collections.Generic;
using ExSymbiotes.Utils;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
  public class Building_StrangeAsteroid : Building
  {
    private static readonly Texture2D InspectCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/Inspect");

    public override IEnumerable<Gizmo> GetGizmos()
    {
      foreach (Gizmo gizmo in base.GetGizmos())
        yield return gizmo;
      if (this.Spawned)
      {
        Command_Action gizmo1 = new Command_Action();
        gizmo1.defaultLabel = (string) "ExSymbiotes.StrangeAsteroidCommandInspect".Translate((NamedArgument) (Thing) this);
        gizmo1.defaultDesc = (string) "ExSymbiotes.StrangeAsteroidCommandInspectDesc".Translate();
        gizmo1.icon = (Texture) Building_StrangeAsteroid.InspectCommandTex;
        gizmo1.action = (Action) (() => Find.Targeter.BeginTargeting(TargetingParameters.ForColonist(), (Action<LocalTargetInfo>) (targ =>
        {
          if (!(targ.Thing is Pawn thing2))
            return;
          thing2.jobs.TryTakeOrderedJob(JobMaker.MakeJob(ExSymbiotesDefOf.ExSymbiotes_InspectStrangeAsteroid, (LocalTargetInfo) (Thing) this));
        }), (Action<LocalTargetInfo>) (targ =>
        {
          if (!ValidateInspectionTarget(targ))
            return;
          GenDraw.DrawTargetHighlight(targ);
        }), new Func<LocalTargetInfo, bool>(ValidateInspectionTarget), mouseAttachment: Building_StrangeAsteroid.InspectCommandTex, onGuiAction: (Action<LocalTargetInfo>) (_ => Widgets.MouseAttachedLabel("ExSymbiotes.StrangeAsteroidChooseWhoShouldInspect".Translate((NamedArgument) (Thing) this)))));
        yield return (Gizmo) gizmo1;
      }

      static bool ValidateInspectionTarget(LocalTargetInfo targ)
      {
        return targ.Thing is Pawn thing && thing.IsColonistPlayerControlled && thing.CanReach((LocalTargetInfo) thing.Position, PathEndMode.Touch, Danger.Deadly) && !thing.Downed && thing.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
      }
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
    {
      foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
        yield return floatMenuOption;

      yield return new FloatMenuOption((string) "ExSymbiotes.StrangeAsteroidCommandInspect".Translate((NamedArgument) (Thing) this), (Action) (() => selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(ExSymbiotesDefOf.ExSymbiotes_InspectStrangeAsteroid, (LocalTargetInfo) (Thing) this))));
    }
    
    public void Inspect(Pawn pawn)
    {
      if (SymbioteUtility.HasSymbiosis(pawn) != null)
      {
        Find.LetterStack.ReceiveLetter("ExSymbiotes.StrangeAsteroid".Translate(), "ExSymbiotes.StrangeAsteroidInspectFail".Translate((NamedArgument) (Thing) pawn), LetterDefOf.NeutralEvent, pawn);
        return;
      }
      HediffDef hediff = this.def == ExSymbiotesDefOf.ExSymbiotes_StrangeAsteroid ? ExSymbiotesDefOf.ExSymbiotes_Symbiosis : ExSymbiotesDefOf.ExSymbiotes_Symbiosis_Red;
      pawn.health.AddHediff(hediff);

      Find.LetterStack.ReceiveLetter("ExSymbiotes.StrangeAsteroid".Translate(), "ExSymbiotes.StrangeAsteroidInspectSuccess".Translate((NamedArgument) (Thing) pawn), LetterDefOf.NeutralEvent, pawn);
      
      this.Destroy();
    }
  }
}