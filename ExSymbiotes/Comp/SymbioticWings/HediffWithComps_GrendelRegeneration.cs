using UnityEngine;
using Verse;

namespace ExSymbiotes;

public class HediffWithComps_GrendelRegeneration : HediffWithComps
{
    protected float hpRemaining = 100f;
    protected float hpCapacity = 100f;
    public override string SeverityLabel
    {
        get => $"{this.hpRemaining:0} / {this.hpCapacity:0}{"HP".Translate()}";
    }
    protected virtual float hpRegen => 1000f;

    public override bool ShouldRemove => (double) this.hpRemaining <= 0.0;

    public void SetHpCapacity(float amount) => this.hpRemaining = this.hpCapacity = amount;

    public override void PostMake()
    {
        base.PostMake();
        this.SetHpCapacity(hpRegen);
    }

    public override void PostRemoved()
    {
        base.PostRemoved();
        CompGrendelSymbiote grendelComp = this.pawn.TryGetComp<CompGrendelSymbiote>();
        if (grendelComp != null)
            grendelComp.ExplodeCocoons();
    }

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