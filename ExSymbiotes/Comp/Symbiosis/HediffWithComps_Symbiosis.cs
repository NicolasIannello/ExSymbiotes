using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class HediffWithComps_Symbiosis : HediffWithComps
    {
        public override void Notify_IngestedThing(Thing thing, int amount)
        {
            base.Notify_IngestedThing(thing, amount);
            if(thing.def == ThingDefOf.Chocolate) this.TryGetComp<HediffComp_Symbiosis>().AddSymbiosis(4);
        }
    }
}