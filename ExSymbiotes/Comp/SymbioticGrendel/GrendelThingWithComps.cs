using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class GrendelThingWithComps : ThingWithComps
    {
        public Thing target;
        public bool flying = false;
        public override Vector3 DrawPos => this.target == null ? base.DrawPos : this.target.ParentHolder is PawnFlyer flyer ? flyer.DrawPos : this.target.DrawPos;
     
        public void SetTarget(Thing target) => this.target = target;
        
        protected override void DrawAt(Vector3 drawLoc, bool flip = false) => this.Comps_PostDraw();

        protected override void Tick()
        {
            base.Tick();
            if (this.target == null)
                return;
            this.Position = this.target.Position;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Thing>(ref this.target, "target");
            Scribe_Values.Look<bool>(ref flying, "flying");
        }
    }
}