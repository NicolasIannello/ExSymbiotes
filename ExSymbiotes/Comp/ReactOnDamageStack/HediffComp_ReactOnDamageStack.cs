using System;
using ExSymbiotes.Utils;
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
            bool notEMPorGrendel = DamageDefOf.EMP != dinfo.Def && ExSymbiotesDefOf.ExSymbiotes_GrendelStun != dinfo.Def;
            bool notFire = DamageDefOf.Flame != dinfo.Def && DamageDefOf.Burn != dinfo.Def;
            if ((notEMPorGrendel && notFire) || SymbioteUtility.IsSpecial(this.parent.pawn))
                return;
            this.React(notFire ? 0.2f : 0.05f);
        }

        private void React(float amount)
        {
            if (this.Props.createHediff != null)
            {
                BodyPartRecord part = this.parent.Part;
                if (this.Props.createHediffOn != null)
                    part = this.parent.pawn.RaceProps.body.AllParts.FirstOrFallback<BodyPartRecord>(
                        (Func<BodyPartRecord, bool>)(p => p.def == this.Props.createHediffOn));
                Hediff hediff = this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.createHediff);
                if (hediff != null)
                    hediff.Severity += amount;
                else
                    this.parent.pawn.health.AddHediff(this.Props.createHediff, part).Severity += amount;
            }

            if (!this.Props.vomit)
                return;
            this.parent.pawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.Vomit), JobCondition.InterruptForced,
                resumeCurJobAfterwards: true);
        }
    }
}