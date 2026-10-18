using System;
using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes;

public class FloatMenuOptionProvider_RemoveSymbiote : FloatMenuOptionProvider
{
    protected override bool Drafted => true;

    protected override bool Undrafted => true;

    protected override bool Multiselect => false;

    protected override bool RequiresManipulation => true;

    protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
    {
        Corpse corpse = clickedThing as Corpse;
        if (corpse == null)
            return (FloatMenuOption) null;
        Hediff symbiosis = SymbioteUtility.HasSymbiosis(corpse.InnerPawn, false);
        return symbiosis == null ? (FloatMenuOption) null : 
            FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(
                (string) ("ExSymbiotes.Removing".Translate() + " " + symbiosis.def.label), 
                (Action) (() => context.FirstSelectedPawn.jobs.TryTakeOrderedJob(
                    JobMaker.MakeJob(ExSymbiotesDefOf.ExSymbiotes_RemoveSymbiote, (LocalTargetInfo) (Thing) corpse)))), context.FirstSelectedPawn, new LocalTargetInfo((Thing) corpse));
    }
}