using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class HediffWithComps_Symbiosis : HediffWithComps_SymbioteStats
    {
        public override void Notify_IngestedThing(Thing thing, int amount)
        {
            base.Notify_IngestedThing(thing, amount);
            if(thing.def == ThingDefOf.Chocolate) this.TryGetComp<HediffComp_Symbiosis>().AddSymbiosis(4);
        }
        
        public override void Notify_Downed()
        {
            base.Notify_Downed();
            this.TryGetComp<HediffComp_Symbiosis>().ChangeVisual(false);
        }
    }
}