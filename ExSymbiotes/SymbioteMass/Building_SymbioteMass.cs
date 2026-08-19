using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace ExSymbiotes
{
  public class Building_SymbioteMass : Building
  {
    private static readonly Vector3 PartDrawSize = new Vector3(3.5f, 3.5f, 3.5f);
    private int bpm = 30;
    private int bpmAccel;
    private int overloadTick = -99999;
    [Unsaved(false)]
    private int lastBeatTick = -99999;
    [Unsaved(false)]
    private Graphic cachedCenterPartGraphic;
    private bool study = false;

    private Graphic CenterPartGraphic
    {
      get
      {
        return this.cachedCenterPartGraphic ?? (this.cachedCenterPartGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Buildings/SymbioteMass/SymbioteMass", ShaderDatabase.Cutout, (Vector2) Building_SymbioteMass.PartDrawSize, Color.white));
      }
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look<int>(ref this.bpmAccel, "bpmAccel");
      Scribe_Values.Look<int>(ref this.bpm, "bpm");
      Scribe_Values.Look<int>(ref this.overloadTick, "overloadTick");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
      base.SpawnSetup(map, respawningAfterLoad);
      if (respawningAfterLoad || this.BeingTransportedOnGravship)
        return;
      if (this.Faction != Faction.OfEntities)
        this.SetFaction(Faction.OfEntities, (Pawn) null);
      EffecterDefOf.ImpactDustCloud.Spawn(this.Position, this.Map).Cleanup();
    }

    public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
    {
      GenPlace.TryPlaceThing(GetReward(study ? "VoidsightSerum" : "Shard", study ? 1 : 5), this.Position, this.Map, ThingPlaceMode.Near);
      base.Kill(dinfo, exactCulprit);
    }

    private Thing GetReward(string itemDefName, int stack)
    {
      ThingDef itemDef = ThingDef.Named(itemDefName);
      Thing item = ThingMaker.MakeThing(itemDef);
      item.stackCount = stack;

      return item;
    }
    
    protected override void Tick()
    {
      base.Tick();
      if ((double) Find.TickManager.TicksGame > (double) this.lastBeatTick + 60.0 / (double) this.bpm * 60.0)
        this.Beat();
      Thing.allowDestroyNonDestroyable = true;
      if (this.overloadTick > 0 && Find.TickManager.TicksGame > this.overloadTick)
        this.Kill(new DamageInfo?(), (Hediff) null);
      Thing.allowDestroyNonDestroyable = false;
    }

    private void Beat()
    {
      this.lastBeatTick = Find.TickManager.TicksGame;
      this.bpm += this.bpmAccel;
      SoundDefOf.FleshmassHeart_Throb.PlayOneShot((SoundInfo) (Thing) this);
    }

    public void StartTachycardiacOverload(bool study)
    {
      this.study = study;
      this.bpm *= 2;
      this.bpmAccel = 15;
      this.overloadTick = Find.TickManager.TicksGame + EffecterDefOf.TachycardiacArrest.maintainTicks;
      EffecterDefOf.TachycardiacArrest.SpawnMaintained(this.Position, this.Map);
      Messages.Message((string) "ExSymbiotes.MessageHeartAttack".Translate(), (LookTargets) (Thing) this, MessageTypeDefOf.PositiveEvent);
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
    
  }
}