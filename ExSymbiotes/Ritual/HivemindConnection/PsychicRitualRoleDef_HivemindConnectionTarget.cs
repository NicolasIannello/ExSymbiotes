using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class PsychicRitualRoleDef_HivemindConnectionTarget : PsychicRitualRoleDef
    {
        [MustTranslate]
        public string psychicRitualLeaveReason_ExSymbiotesConnected;
        [MustTranslate]
        public string psychicRitualLeaveReason_ExSymbiotesSymbiosis;
        
        protected override bool PawnCanDo(
            PsychicRitualRoleDef.Context context,
            Pawn pawn,
            TargetInfo target,
            out AnyEnum reason)
        {
            if (!base.PawnCanDo(context, pawn, target, out reason) || pawn == null)
                return false;
        
            if (SymbioteUtility.HasSymbiosis(pawn, false) == null)
            {
                reason = AnyEnum.FromEnum<HivemindConnectionTargetReason>(HivemindConnectionTargetReason.NonSymbiosis);
                return false;
            }
        
            Hediff spiral = pawn.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_HivemindConnection);
            if (spiral == null)
                return true;
            reason = AnyEnum.FromEnum<HivemindConnectionTargetReason>(HivemindConnectionTargetReason.Connected);
            return false;
        }

        public override TaggedString PawnCannotDoReason(
            AnyEnum reason,
            PsychicRitualRoleDef.Context context,
            Pawn pawn,
            TargetInfo target)
        {
            HivemindConnectionTargetReason? nullable = reason.As<HivemindConnectionTargetReason>();
            if (!nullable.HasValue)
                return base.PawnCannotDoReason(reason, context, pawn, target);
            int valueOrDefault = (int) nullable.GetValueOrDefault();
            TaggedString result = TaggedString.Empty;
            switch (valueOrDefault)
            {
                case 1: 
                    result = this.psychicRitualLeaveReason_ExSymbiotesSymbiosis.Formatted(pawn.Named("PAWN"));
                    break;
                case 2 :
                    result = this.psychicRitualLeaveReason_ExSymbiotesConnected.Formatted(pawn.Named("PAWN"));
                    break;
            }
            return result;
        }

        public enum HivemindConnectionTargetReason
        {
            None,
            NonSymbiosis,
            Connected,
        }
    }
}