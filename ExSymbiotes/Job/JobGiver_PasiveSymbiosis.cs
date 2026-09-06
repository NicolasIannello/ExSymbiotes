using RimWorld;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
    public class JobGiver_PasiveSymbiosis : JobGiver_AIFightEnemy
    {
        private AbilityDef ability;
        private bool skipIfCantTargetNow = true;
        protected override bool OnlyUseAbilityVerbs => true;
        protected override bool OnlyUseRangedSearch => true;
        
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_PasiveSymbiosis giverAiAbilityFight = (JobGiver_PasiveSymbiosis) base.DeepCopy(resolve);
            giverAiAbilityFight.ability = this.ability;
            giverAiAbilityFight.skipIfCantTargetNow = this.skipIfCantTargetNow;
            return (ThinkNode) giverAiAbilityFight;
        }

        protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest, Verb verbToUse = null)
        {
            dest = IntVec3.Invalid;
            Thing enemyTarget = pawn.mindState.enemyTarget;
            Ability ability = pawn.abilities.GetAbility(this.ability);
            return CastPositionFinder.TryFindCastPosition(new CastPositionRequest()
            {
                caster = pawn,
                target = enemyTarget,
                verb = ability.verb,
                maxRangeFromTarget = ability.verb.EffectiveRange,
                wantCoverFromTarget = false,
                preferredCastPosition = new IntVec3?(pawn.Position)
            }, out dest);
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            return pawn.abilities.GetAbility(this.ability).OnCooldown && this.skipIfCantTargetNow ? (Job) null : base.TryGiveJob(pawn);
        }

        protected override bool ShouldLoseTarget(Pawn pawn)
        {
            return base.ShouldLoseTarget(pawn) || !this.CanTarget(pawn, pawn.mindState.enemyTarget);
        }

        protected override bool ExtraTargetValidator(Pawn pawn, Thing target)
        {
            return base.ExtraTargetValidator(pawn, target) && this.CanTarget(pawn, target);
        }

        private bool CanTarget(Pawn pawn, Thing target)
        {
            if (!this.ability.verbProperties.targetParams.CanTarget((TargetInfo) target))
                return false;
            Ability ability = pawn.abilities.GetAbility(this.ability);
            if (!ability.CanApplyOn((LocalTargetInfo) target))
                return false;
            return !this.skipIfCantTargetNow || ability.AICanTargetNow((LocalTargetInfo) target);
        }
    }
}