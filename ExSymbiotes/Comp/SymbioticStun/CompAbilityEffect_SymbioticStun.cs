using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbioticStun : CompAbilityEffect
    {
        private CompProperties_AbilitySymbioticStun Props2 => (CompProperties_AbilitySymbioticStun) this.props;

        private Pawn Pawn => this.parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            EffecterDefOf.ChimeraRage.SpawnAttached(Pawn, Pawn.Map).Cleanup();
            ExSymbiotesDefOf.ExSymbiotes_VoidStructureActivated.SpawnMaintained(Pawn, Pawn);
            GenExplosion.DoExplosion(Pawn.Position, Pawn.Map, this.Props2.radius, DamageDefOf.Stun, Pawn, doVisualEffects:false, doSoundEffects:false, screenShakeFactor:0f, excludeRadius:1f);
            base.Apply(target, dest);
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return this.Pawn.Faction != Faction.OfPlayer && target.HasThing && target.Thing is Pawn thing && thing.TargetCurrentlyAimingAt == (LocalTargetInfo) (Thing) this.Pawn;
        }
    }
}