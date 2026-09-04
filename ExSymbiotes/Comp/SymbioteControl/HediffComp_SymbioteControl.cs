using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class HediffComp_SymbioteControl: HediffComp, IThingHolder
    {
        private ThingOwner<Thing> innerContainer;
        public IThingHolder ParentHolder => this.Pawn.ParentHolder;
        public Thing SymbioteControlling
        {
            get
            {
                return this.innerContainer.InnerListForReading.Count <= 0 ? (Thing) null : this.innerContainer.InnerListForReading[0];
            }
        }
        public static readonly ConditionalWeakTable<Pawn, HediffComp_SymbioteControl> SymbioteControlWeakTable = new ConditionalWeakTable<Pawn, HediffComp_SymbioteControl>();

        public HediffComp_SymbioteControl()
        {
            this.innerContainer = new ThingOwner<Thing>((IThingHolder) this, LookMode.Deep, false);
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            Pawn.mindState.mentalStateHandler.Reset();
            Pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.BerserkPermanent, "A symbiote has taken control over his body", forced: true, forceWake: true);
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            Thing symbiote = DropPawn(Pawn.MapHeld);
            DamageInfo dinfo = new DamageInfo(DamageDefOf.AcidBurn, (float) 50, instigator: (Thing) symbiote);
            dinfo.SetApplyAllDamage(true);
            symbiote.TakeDamage(dinfo);
            Pawn.mindState.mentalStateHandler.Reset();
            SymbioteControlWeakTable.Remove(this.Pawn);
            Messages.Message("The symbiote is leaving "+Pawn.Name+"'s body cause of its injuries", (LookTargets) (Thing) Pawn, MessageTypeDefOf.NegativeEvent);
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            if(Pawn.Downed) this.parent.pawn.health.RemoveHediff(parent);
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            this.parent.pawn.health.RemoveHediff(parent);
        }

        public void AddThing(Thing thing)
        {
            thing.DeSpawn(DestroyMode.Vanish);
            this.innerContainer.TryAdd((Thing) thing, true);
            if (SymbioteControlling.def != ExSymbiotesDefOf.ExSymbiotes_Symbiote)
            {
                SymbioteControlWeakTable.Remove(this.Pawn);
                SymbioteControlWeakTable.Add(this.Pawn, this);
            }
        }
        
        private Pawn DropPawn(Map map)
        {
            Thing lastResultingThing;
            if (!this.innerContainer.TryDrop(this.SymbioteControlling, this.Pawn.PositionHeld, map, ThingPlaceMode.Near, out lastResultingThing, (Action<Thing, int>) null, (Predicate<IntVec3>) null))
            {
                IntVec3 result;
                if (RCellFinder.TryFindRandomCellNearWith(this.Pawn.PositionHeld, (Predicate<IntVec3>) (c => c.Standable(map)), map, out result, 1))
                {
                    lastResultingThing = GenSpawn.Spawn(this.innerContainer.Take(this.SymbioteControlling), result, map);
                }
                else
                {
                    Debug.LogError((object) "Could not drop controlling symbiote!");
                    return (Pawn) null;
                }
            }
            if (lastResultingThing is Corpse corpse)
                return corpse.InnerPawn;
            Pawn pawn = (Pawn) lastResultingThing;
            pawn.stances.stunner.StunFor(15, (Thing) this.Pawn, false, false);
            return pawn;
        }
        
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, (IList<Thing>) this.GetDirectlyHeldThings());
        }
        
        public ThingOwner GetDirectlyHeldThings() => (ThingOwner) this.innerContainer;
        
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Deep.Look<ThingOwner<Thing>>(ref this.innerContainer, "innerContainer", (object) this);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.Pawn != null && SymbioteControlling.def != ExSymbiotesDefOf.ExSymbiotes_Symbiote) 
                SymbioteControlWeakTable.Add(this.Pawn, this);
            if (Scribe.mode != LoadSaveMode.PostLoadInit || !this.innerContainer.removeContentsIfDestroyed)
                return;
            this.innerContainer.removeContentsIfDestroyed = false;
        }
    }
}