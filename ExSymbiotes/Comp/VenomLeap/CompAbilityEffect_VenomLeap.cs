using ExSymbiotes.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_VenomLeap : CompAbilityEffect, ICompAbilityEffectOnJumpCompleted
    {
        private int tickChange = -99999;
        private CompProperties_VenomLeap Props2 => (CompProperties_VenomLeap) this.props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            float distance = this.parent.pawn.Position.DistanceTo(target.Cell);
            float flightSpeed = 12f;        //ExSymbiotes_PawnFlyer_VenomLeap flightSpeed
            float flightDurationMin = 0.5f; //ExSymbiotes_PawnFlyer_VenomLeap flightDurationMin
            float totalTicks = Mathf.CeilToInt(Mathf.Max(flightDurationMin * 60f, distance / flightSpeed * 60f));
            int tick70 = Mathf.CeilToInt(totalTicks * 0.55f);
            this.tickChange = Find.TickManager.TicksGame + tick70;
            this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis).TryGetComp<HediffComp_VenomLeap>().SetStage(true);
            this.parent.pawn.Drawer.renderer.SetAllGraphicsDirty();
            SymbioteUtility.MeatSplatter(0, this.parent.pawn.Position, this.parent.pawn.Map, FleshbeastUtility.MeatExplosionSize.Small);
        }
        
        public void OnJumpCompleted(IntVec3 origin, LocalTargetInfo target)
        {
            this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis).TryGetComp<HediffComp_VenomLeap>().SetStage(false);
            this.parent.pawn.Drawer.renderer.SetAllGraphicsDirty();
            GenExplosion.DoExplosion(this.parent.pawn.Position, this.parent.pawn.Map, this.Props2.radius, ExSymbiotesDefOf.ExSymbiotes_VenomLeap, this.parent.pawn, doVisualEffects:false, doSoundEffects:false, screenShakeFactor:0f, excludeRadius:1f);
            SymbioteUtility.MeatSplatter(0, this.parent.pawn.Position, this.parent.pawn.Map, FleshbeastUtility.MeatExplosionSize.Small);
            FilthMaker.TryMakeFilth(this.parent.pawn.Position, this.parent.pawn.Map, this.Props2.filth);
        }
        
        public override void CompTick()
        {
            base.CompTick();
            if (tickChange>0 && Find.TickManager.TicksGame >= this.tickChange)
            {
                tickChange = -99999;
                this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis).TryGetComp<HediffComp_VenomLeap>().SetLanding(true);
                this.parent.pawn.Drawer.renderer.SetAllGraphicsDirty();
            }
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.tickChange, "tickChange");
        }
    }
}