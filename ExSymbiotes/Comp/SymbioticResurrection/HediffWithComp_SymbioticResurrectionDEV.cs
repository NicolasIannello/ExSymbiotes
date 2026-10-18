using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes;

public class HediffWithComp_SymbioticResurrectionDEV : HediffWithComps
{
    private string SymbioticResurrectionText = "ExSymbiotes.SymbioticResurrection".Translate();
    private string SymbioticResurrectionDescText = "ExSymbiotes.SymbioticResurrectionDesc".Translate();

    public override IEnumerable<Gizmo> GetGizmos()
    {
        IEnumerable<Gizmo> compGetGizmos = base.GetGizmos();
        if (compGetGizmos != null) foreach (Gizmo gizmo in compGetGizmos) yield return gizmo;
        
        Command_Action gizmo1 = new Command_Action();
        gizmo1.icon = (Texture)ContentFinder<Texture2D>.Get("UI/SymbioticResurrect");
        gizmo1.defaultLabel = SymbioticResurrectionText;
        gizmo1.defaultDesc = SymbioticResurrectionDescText;
        gizmo1.action = (Action) this.SymbioticResurrection;
        yield return (Gizmo) gizmo1;
    }

    public void SymbioticResurrection()
    {
        ResurrectionUtility.TryResurrect(this.pawn);
        this.pawn.health.AddHediff(ExSymbiotesDefOf.ExSymbiotes_SymbioticResurrection);
        this.pawn.health.RemoveHediff(this);
    }
}