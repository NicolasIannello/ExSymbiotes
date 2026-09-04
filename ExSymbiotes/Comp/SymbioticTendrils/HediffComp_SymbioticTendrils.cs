using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ExSymbiotes
{
    public class HediffComp_SymbioticTendrils: HediffComp, 
        IAttackTarget, 
        ILoadReferenceable, 
        IAttackTargetSearcher
    {
        public HediffCompProperties_SymbioticTendrils Props => (HediffCompProperties_SymbioticTendrils) this.props;
        private LocalTargetInfo lastAttackedTarget;
        private int lastAttackTargetTick;
        public virtual bool IsEverThreat => true;
        protected LocalTargetInfo forcedTarget = LocalTargetInfo.Invalid;
        
        public string GetUniqueLoadID() => "ExSymbiotes_HediffComp_SymbioticTendrils" + this.Pawn.ThingID;
        
        public bool ThreatDisabled(IAttackTargetSearcher disabledFor) => !this.IsEverThreat;

        Thing IAttackTarget.Thing => (Thing) this.Pawn;

        public Verb CurrentEffectiveVerb => this.AttackVerb;
        public LocalTargetInfo LastAttackedTarget => this.lastAttackedTarget;
        public int LastAttackTargetTick => this.lastAttackTargetTick;
        public LocalTargetInfo TargetCurrentlyAimingAt => this.CurrentTarget;
        public float TargetPriorityFactor => 1f;

        Thing IAttackTargetSearcher.Thing => (Thing) this.Pawn;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            
            if (this.forcedTarget.IsValid && !this.CanSetForcedTarget)
                this.ResetForcedTarget();
            if (!this.CanToggleHoldFire)
                this.holdFire = false;
            if (this.forcedTarget.ThingDestroyed)
                this.ResetForcedTarget();
            if (!this.Active && this.Pawn.Spawned)
            {
                this.GunCompEq.verbTracker.VerbsTick();
                if (this.AttackVerb.state == VerbState.Bursting)
                    return;
                this.burstActivated = false;
                if (this.WarmingUp)
                {
                    --this.burstWarmupTicksLeft;
                    if (this.burstWarmupTicksLeft <= 0)
                        this.BeginBurst();
                }
                else
                {
                    if (this.burstCooldownTicksLeft > 0)
                    {
                        --this.burstCooldownTicksLeft;
                    }
                    if (this.burstCooldownTicksLeft <= 0 && this.Pawn.IsHashIntervalTick(15))
                        this.TryStartShootSomething(true);
                }
            }
            else
                this.ResetCurrentTarget();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_TargetInfo.Look(ref this.forcedTarget, "forcedTarget");
            Scribe_TargetInfo.Look(ref this.lastAttackedTarget, "lastAttackedTarget");
            Scribe_Values.Look<int>(ref this.lastAttackTargetTick, "lastAttackTargetTick");
            
            Scribe_Values.Look<int>(ref this.burstCooldownTicksLeft, "burstCooldownTicksLeft");
            Scribe_Values.Look<int>(ref this.burstWarmupTicksLeft, "burstWarmupTicksLeft");
            Scribe_TargetInfo.Look(ref this.currentTargetInt, "currentTarget");
            Scribe_Values.Look<bool>(ref this.holdFire, "holdFire");
            Scribe_Values.Look<bool>(ref this.burstActivated, "burstActivated");
            Scribe_Deep.Look<Thing>(ref this.gun, "gun");
            BackCompatibility.PostExposeData((object) this);
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            if (this.gun == null)
            {
                Log.Error("Turret had null gun after loading. Recreating.");
                this.MakeGun();
            }
            else
                this.UpdateGunVerbs();
        }
        
        protected void OnAttackedTarget(LocalTargetInfo target)
        {
            this.lastAttackTargetTick = Find.TickManager.TicksGame;
            this.lastAttackedTarget = target;
        }
        
        protected int burstCooldownTicksLeft;
        protected int burstWarmupTicksLeft;
        protected LocalTargetInfo currentTargetInt = LocalTargetInfo.Invalid;
        private bool holdFire;
        private bool burstActivated;
        public Thing gun;
        public bool Active => this.burstActivated;
        public CompEquippable GunCompEq => this.gun.TryGetComp<CompEquippable>();
        public LocalTargetInfo CurrentTarget => this.currentTargetInt;
        private bool WarmingUp => this.burstWarmupTicksLeft > 0;
        public Verb AttackVerb => this.GunCompEq.PrimaryVerb;
        private bool PlayerControlled => this.Pawn.Faction == Faction.OfPlayer && !this.Pawn.Downed;
        protected virtual bool CanSetForcedTarget => this.PlayerControlled;
        private bool CanToggleHoldFire => this.PlayerControlled;

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.burstCooldownTicksLeft = 30;
            this.MakeGun();
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            this.ResetCurrentTarget();
        }
        
        public void OrderAttack(LocalTargetInfo targ)
        {
            if (!targ.IsValid)
            {
                if (!this.forcedTarget.IsValid)
                    return;
                this.ResetForcedTarget();
            }
            else
            {
                IntVec3 intVec3 = targ.Cell - this.Pawn.Position;
                if ((double) intVec3.LengthHorizontal < (double) this.AttackVerb.verbProps.EffectiveMinRange(targ, (Thing) this.Pawn))
                {
                    Messages.Message((string) "MessageTargetBelowMinimumRange".Translate(), (LookTargets) (Thing) this.Pawn, MessageTypeDefOf.RejectInput, false);
                }
                else
                {
                    intVec3 = targ.Cell - this.Pawn.Position;
                    if ((double) intVec3.LengthHorizontal > (double) this.AttackVerb.EffectiveRange)
                    {
                        Messages.Message((string) "MessageTargetBeyondMaximumRange".Translate(), (LookTargets) (Thing) this.Pawn, MessageTypeDefOf.RejectInput, false);
                    }
                    else
                    {
                        if (this.forcedTarget != targ)
                        {
                            this.forcedTarget = targ;
                            if (this.burstCooldownTicksLeft <= 0)
                                this.TryStartShootSomething(false);
                        }
                        if (!this.holdFire)
                            return;
                        Messages.Message((string) "MessageTurretWontFireBecauseHoldFire".Translate((NamedArgument) this.Pawn.def.label), (LookTargets) (Thing) this.Pawn, MessageTypeDefOf.RejectInput, false);
                    }
                }
            }
        }
        
        public void TryStartShootSomething(bool canBeginBurstImmediately)
        {
            if (!this.Pawn.Spawned || this.holdFire && this.CanToggleHoldFire && !this.AttackVerb.Available())
            {
                this.ResetCurrentTarget();
            }
            else
            {
                int num = this.currentTargetInt.IsValid ? 1 : 0;
                this.currentTargetInt = !this.forcedTarget.IsValid ? this.TryFindNewTarget() : this.forcedTarget;
                if (this.currentTargetInt.IsValid)
                {
                    if (canBeginBurstImmediately)
                        this.BeginBurst();
                    else
                        this.burstWarmupTicksLeft = 1;
                }
                else
                    this.ResetCurrentTarget();
            }
        }
        
        public virtual LocalTargetInfo TryFindNewTarget()
        {
            IAttackTargetSearcher searcher = this.TargSearcher();
            Faction faction = searcher.Thing.Faction;
            float range = this.AttackVerb.EffectiveRange;
            Building result;
            if ((double) Rand.Value < 0.5 && this.AttackVerb.ProjectileFliesOverhead() && faction.HostileTo(this.Pawn.Faction) && this.Pawn.Map.listerBuildings.allBuildingsColonist.Where<Building>((Func<Building, bool>) (x =>
                {
                    float num = this.AttackVerb.verbProps.EffectiveMinRange((LocalTargetInfo) (Thing) x, (Thing) this.Pawn);
                    float squared = (float) x.Position.DistanceToSquared(this.Pawn.Position);
                    return (double) squared > (double) num * (double) num && (double) squared < (double) range * (double) range;
                })).TryRandomElement<Building>(out result))
                return (LocalTargetInfo) (Thing) result;
            TargetScanFlags flags = TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
            if (!this.AttackVerb.ProjectileFliesOverhead())
                flags = flags | TargetScanFlags.NeedLOSToAll | TargetScanFlags.LOSBlockableByGas;
            if (this.AttackVerb.IsIncendiary_Ranged())
                flags |= TargetScanFlags.NeedNonBurning;
            return (LocalTargetInfo) (Thing) AttackTargetFinder.BestShootTargetFromCurrentPosition(searcher, flags, new Predicate<Thing>(this.IsValidTarget));
        }
        
        private IAttackTargetSearcher TargSearcher()
        {
            return (IAttackTargetSearcher) this.Pawn;
        }
        
        private bool IsValidTarget(Thing t)
        {
            if (t is Pawn p)
            {
                if (this.Pawn.Faction == Faction.OfPlayer && p.IsPrisoner)
                    return false;
                if (this.AttackVerb.ProjectileFliesOverhead())
                {
                    RoofDef roofDef = this.Pawn.Map.roofGrid.RoofAt(t.Position);
                    if (roofDef != null && roofDef.isThickRoof)
                        return false;
                }
                if (p.RaceProps.Animal && p.Faction == this.Pawn.Faction)
                    return false;
            }
            return true;
        }
        
        protected virtual void BeginBurst()
        {
            this.AttackVerb.TryStartCastOn(this.CurrentTarget);
            this.OnAttackedTarget(this.CurrentTarget);
        }
        
        protected virtual void BurstComplete()
        {
            this.burstCooldownTicksLeft = this.BurstCooldownTime().SecondsToTicks();
        }
        
        protected virtual float BurstCooldownTime()
        {
            return this.Props.cd;
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            IEnumerable<Gizmo> compGetGizmos = base.CompGetGizmos();
            if (compGetGizmos != null) foreach (Gizmo gizmo in compGetGizmos) yield return gizmo;
            CompChangeableProjectile comp1 = this.gun.TryGetComp<CompChangeableProjectile>();
            if (comp1 != null)
            {
                foreach (Gizmo gizmo in StorageSettingsClipboard.CopyPasteGizmosFor(comp1.GetStoreSettings()))
                    yield return gizmo;
            }
            if (this.CanSetForcedTarget)
            {
                Command_Action gizmo = new Command_Action();
                gizmo.defaultLabel = (string) "CommandSetForceAttackTarget".Translate();
                gizmo.defaultDesc = (string) "CommandSetForceAttackTargetDesc".Translate();
                gizmo.icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/Attack");
                gizmo.hotKey = KeyBindingDefOf.Misc4;
                gizmo.action = delegate
                {
                    TargetingParameters targetParams = this.AttackVerb.targetParams ?? TargetingParameters.ForAttackAny();
                    Find.Targeter.BeginTargeting(targetParams, delegate(LocalTargetInfo target)
                    {
                        this.OrderAttack(target);
                    }, this.Pawn);
                };
                if (this.Pawn.Spawned)
                {
                    float weatherMaxRangeCap = this.Pawn.Map.weatherManager.CurWeatherMaxRangeCap;
                    if ((double) weatherMaxRangeCap > 0.0 && (double) weatherMaxRangeCap < (double) this.AttackVerb.verbProps.minRange)
                      gizmo.Disable((string) ("CannotFire".Translate() + ": " + this.Pawn.Map.weatherManager.curWeather.LabelCap));
                }
                yield return (Gizmo) gizmo;
            }
            if (this.forcedTarget.IsValid)
            {
                Command_Action gizmo = new Command_Action();
                gizmo.defaultLabel = (string) "CommandStopForceAttack".Translate();
                gizmo.defaultDesc = (string) "CommandStopForceAttackDesc".Translate();
                gizmo.icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/Halt");
                gizmo.action = (Action) (() =>
                {
                  this.ResetForcedTarget();
                  SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                });
                if (!this.forcedTarget.IsValid)
                  gizmo.Disable((string) "CommandStopAttackFailNotForceAttacking".Translate());
                gizmo.hotKey = KeyBindingDefOf.Misc5;
                yield return (Gizmo) gizmo;
            }
            
            if (this.CanToggleHoldFire)
            {
              Command_Toggle gizmo = new Command_Toggle();
              gizmo.defaultLabel = (string) "CommandHoldFire".Translate();
              gizmo.defaultDesc = (string) "CommandHoldFireDesc".Translate();
              gizmo.icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/HoldFire");
              gizmo.hotKey = KeyBindingDefOf.Misc6;
              gizmo.toggleAction = (Action) (() =>
              {
                this.holdFire = !this.holdFire;
                if (!this.holdFire)
                  return;
                this.ResetForcedTarget();
              });
              gizmo.isActive = (Func<bool>) (() => this.holdFire);
              yield return (Gizmo) gizmo;
            }            
        }
        
        private void ResetForcedTarget()
        {
            this.forcedTarget = LocalTargetInfo.Invalid;
            this.burstWarmupTicksLeft = 0;
            if (this.burstCooldownTicksLeft > 0)
                return;
            this.TryStartShootSomething(false);
        }
        
        private void ResetCurrentTarget()
        {
            this.currentTargetInt = LocalTargetInfo.Invalid;
            this.burstWarmupTicksLeft = 0;
        }
        
        public void MakeGun()
        {
            this.gun = ThingMaker.MakeThing(Props.turretGunDef);
            this.UpdateGunVerbs();
        }
        
        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = this.gun.TryGetComp<CompEquippable>().AllVerbs;
            for (int index = 0; index < allVerbs.Count; ++index)
            {
                Verb verb = allVerbs[index];
                verb.caster = (Thing) this.Pawn;
                verb.castCompleteCallback = new Action(this.BurstComplete);
            }
        }
    }
}