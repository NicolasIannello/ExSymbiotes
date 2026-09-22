using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_VenomLeap : CompAbilityEffect, ICompAbilityEffectOnJumpCompleted
    {
        private CompProperties_VenomLeap Props2 => (CompProperties_VenomLeap) this.props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (this.parent.pawn.health.hediffSet.HasHediff(ExSymbiotesDefOf.ExSymbiotes_Symbiosis))
            {
                this.parent.pawn.health.GetOrAddHediff(ExSymbiotesDefOf.ExSymbiotes_Symbiosis).TryGetComp<HediffComp_VenomLeap>().SetStage(true);
                SymbioteUtility.MeatSplatter(0, this.parent.pawn.Position, this.parent.pawn.Map, FleshbeastUtility.MeatExplosionSize.Small);
            }
        }
        
        public void OnJumpCompleted(IntVec3 origin, LocalTargetInfo target)
        {
            if(this.parent.pawn.health.hediffSet.HasHediff(ExSymbiotesDefOf.ExSymbiotes_Symbiosis))
                this.parent.pawn.health.GetOrAddHediff(ExSymbiotesDefOf.ExSymbiotes_Symbiosis).TryGetComp<HediffComp_VenomLeap>().SetStage(false);
            GenExplosion.DoExplosion(this.parent.pawn.Position, this.parent.pawn.Map, this.Props2.radius, ExSymbiotesDefOf.ExSymbiotes_VenomLeap, this.parent.pawn, doVisualEffects:false, doSoundEffects:false, screenShakeFactor:0f, excludeRadius:1f);
            SymbioteUtility.MeatSplatter(0, this.parent.pawn.Position, this.parent.pawn.Map, FleshbeastUtility.MeatExplosionSize.Small);
            FilthMaker.TryMakeFilth(this.parent.pawn.Position, this.parent.pawn.Map, this.Props2.filth);
        }
    }
}