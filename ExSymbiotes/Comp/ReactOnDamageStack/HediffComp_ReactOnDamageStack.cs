using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
    public class HediffComp_ReactOnDamageStack : HediffComp
    {
        public HediffCompProperties_ReactOnDamageStack Props
        {
            get => (HediffCompProperties_ReactOnDamageStack)this.props;
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (this.Props.damageDefIncoming != dinfo.Def)
                return;
            this.React();
        }

        private void React()
        {
            if (this.Props.createHediff != null)
            {
                BodyPartRecord part = this.parent.Part;
                if (this.Props.createHediffOn != null)
                    part = this.parent.pawn.RaceProps.body.AllParts.FirstOrFallback<BodyPartRecord>(
                        (Func<BodyPartRecord, bool>)(p => p.def == this.Props.createHediffOn));
                Hediff hediff = this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.createHediff);
                if (hediff != null)
                    hediff.Severity += 0.2f;
                else
                    this.parent.pawn.health.AddHediff(this.Props.createHediff, part);
            }

            if (!this.Props.vomit)
                return;
            this.parent.pawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.Vomit), JobCondition.InterruptForced,
                resumeCurJobAfterwards: true);
        }
    }
}