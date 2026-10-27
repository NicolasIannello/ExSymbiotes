using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompProperties_GrendelLeap : CompProperties_AbilityEffect
    {
        public float radius;
        public ThingDef filth;
        
        public CompProperties_GrendelLeap() => this.compClass = typeof (CompAbilityEffect_GrendelLeap);
    }
}