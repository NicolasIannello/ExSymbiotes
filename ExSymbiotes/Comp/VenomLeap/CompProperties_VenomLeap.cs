using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompProperties_VenomLeap : CompProperties_AbilityEffect
    {
        public float radius;
        public ThingDef filth;
        
        public CompProperties_VenomLeap() => this.compClass = typeof (CompAbilityEffect_VenomLeap);
    }
}