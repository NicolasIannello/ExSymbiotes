using RimWorld;

namespace ExSymbiotes
{
    public class CompProperties_AbilitySymbioticStun : CompProperties_AbilityEffect
    {
        public float radius = 6f;

        public CompProperties_AbilitySymbioticStun() => this.compClass = typeof (CompAbilityEffect_SymbioticStun);
    }
}