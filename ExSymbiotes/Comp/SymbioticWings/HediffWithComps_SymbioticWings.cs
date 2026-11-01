using Verse;

namespace ExSymbiotes
{
    public class HediffWithComps_SymbioticWings : HediffWithComps_GrendelRegeneration
    {
        protected override float hpRegen => 300f;
        public override bool ShouldRemove => false;
        
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            this.pawn.flight.StartFlying();
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            if(!this.pawn.Dead) this.pawn.flight.ForceLand();
        }

        public override void PostTick()
        {
            base.PostTick();
            Log.Message($"hpRemaining {hpRemaining} Severity {Severity}");
            if (hpRemaining <= 0f && Severity > 0.5f) this.Severity = 0.5f;
        }
    }
}