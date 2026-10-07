using RimWorld;

namespace ExSymbiotes
{
    public class CompProperties_AbilitySymbioticConsume : CompProperties_AbilityEffect
    {
        public CompProperties_AbilitySymbioticConsume()
        {
            this.compClass = typeof (CompAbilityEffect_SymbioticConsume);
        }
    }
}