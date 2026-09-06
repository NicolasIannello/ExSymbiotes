using ExSymbiotes.Utils;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ExSymbiotes
{
    public class JobGiver_FindHost : JobGiver_AIFightEnemy
    {
        private AbilityDef ability;
        private bool skipIfCantTargetNow = true;
        protected override bool OnlyUseAbilityVerbs => true;
        protected override bool OnlyUseRangedSearch => true;
        
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_FindHost giverAiAbilityFight = (JobGiver_FindHost) base.DeepCopy(resolve);
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
            return target is Pawn;
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
        
        protected override Thing FindAttackTarget(Pawn pawn)
        {
            Lord lord = LordUtility.GetLord(pawn);
            pawn.GetLord();           
            Verb verb = pawn.CurrentEffectiveVerb;

            Thing closestPawn = GenClosest.ClosestThing_Global(pawn.Position, pawn.Map.mapPawns.AllPawns, 9999f, thing =>
            {
                Pawn target = thing as Pawn;
                if (target == null) return false;

                if (target.Faction == pawn.Faction) return false;
                
                if (lord != null && !lord.LordJob.ValidateAttackTarget(pawn, target)) return false;
                
                if (target.IsBurning()) return false;

                bool reachable = SymbioteUtility.TargetFinder.CanReach(pawn, target, canBashDoors: false, canBashFences: false);
                bool canShoot = SymbioteUtility.TargetFinder.CanShootAtFromCurrentPosition(target, pawn, verb);

                return reachable || canShoot;
            });

            return closestPawn;
        }
        
    }
}