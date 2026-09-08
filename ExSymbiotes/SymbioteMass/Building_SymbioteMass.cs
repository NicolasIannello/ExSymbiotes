using System;
using System.Collections.Generic;
using ExSymbiotes.Utils;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using Verse.AI.Group;

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
    private bool red = false;
    public int texture = 0;
    private Lord defendHeartLord;
    private float damageTaken = 0;
    private int damageLoop = 0;
    private int defendTick = -99999;
    private Graphic CenterPartGraphic
    {
      get
      {
        return this.cachedCenterPartGraphic ?? (this.cachedCenterPartGraphic = GraphicDatabase.Get<Graphic_Multi>(texture==0 ? "Things/Buildings/SymbioteMass/SymbioteMass" : "Things/Buildings/SymbioteMass/SymbioteMassRed", ShaderDatabase.Cutout, (Vector2) Building_SymbioteMass.PartDrawSize, Color.white));
      }
    }
    public Lord DefendHeartLord
    {
      get
      {
        if (this.defendHeartLord == null)
          this.defendHeartLord = SymbioteUtility.GetSymbioteLord(this.Map);
        return this.defendHeartLord;
      }
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look<int>(ref this.bpmAccel, "bpmAccel");
      Scribe_Values.Look<int>(ref this.bpm, "bpm");
      Scribe_Values.Look<int>(ref this.overloadTick, "overloadTick");
      Scribe_Values.Look<int>(ref this.defendTick, "defendTick");
      Scribe_References.Look<Lord>(ref this.defendHeartLord, "defendHeartLord");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
      base.SpawnSetup(map, respawningAfterLoad);
      if (respawningAfterLoad || this.BeingTransportedOnGravship)
        return;
      if (this.Faction != Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes))
        this.SetFaction(Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes), (Pawn) null);
      EffecterDefOf.ImpactDustCloud.Spawn(this.Position, this.Map).Cleanup();
    }

    public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
    {
      if (!red || texture==1)
      {
        if (study) GenPlace.TryPlaceThing(GetReward(texture==0 ? "ExSymbiotes_SymbioticCore" : "ExSymbiotes_SymbioticCore_Red", 1), this.Position, this.Map, ThingPlaceMode.Near);
        else
        {
          GenPlace.TryPlaceThing(GetReward("Shard", 10), this.Position, this.Map, ThingPlaceMode.Near);
          GenPlace.TryPlaceThing(GetReward(texture==0 ? "ExSymbiotes_SymbioticTissue" : "ExSymbiotes_SymbioticTissue_Red", 50), this.Position, this.Map, ThingPlaceMode.Near);
        }
      }
      else
      {
        Map map = this.Map;
        IntVec3 pos = this.Position;
        Thing redMass = ThingMaker.MakeThing(ExSymbiotesDefOf.ExSymbiotes_SymbioteMass_Red); 
        ((Building_SymbioteMass)redMass).texture = 1;
        
        base.Kill(dinfo, exactCulprit);

        GenPlace.TryPlaceThing(redMass, pos, map, ThingPlaceMode.Direct, null, null, new Rot4?(redMass.Rotation));

        return;
      }

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

      if (this.IsHashIntervalTick(300) && Find.TickManager.TicksGame > this.defendTick)
      {
        if (damageTaken > 0)
        {
          if (damageTaken>500 || damageLoop > 2) DefendMode();
          damageLoop++;
        }
        else if (damageLoop > 0) damageLoop--;
        damageTaken = 0;
      }
    }

    public void DefendMode()
    {
      ((LordJob_SymbioteMass)DefendHeartLord.LordJob).DefendMode(this.Position);
      damageLoop = 0;
      defendTick = Find.TickManager.TicksGame + 15000;
    }

    public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
      base.PostApplyDamage(dinfo, totalDamageDealt);
      if (!this.Spawned)
        return;
      damageTaken += totalDamageDealt;
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
      if (texture == 1)
      {
        this.overloadTick = Find.TickManager.TicksGame + EffecterDefOf.TachycardiacArrest.maintainTicks;
        EffecterDefOf.TachycardiacArrest.SpawnMaintained(this.Position, this.Map);
      }
      else
      {
        this.overloadTick = Find.TickManager.TicksGame + ExSymbiotesDefOf.ExSymbiotes_TachycardiacArrest.maintainTicks;
        ExSymbiotesDefOf.ExSymbiotes_TachycardiacArrest.SpawnMaintained(this.Position, this.Map);
      }
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
    
    public void SpawnRedMass()
    {
      this.red = true;
      this.bpm *= 2;
      this.bpmAccel = 15;
      this.overloadTick = Find.TickManager.TicksGame + EffecterDefOf.TachycardiacArrest.maintainTicks;
      EffecterDefOf.TachycardiacArrest.SpawnMaintained(this.Position, this.Map);
      Messages.Message((string) "ExSymbiotes.MessageRedMass".Translate(), (LookTargets) (Thing) this, MessageTypeDefOf.NegativeEvent);
    }
    
    public override void PostSwapMap()
    {
      base.PostSwapMap();
      this.defendHeartLord = (Lord) null;
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
      IEnumerable<Gizmo> compGetGizmos = base.GetGizmos();
      if (compGetGizmos != null) foreach (Gizmo gizmo in compGetGizmos) yield return gizmo;

      if (DebugSettings.ShowDevGizmos)
      {
        Command_Action commandAction = new Command_Action();
        commandAction.defaultLabel = "DEV: Defend";
        commandAction.action = (Action) (() =>
        {
          ((LordJob_SymbioteMass)DefendHeartLord.LordJob).DefendMode(this.Position);
        });
        yield return (Gizmo) commandAction;
      }
    }
  }
}