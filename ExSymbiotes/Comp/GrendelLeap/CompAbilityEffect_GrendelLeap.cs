using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_GrendelLeap : CompAbilityEffect, ICompAbilityEffectOnJumpCompleted
    {
        private CompProperties_GrendelLeap Props2 => (CompProperties_GrendelLeap) this.props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            CompGrendelSymbiote compGrendel = this.parent.pawn.TryGetComp<CompGrendelSymbiote>();
            if (compGrendel != null) compGrendel.flying = true;
            HediffWithComps_SymbioticGrendel hediffGrendel = this.parent.pawn.health.hediffSet.GetFirstHediff<HediffWithComps_SymbioticGrendel>();
            if (hediffGrendel != null) hediffGrendel.flying = true;
            this.parent.pawn.Drawer.renderer.SetAllGraphicsDirty();
            SymbioteUtility.MeatSplatter(0, this.parent.pawn.Position, this.parent.pawn.Map, FleshbeastUtility.MeatExplosionSize.Small);
        }
        
        public void OnJumpCompleted(IntVec3 origin, LocalTargetInfo target)
        {
            CompGrendelSymbiote compGrendel = this.parent.pawn.TryGetComp<CompGrendelSymbiote>();
            if (compGrendel != null) compGrendel.flying = false;
            HediffWithComps_SymbioticGrendel hediffGrendel = this.parent.pawn.health.hediffSet.GetFirstHediff<HediffWithComps_SymbioticGrendel>();
            if (hediffGrendel != null) hediffGrendel.flying = false;
            this.parent.pawn.Drawer.renderer.SetAllGraphicsDirty();
            GenExplosion.DoExplosion(this.parent.pawn.Position, this.parent.pawn.Map, this.Props2.radius, ExSymbiotesDefOf.ExSymbiotes_VenomLeap, this.parent.pawn, doVisualEffects:false, doSoundEffects:false, screenShakeFactor:0f);
            SymbioteUtility.MeatSplatter(0, this.parent.pawn.Position, this.parent.pawn.Map, FleshbeastUtility.MeatExplosionSize.Small);
            FilthMaker.TryMakeFilth(this.parent.pawn.Position, this.parent.pawn.Map, this.Props2.filth);
            this.parent.pawn.stances.stunner.StunFor(30, this.parent.pawn, showMote:false);
        }
        
        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return target.Pawn.Position.DistanceTo(this.parent.pawn.Position) > 15;
        }
    }
}