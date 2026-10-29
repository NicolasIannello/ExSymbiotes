using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompAbilityEffect_SymbioticStun : CompAbilityEffect
    {
        protected CompProperties_AbilitySymbioticStun Props2 => (CompProperties_AbilitySymbioticStun) this.props;
        protected virtual DamageDef damageDef => ExSymbiotesDefOf.ExSymbiotes_SymbioticStun;
        protected virtual EffecterDef effecter => ExSymbiotesDefOf.ExSymbiotes_VoidStructureActivated;
        protected Pawn Pawn => this.parent.pawn;
        protected virtual float radius => this.Props2.radius;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            EffecterDefOf.ChimeraRage.SpawnAttached(Pawn, Pawn.Map).Cleanup();
            effecter.SpawnMaintained(Pawn, Pawn);
            GenExplosion.DoExplosion(Pawn.Position, Pawn.Map, radius, damageDef, Pawn, doVisualEffects:false, doSoundEffects:false, screenShakeFactor:0f);
            base.Apply(target, dest);
        }
    }
    
    public class CompAbilityEffect_SymbioticStunBlack : CompAbilityEffect_SymbioticStun
    { 
        protected CompProperties_AbilityGrendelStun Props3 => (CompProperties_AbilityGrendelStun) this.props;
        protected override DamageDef damageDef => ExSymbiotesDefOf.ExSymbiotes_GrendelStun;
        protected override EffecterDef effecter => ExSymbiotesDefOf.ExSymbiotes_VoidStructureActivatedBlack;
        protected override float radius => this.Props3.radius;
        private bool flag = true;
        private bool flagCast = false;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            flagCast = false;
        }
        
        public override void CompTick()
        {
            base.CompTick();
            if (flagCast && !parent.Casting) parent.verb.TryStartCastOn(Pawn, nonInterruptingSelfCast:true);
            
            if (!flag && !parent.Casting)
            {
                flag = true;
                return;
            }

            if (flag && parent.Casting && Pawn.Spawned)
            {
                flagCast = true;
                flag = false;
                Props3.warmupEffecter.SpawnAttached(Pawn, Pawn.Map).Cleanup();
            }
        }
        
        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            int count = 0;
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(Pawn.Position, this.Props3.radius-0.5f, true))
            {
                foreach (Thing t in cell.GetThingList(Pawn.MapHeld)) if (t is Pawn) count++;
                if (count > 2) return true;
            }
            return false;
        }
    }
}