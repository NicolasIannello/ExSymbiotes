using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ExSymbiotes
{
    public class Verb_SymbioticCannon : Verb
    {
      private List<Vector3> path = new List<Vector3>();
      private List<Vector3> tmpPath = new List<Vector3>();
      private MoteDualAttached mote;
      private Effecter endEffecter;
      private Sustainer sustainer;
      private HashSet<IntVec3> pathCells = new HashSet<IntVec3>();
      private HashSet<IntVec3> tmpPathCells = new HashSet<IntVec3>();
      private HashSet<IntVec3> tmpHighlightCells = new HashSet<IntVec3>();
      protected override int ShotsPerBurst => this.BurstShotCount;
      private Vector3 casterPos => this.caster.Position.ToVector3Shifted().Yto0();
      private Vector3 beamPos; 
      private Vector3 newBeamPos => (currentTarget.CenterVector3.Yto0() - casterPos).normalized; 
      private float rotationSpeed = 2f;
      public Vector3 InterpolatedPosition => casterPos + beamPos * (this.EffectiveRange);
      
      public override void DrawHighlight(LocalTargetInfo target)
      {
        base.DrawHighlight(target);
        this.CalculatePath(target.CenterVector3, this.tmpPath, this.tmpPathCells, false);
        foreach (IntVec3 tmpPathCell in this.tmpPathCells)
        {
          ShootLine resultingLine;
          bool shootLineFromTo = this.TryFindShootLineFromTo(this.caster.Position, target, out resultingLine);
          IntVec3 hitCell;
          if ((!this.verbProps.stopBurstWithoutLos || shootLineFromTo) && this.TryGetHitCell(resultingLine.Source, tmpPathCell, out hitCell))
          {
            this.tmpHighlightCells.Add(hitCell);
          }
        }
        Color? nullable;
        if (this.tmpHighlightCells.Any<IntVec3>())
        {
          List<IntVec3> list = this.tmpHighlightCells.ToList<IntVec3>();
          nullable = this.verbProps.highlightColor;
          Color color = nullable ?? Color.white;
          float? altOffset = new float?();
          GenDraw.DrawFieldEdges(list, color, altOffset);
        }
        this.tmpHighlightCells.Clear();
      }
      
      protected override bool TryCastShot()
      {
        ShootLine resultingLine;
        this.TryFindShootLineFromTo(this.caster.Position, InterpolatedPosition.ToIntVec3(), out resultingLine);
        if (this.EquipmentSource != null)
        {
          this.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
          this.EquipmentSource.GetComp<CompApparelReloadable>()?.UsedOnce();
        }
        this.lastShotTick = Find.TickManager.TicksGame;
        List<IntVec3> points = resultingLine.Points().ToList();
        for (int i = 0; i < points.Count(); i++)
        {
          IntVec3 hitCell;
          if (this.TryGetHitCell(resultingLine.Source, points[i], out hitCell))
          {
            this.HitCell(hitCell, resultingLine.Source);
          }
        }
        return true;
      }

      protected bool TryGetHitCell(IntVec3 source, IntVec3 targetCell, out IntVec3 hitCell)
      {
        IntVec3 a = GenSight.LastPointOnLineOfSight(source, targetCell, (Func<IntVec3, bool>) (c => c.InBounds(this.caster.Map) && c.CanBeSeenOverFast(this.caster.Map)), true);
        if (this.verbProps.beamCantHitWithinMinRange && (double) a.DistanceTo(source) < (double) this.verbProps.minRange)
        {
          hitCell = new IntVec3();
          return false;
        }
        hitCell = a.IsValid ? a : targetCell;
        return a.IsValid;
      }

      public override bool TryStartCastOn(
        LocalTargetInfo castTarg,
        LocalTargetInfo destTarg,
        bool surpriseAttack = false,
        bool canHitNonTargetPawns = true,
        bool preventFriendlyFire = false,
        bool nonInterruptingSelfCast = false)
      {
        return base.TryStartCastOn(this.verbProps.beamTargetsGround ? (LocalTargetInfo) castTarg.Cell : castTarg, destTarg, surpriseAttack, canHitNonTargetPawns, preventFriendlyFire, nonInterruptingSelfCast);
      }

      public override void BurstingTick()
      {
        this.beamPos = Vector3.RotateTowards(this.beamPos, this.newBeamPos, this.rotationSpeed * Mathf.Deg2Rad, 0f);
        Vector3 vector3_1 = this.InterpolatedPosition;
        IntVec3 intVec3_1 = vector3_1.ToIntVec3();
        Vector3 vector3_2 = this.InterpolatedPosition - this.caster.Position.ToVector3Shifted();
        float num1 = vector3_2.MagnitudeHorizontal();
        Vector3 normalized = vector3_2.Yto0().normalized;
        IntVec3 intVec3_2 = GenSight.LastPointOnLineOfSight(this.caster.Position, intVec3_1, (Func<IntVec3, bool>) (c => c.CanBeSeenOverFast(this.caster.Map)), true);
        IntVec3 intVec3_3;
        if (intVec3_2.IsValid)
        {
          double num2 = (double) num1;
          intVec3_3 = intVec3_1 - intVec3_2;
          double lengthHorizontal = (double) intVec3_3.LengthHorizontal;
          num1 = (float) (num2 - lengthHorizontal);
          intVec3_3 = this.caster.Position;
          vector3_1 = intVec3_3.ToVector3Shifted() + normalized * num1;
          intVec3_1 = vector3_1.ToIntVec3();
        }
        Vector3 offsetA = normalized * this.verbProps.beamStartOffset;
        Vector3 vector3_3 = vector3_1 - intVec3_1.ToVector3Shifted();
        if (this.mote != null)
        {
          this.mote.UpdateTargets(new TargetInfo(this.caster.Position, this.caster.Map), new TargetInfo(intVec3_1, this.caster.Map), offsetA, vector3_3);
          this.mote.Maintain();
        }
        if (this.verbProps.beamGroundFleckDef != null)
        {
          ShootLine resultingLine;
          this.TryFindShootLineFromTo(this.caster.Position, InterpolatedPosition.ToIntVec3(), out resultingLine);
          List<IntVec3> points = resultingLine.Points().ToList();
          for (int i = 0; i < points.Count(); i++)
          {
            if(Rand.Chance(this.verbProps.beamFleckChancePerTick))
              if (GenSight.LineOfSight(this.caster.Position, points[i], this.caster.Map))
                FleckMaker.Static(points[i], this.caster.Map, this.verbProps.beamGroundFleckDef, 3f);
              else
                break;
          }
        }
        if (this.verbProps.beamGroundFleckDef != null && Rand.Chance(this.verbProps.beamFleckChancePerTick))
          FleckMaker.Static(vector3_1, this.caster.Map, this.verbProps.beamGroundFleckDef);
        if (this.endEffecter == null && this.verbProps.beamEndEffecterDef != null)
          this.endEffecter = this.verbProps.beamEndEffecterDef.Spawn(intVec3_1, this.caster.Map, vector3_3);
        if (this.endEffecter != null)
        {
          this.endEffecter.offset = vector3_3;
          this.endEffecter.EffectTick(new TargetInfo(intVec3_1, this.caster.Map), TargetInfo.Invalid);
          --this.endEffecter.ticksLeft;
        }
        if (this.verbProps.beamLineFleckDef != null)
        {
          float num3 = 1f * num1;
          for (int index = 0; (double) index < (double) num3; ++index)
          {
            if (Rand.Chance(this.verbProps.beamLineFleckChanceCurve.Evaluate((float) index / num3)))
            {
              Vector3 vector3_4 = (float) index * normalized - normalized * Rand.Value + normalized / 2f;
              intVec3_3 = this.caster.Position;
              FleckMaker.Static(intVec3_3.ToVector3Shifted() + vector3_4, this.caster.Map, this.verbProps.beamLineFleckDef);
            }
          }
        }
        this.sustainer?.Maintain();
      }

      public override void WarmupComplete()
      {
        this.burstShotsLeft = this.ShotsPerBurst;
        this.state = VerbState.Bursting;
        this.beamPos = (this.currentTarget.CenterVector3.Yto0() - casterPos).normalized;
        this.CalculatePath(this.currentTarget.CenterVector3, this.path, this.pathCells);
        if (this.verbProps.beamMoteDef != null)
          this.mote = MoteMaker.MakeInteractionOverlay(this.verbProps.beamMoteDef, (TargetInfo) this.caster, new TargetInfo(this.path[0].ToIntVec3(), this.caster.Map));
        this.TryCastNextBurstShot();
        this.endEffecter?.Cleanup();
        if (this.verbProps.soundCastBeam == null)
          return;
        this.sustainer = this.verbProps.soundCastBeam.TrySpawnSustainer(SoundInfo.InMap((TargetInfo) this.caster, MaintenanceType.PerTick));
      }

      private void CalculatePath(Vector3 target, List<Vector3> pathList, HashSet<IntVec3> pathCellsList, bool addRandomOffset = true)
      {
        pathList.Clear();
        Vector3 direction = (target.Yto0() - casterPos).normalized;
        for (int index = 0; index < EffectiveRange; ++index)
        {
          Vector3 path = casterPos + direction * index;
          pathList.Add(path);
        }
        pathCellsList.Clear();
        foreach (Vector3 path in pathList) pathCellsList.Add(path.ToIntVec3());
      }

      private bool CanHit(Thing thing)
      {
        return thing.Spawned && !CoverUtility.ThingCovered(thing, this.caster.Map);
      }

      private void HitCell(IntVec3 cell, IntVec3 sourceCell, float damageFactor = 1f)
      {
        if (!cell.InBounds(this.caster.Map))
          return;
        this.ApplyDamage(VerbUtility.ThingsToHit(cell, this.caster.Map, new Func<Thing, bool>(this.CanHit)).RandomElementWithFallback<Thing>(), sourceCell, damageFactor);
        if (!this.verbProps.beamSetsGroundOnFire || !Rand.Chance(this.verbProps.beamChanceToStartFire))
          return;
        FireUtility.TryStartFireIn(cell, this.caster.Map, 1f, this.caster);
      }

      private void ApplyDamage(Thing thing, IntVec3 sourceCell, float damageFactor = 1f)
      {
        IntVec3 intVec3_1 = this.InterpolatedPosition.Yto0().ToIntVec3();
        IntVec3 intVec3_2 = GenSight.LastPointOnLineOfSight(sourceCell, intVec3_1, (Func<IntVec3, bool>) (c => c.InBounds(this.caster.Map) && c.CanBeSeenOverFast(this.caster.Map)), true);
        if (intVec3_2.IsValid)
          intVec3_1 = intVec3_2;
        Map map = this.caster.Map;
        if (thing == null || this.verbProps.beamDamageDef == null)
          return;
        float angleFlat = (this.currentTarget.Cell - this.caster.Position).AngleFlat;
        BattleLogEntry_RangedImpact log = new BattleLogEntry_RangedImpact(this.caster, thing, this.currentTarget.Thing, this.EquipmentSource.def, (ThingDef) null, (ThingDef) null);
        if (!(thing is Pawn)) damageFactor += 3f;
        DamageInfo dinfo = new DamageInfo(this.verbProps.beamDamageDef, this.verbProps.beamTotalDamage * damageFactor, this.verbProps.beamDamageDef.defaultArmorPenetration, angleFlat, this.caster, weapon: this.EquipmentSource.def, intendedTarget: this.currentTarget.Thing);
        thing.TakeDamage(dinfo).AssociateWithLog((LogEntry_DamageResult) log);
        if (thing.CanEverAttachFire())
        {
          if (!Rand.Chance(this.verbProps.flammabilityAttachFireChanceCurve == null ? this.verbProps.beamChanceToAttachFire : this.verbProps.flammabilityAttachFireChanceCurve.Evaluate(thing.GetStatValue(StatDefOf.Flammability))))
            return;
          thing.TryAttachFire(this.verbProps.beamFireSizeRange.RandomInRange, this.caster);
        }
        else
        {
          if (!Rand.Chance(this.verbProps.beamChanceToStartFire))
            return;
          FireUtility.TryStartFireIn(intVec3_1, map, this.verbProps.beamFireSizeRange.RandomInRange, this.caster, this.verbProps.flammabilityAttachFireChanceCurve);
        }
      }

      public void ChangeTarget(LocalTargetInfo target)
      {
        this.currentTarget = target; 
      }
      
      public override void ExposeData()
      {
        base.ExposeData();
        Scribe_Collections.Look<Vector3>(ref this.path, "path", LookMode.Value);
        Scribe_Values.Look<Vector3>(ref this.beamPos, "beamPos");
        if (Scribe.mode != LoadSaveMode.PostLoadInit || this.path != null)
          return;
        this.path = new List<Vector3>();
      }
    }
}