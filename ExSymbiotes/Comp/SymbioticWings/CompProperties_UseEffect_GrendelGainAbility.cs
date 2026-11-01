using RimWorld;

namespace ExSymbiotes
{

    public class CompProperties_UseEffect_GrendelGainAbility : CompProperties_UseEffect
    {
        public AbilityDef ability;

        public CompProperties_UseEffect_GrendelGainAbility()
        {
            this.compClass = typeof(CompUseEffect_GrendelGainAbility);
        }
    }
}