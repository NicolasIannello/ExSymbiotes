using System.Collections.Generic;
using Verse.AI;

namespace ExSymbiotes
{
    public class JobDriver_SymbioteCastVerb : JobDriver_CastVerbOnce
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull<JobDriver_CastVerbOnce>(TargetIndex.A);

            Toil gotoCastPosition = Toils_Combat.GotoCastPosition(TargetIndex.A, TargetIndex.B);
            Toil checkTargetHittable = Toils_Jump.JumpIfTargetNotHittable(TargetIndex.A, gotoCastPosition);

            yield return gotoCastPosition;
            yield return checkTargetHittable;
            yield return Toils_Combat.CastVerb(TargetIndex.A);
            yield return Toils_Jump.Jump(checkTargetHittable);
        }
    }
}