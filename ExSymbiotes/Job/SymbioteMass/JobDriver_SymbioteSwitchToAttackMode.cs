using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class JobDriver_SymbioteSwitchToAttackMode : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil toil = ToilMaker.MakeToil(nameof(MakeNewToils));
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            toil.initAction = (Action)(() =>
            {
                Lord lord = this.pawn.GetLord();
                if (!(lord?.LordJob is LordJob_SymbioteMass))
                    return;
                lord.ReceiveMemo("StalkToAttack");
            });
            yield return toil;
        }
    }
}