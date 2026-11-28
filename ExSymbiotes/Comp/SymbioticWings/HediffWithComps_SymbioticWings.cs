using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class HediffWithComps_SymbioticWings : HediffWithComps_GrendelRegeneration
    {
        protected override float hpRegen => 300f;
        public override bool ShouldRemove
        {
            get
            {
                if (this.comps != null)
                {
                    for (int index = 0; index < this.comps.Count; ++index)
                    {
                        if (this.comps[index].CompShouldRemove)
                            return true;
                    }
                }
                return (double) this.Severity <= 0.0;
            }
        }
        
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            pawn.GetStatValue(StatDefOf.MaxFlightTime); //this refreshes Pawn_FlightTracker.MaxFlightTicks
            this.pawn.flight.StartFlying();
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            pawn.GetStatValue(StatDefOf.MaxFlightTime);
            if(!this.pawn.Dead) this.pawn.flight.ForceLand();
        }

        public override void PostTick()
        {
            base.PostTick();
            if (hpRemaining <= 0f && Severity > 0.5f) this.Severity = 0.5f;
        }
    }
}