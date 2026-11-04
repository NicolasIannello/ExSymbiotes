using System;
using System.Collections.Generic;
using ExSymbiotes.Utils;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace ExSymbiotes
{
  public class Building_SymbioteCocoon : Building
  {
    private static readonly Vector3 PartDrawSize = new Vector3(2.5f, 2.5f, 2.5f);
    private int bpm = 30;
    [Unsaved(false)]
    private int lastBeatTick = -99999;
    [Unsaved(false)]
    private Graphic cachedCenterPartGraphic;
    protected virtual string texture => "Things/Buildings/SymbioteCocoon/SymbioteCocoon";
    protected virtual float points => 1000;
    private Graphic CenterPartGraphic
    {
      get
      {
        return this.cachedCenterPartGraphic ?? (this.cachedCenterPartGraphic = GraphicDatabase.Get<Graphic_Multi>(texture, ShaderDatabase.Cutout, (Vector2) Building_SymbioteCocoon.PartDrawSize, Color.white));
      }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
      base.SpawnSetup(map, respawningAfterLoad);
      if (respawningAfterLoad || this.BeingTransportedOnGravship)
        return;
      if (this.Faction != Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes))
        this.SetFaction(Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes), (Pawn) null);
    }
    
    protected override void Tick()
    {
      base.Tick();
      if ((double) Find.TickManager.TicksGame > (double) this.lastBeatTick + 60.0 / (double) this.bpm * 60.0)
        this.Beat();
    }
    
    private void Beat()
    {
      this.lastBeatTick = Find.TickManager.TicksGame;
      SoundDefOf.FleshmassHeart_Throb.PlayOneShot((SoundInfo) (Thing) this);
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
      base.DrawAt(drawLoc, flip);
      Matrix4x4 matrix1 = Matrix4x4.TRS(drawLoc + new Vector3(0.0f, 0.35f, -0.35f), Quaternion.AngleAxis(0.0f, Vector3.up), new Vector3(this.GetBeatScale(1.15f), 1f, this.GetBeatScale(1.2f)));
      GenDraw.DrawMeshNowOrLater(this.CenterPartGraphic.MeshAt(Rot4.South), matrix1, this.CenterPartGraphic.MatSouth, false);
    }

    private float GetBeatScale(float maxScale, int delay = 0)
    {
      float num = (float) (Find.TickManager.TicksGame - (this.lastBeatTick + delay)) / 15f;
      return (double) num < 1.0 ? Mathf.Lerp(1f, maxScale, Mathf.Sin(3.1415927f * num)) : 1f;
    }

    public void Explode(Pawn grendel)
    {
      SymbioteUtility.SymbioteHorde(this.MapHeld, this, points, "ExSymbiotes.LabelSymbioteCocoon", "ExSymbiotes.TextSymbioteCocoon", grendel);
      this.Kill();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
      IEnumerable<Gizmo> compGetGizmos = base.GetGizmos();
      if (compGetGizmos != null) foreach (Gizmo gizmo in compGetGizmos) yield return gizmo;

      // if (DebugSettings.ShowDevGizmos)
      // {
      //   Command_Action commandAction = new Command_Action();
      //   commandAction.defaultLabel = "DEV: Explode";
      //   commandAction.action = new Action(this.Explode);
      //
      //   yield return (Gizmo) commandAction;
      // }
    }
  }

  public class Building_SymbioteCocoonRed : Building_SymbioteCocoon
  {
    protected override string texture => "Things/Buildings/SymbioteCocoon/SymbioteCocoonRed";
    protected override float points => 3000;

  }
}