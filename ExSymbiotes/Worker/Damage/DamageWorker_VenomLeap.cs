using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class DamageWorker_VenomLeap : DamageWorker_Blunt
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
            if (victim is Pawn pawn && !pawn.Dead)
            {
                pawn.stances.stunner.StunFor((int)dinfo.Def.constantStunDurationTicks, dinfo.Instigator, showMote:false);
                MoteMaker.MakeAttachedOverlay(pawn, ExSymbiotesDefOf.ExSymbiotes_Mote_HarbingerTreeRootsBlack, new Vector3(), pawn.BodySize, (float)dinfo.Def.constantStunDurationTicks/60);
            }
            damageResult.stunned = true;
            return damageResult;
        }
    }
}