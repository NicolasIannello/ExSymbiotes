using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class HediffWithComps_Symbiosis : HediffWithComps
    {
        public override void Notify_IngestedThing(Thing thing, int amount)
        {
            base.Notify_IngestedThing(thing, amount);
            if(thing.def == ThingDefOf.Chocolate) this.TryGetComp<HediffComp_Symbiosis>().AddSymbiosis(1);
        }
        
        public override void Notify_Downed()
        {
            base.Notify_Downed();
            this.TryGetComp<HediffComp_Symbiosis>().ChangeVisual(false);
        }
    }
}