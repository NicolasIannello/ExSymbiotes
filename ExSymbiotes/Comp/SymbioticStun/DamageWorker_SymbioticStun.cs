using Verse;

namespace ExSymbiotes
{
    public class DamageWorker_SymbioticStun : DamageWorker
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
            if (victim is Pawn pawn)
            {
                pawn.stances.stunner.StunFor((int)dinfo.Def.constantStunDurationTicks, dinfo.Instigator);
            }
            damageResult.stunned = true;
            return damageResult;
        }
    }
}