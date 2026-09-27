using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    [StaticConstructorOnStartup]
    public class CompSymbioticInterceptor : CompProjectileInterceptor
    {
        public int startTick = -999999;
        public int RemainingTicks
        {
            get => this.startTick + this.Props.activeDuration - Find.TickManager.TicksGame;
        }
        
        public override void PostPostMake()
        {
            base.PostPostMake();
            startTick = Find.TickManager.TicksGame;
        }
        
        public override void CompTick()
        {
            if(this.currentHitPoints==0 || RemainingTicks<=0) this.Break();
            base.CompTick();
        }
        
        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(ref dinfo, out absorbed);
            if (dinfo.Def != DamageDefOf.EMP || this.Props.disarmedByEmpForTicks <= 0)
                return;
            this.Break();
        }

        public void Break()
        {
            EffecterDefOf.Shield_Break.SpawnAttached((Thing) this.parent, this.parent.MapHeld, this.Props.radius);
            int num = Mathf.CeilToInt(this.Props.radius * 2f);
            float fTheta = 6.2831855f / (float) num;
            Vector3 center = this.parent.TrueCenter();
            for (int index = 0; index < num; ++index)
                FleckMaker.ConnectingLine(PosAtIndex(index), PosAtIndex((index + 1) % num), FleckDefOf.LineEMP, this.parent.Map, 1.5f);
            Vector3 PosAtIndex(int index)
            {
                return new Vector3(this.Props.radius * Mathf.Cos(fTheta * (float) index) + center.x, 0.0f, this.Props.radius * Mathf.Sin(fTheta * (float) index) + center.z);
            }
            this.parent.Destroy();
        }
    }
}