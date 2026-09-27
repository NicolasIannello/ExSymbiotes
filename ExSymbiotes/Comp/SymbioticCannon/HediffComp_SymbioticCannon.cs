using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ExSymbiotes
{
    public class HediffComp_SymbioticCannon : HediffComp, ILoadReferenceable
    {
        public HediffCompProperties_SymbioticCannon Props => (HediffCompProperties_SymbioticCannon) this.props;
        private LocalTargetInfo lastAttackedTarget;
        private int lastAttackTargetTick;
        protected LocalTargetInfo forcedTarget = LocalTargetInfo.Invalid;
        public string GetUniqueLoadID() => "ExSymbiotes_HediffComp_SymbioticCannon" + this.Pawn.ThingID;
        public Verb CurrentEffectiveVerb => this.AttackVerb;
        public LocalTargetInfo LastAttackedTarget => this.lastAttackedTarget;
        public int LastAttackTargetTick => this.lastAttackTargetTick;
        public LocalTargetInfo TargetCurrentlyAimingAt => this.CurrentTarget;
        public float TargetPriorityFactor => 1f;
        
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            
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
        private bool burstActivated;
        public Thing gun;
        public bool Active => this.burstActivated;
        public CompEquippable GunCompEq => this.gun.TryGetComp<CompEquippable>();
        public LocalTargetInfo CurrentTarget => this.currentTargetInt;
        private bool WarmingUp => this.burstWarmupTicksLeft > 0;
        public Verb AttackVerb => this.GunCompEq.PrimaryVerb;
        private bool PlayerControlled => this.Pawn.Faction == Faction.OfPlayer && !this.Pawn.Downed;
        protected virtual bool CanSetForcedTarget => this.PlayerControlled;

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
                    }
                }
            }
        }
        
        public void TryStartShootSomething(bool canBeginBurstImmediately)
        {
            if (!this.Pawn.Spawned || this.Pawn.Downed || this.Pawn.stances.stunner.Stunned || this.Pawn.stances.stagger.Staggered && !this.AttackVerb.Available())
            {
                this.ResetCurrentTarget();
            }
            else
            {
                this.currentTargetInt = this.forcedTarget;
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
        
        protected virtual void BeginBurst()
        {
            this.BurstComplete();
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
                gizmo.onHover = () =>
                {
                    GenDraw.DrawRadiusRing(this.Pawn.Position, this.AttackVerb.EffectiveRange, Color.white);
                };
                gizmo.defaultLabel = (string) "ExSymbiotes.CommandSetForceAttackTarget".Translate();
                gizmo.icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/Attack");
                gizmo.action = delegate
                {
                    TargetingParameters targetParams = this.AttackVerb.targetParams ?? TargetingParameters.ForAttackAny();
                    Find.Targeter.BeginTargeting(targetParams:targetParams, action: delegate(LocalTargetInfo target)
                    {
                        this.OrderAttack(target);
                    }, highlightAction: delegate(LocalTargetInfo target)
                    {
                        this.AttackVerb.DrawHighlight(target);
                        GenDraw.DrawRadiusRing(this.Pawn.Position, this.AttackVerb.EffectiveRange, Color.white);
                    }, null, caster: this.Pawn);
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
                gizmo.defaultLabel = (string) "ExSymbiotes.CommandStopForceAttack".Translate();
                gizmo.icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/Halt");
                gizmo.action = (Action) (() =>
                {
                  this.ResetForcedTarget();
                  SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                });
                if (!this.forcedTarget.IsValid)
                  gizmo.Disable((string) "CommandStopAttackFailNotForceAttacking".Translate());
                yield return (Gizmo) gizmo;
            }
        }
        
        private void ResetForcedTarget()
        {
            this.forcedTarget = null;
            this.burstWarmupTicksLeft = 0;
            this.ResetCurrentTarget();
        }
        
        private void ResetCurrentTarget()
        {
            this.currentTargetInt = null;
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
            }
        }
    }
}