using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class HediffComp_SymbioteControl: HediffComp_SymbioteBase, IThingHolder
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
        private Faction originalFaction;
        public HediffCompProperties_SymbioteControl Props => (HediffCompProperties_SymbioteControl) this.props;

        public HediffComp_SymbioteControl()
        {
            this.innerContainer = new ThingOwner<Thing>((IThingHolder) this, LookMode.Deep, false);
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            originalFaction=Pawn.Faction;
            this.Pawn.SetFaction(Find.FactionManager.FirstFactionOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiotes));
            if (Pawn.RaceProps.Humanlike) Pawn.story.skinColorOverride = Props.color;
            skin = Props.color;
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            Thing symbiote = DropPawn(Pawn.MapHeld);
            DamageInfo dinfo = new DamageInfo(DamageDefOf.AcidBurn, (float) 50, instigator: (Thing) symbiote);
            dinfo.SetApplyAllDamage(true);
            symbiote.TakeDamage(dinfo);
            this.Pawn.SetFaction(originalFaction);
            if (Pawn.RaceProps.Humanlike) Pawn.story.skinColorOverride = null;
            if(ConditionalWeakTableTryGet(this.Pawn)) ConditionalWeakTableRemove(this.Pawn);
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
            Lord symbioteLord = ((Pawn)thing).GetLord();
            color = thing.def == ExSymbiotesDefOf.ExSymbiotes_Symbiote ? blue : red;
            
            thing.DeSpawn(DestroyMode.Vanish);
            this.innerContainer.TryAdd((Thing) thing, true);
            ConditionalWeakTableRemove(this.Pawn);
            ConditionalWeakTableAdd(this.Pawn);
            
            if(symbioteLord==null) 
                this.parent.pawn.health.RemoveHediff(parent);
            else 
                symbioteLord.AddPawn(this.Pawn);
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
            Scribe_References.Look(ref originalFaction, "originalFaction");
            Scribe_Deep.Look<ThingOwner<Thing>>(ref this.innerContainer, "innerContainer", (object) this);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.Pawn != null) ConditionalWeakTableAdd(this.Pawn);
            if (Scribe.mode != LoadSaveMode.PostLoadInit || !this.innerContainer.removeContentsIfDestroyed)
                return;
            this.innerContainer.removeContentsIfDestroyed = false;
        }
    }
}