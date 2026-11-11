using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{

    public class JobDriver_InspectStrangeAsteroid : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetA, this.job, errorOnFailed: errorOnFailed);
        }

        protected override 
        #nullable disable
        IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull<JobDriver_InspectStrangeAsteroid>(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.WaitWith(TargetIndex.A, 120, true, face: TargetIndex.A);
            Toil toil = ToilMaker.MakeToil(nameof (MakeNewToils));
            toil.initAction = (Action) (() =>
            {
                if (!(this.TargetA.Thing is Building_StrangeAsteroid thing2))
                    return;
                thing2.Inspect(pawn);
            });
            yield return toil;
        }
    }
}