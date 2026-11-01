using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class HediffWithComps_SymbioticWings : HediffWithComps
    {
        private float hpRemaining = 100f;
        private float hpCapacity = 100f;
        public override string SeverityLabel
        {
            get => $"{this.hpRemaining:0} / {this.hpCapacity:0}{"HP".Translate()}";
        }
        
        public override void PostMake()
        {
            base.PostMake();
            this.SetHpCapacity(300f);
        }
        
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            this.pawn.flight.StartFlying();
        }

        public override void PreRemoved()
        {
            base.PreRemoved();
            if(!this.pawn.Dead) this.pawn.flight.ForceLand();
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            if(!this.pawn.Dead) this.pawn.flight.ForceLand();
        }

        public void SetHpCapacity(float amount) => this.hpRemaining = this.hpCapacity = amount;

        public override void Notify_Regenerated(float hp)
        {
            this.hpRemaining = Mathf.Max(this.hpRemaining - hp, 0.0f);
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.hpRemaining, "hpRemaining");
            Scribe_Values.Look<float>(ref this.hpCapacity, "hpCapacity");
        }
    }
}