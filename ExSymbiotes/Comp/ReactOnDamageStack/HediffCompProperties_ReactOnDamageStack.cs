using Verse;

namespace ExSymbiotes
{
    public class HediffCompProperties_ReactOnDamageStack : HediffCompProperties_ReactOnDamage
    {
        public HediffCompProperties_ReactOnDamageStack() => this.compClass = typeof(HediffComp_ReactOnDamageStack);
    }
}