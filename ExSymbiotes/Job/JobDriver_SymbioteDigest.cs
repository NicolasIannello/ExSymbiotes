using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace ExSymbiotes
{
    public class JobDriver_SymbioteDigest : JobDriver
    {
        private CompSymbiote comp;

        private CompSymbiote Comp => this.comp ?? (this.comp = this.pawn.GetComp<CompSymbiote>());

        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        public override string GetReport() => (string)null;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            int digestionTicks = this.Comp.GetDigestionTicks();
            Toil toil = Toils_General.Wait(digestionTicks).WithProgressBarToilDelay(TargetIndex.None, digestionTicks);
            toil.FailOn<Toil>((Func<bool>)(() => !this.Comp.Digesting));
            toil.PlaySustainerOrSound(SoundDefOf.Pawn_Devourer_Digesting);
            toil.AddFinishAction(new Action(this.Comp.DigestJobFinished));
            yield return toil;
        }
    }
}