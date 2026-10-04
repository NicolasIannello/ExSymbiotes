using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class PsychicRitualRoleDef_HivemindConnectionInvoker : PsychicRitualRoleDef
    {
        [MustTranslate]
        public string psychicRitualLeaveReason_ExSymbiotesVoid;
        
        protected override bool PawnCanDo(
            PsychicRitualRoleDef.Context context,
            Pawn pawn,
            TargetInfo target,
            out AnyEnum reason)
        {
            if (!base.PawnCanDo(context, pawn, target, out reason))
                return false;
            Hediff spiral = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.VoidTouched);
            if (spiral != null)
                return true;
            reason = AnyEnum.FromEnum<HivemindConnectionInvokerReason>(HivemindConnectionInvokerReason.NonVoid);
            return false;
        }

        public override TaggedString PawnCannotDoReason(
            AnyEnum reason,
            PsychicRitualRoleDef.Context context,
            Pawn pawn,
            TargetInfo target)
        {
            HivemindConnectionInvokerReason? nullable = reason.As<HivemindConnectionInvokerReason>();
            if (!nullable.HasValue)
                return base.PawnCannotDoReason(reason, context, pawn, target);
            int valueOrDefault = (int) nullable.GetValueOrDefault();
            return this.psychicRitualLeaveReason_ExSymbiotesVoid.Formatted(pawn.Named("PAWN"));
        }

        public enum HivemindConnectionInvokerReason
        {
            None,
            NonVoid,
        }
    }
}