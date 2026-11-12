using RimWorld;

namespace ExSymbiotes
{
    public class CompProperties_AbilitySymbioticRenewal : CompProperties_AbilityEffect
    {
        public CompProperties_AbilitySymbioticRenewal()
        {
            this.compClass = typeof (CompAbilityEffect_SymbioticRenewal);
        }
    }
}