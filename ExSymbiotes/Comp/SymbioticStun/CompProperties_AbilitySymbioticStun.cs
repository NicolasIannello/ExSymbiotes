using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompProperties_AbilitySymbioticStun : CompProperties_AbilityEffect
    {
        public float radius = 6f;

        public CompProperties_AbilitySymbioticStun() => this.compClass = typeof (CompAbilityEffect_SymbioticStun);
    }
    
    public class CompProperties_AbilityGrendelStun : CompProperties_AbilityEffect
    {
        public float radius = 6f;
        public EffecterDef warmupEffecter;

        public CompProperties_AbilityGrendelStun() => this.compClass = typeof (CompAbilityEffect_SymbioticStunBlack);
    }
}