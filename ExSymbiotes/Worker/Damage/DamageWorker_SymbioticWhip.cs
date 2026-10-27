using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ExSymbiotes
{
    public class DamageWorker_SymbioticWhip : DamageWorker
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
            if (victim is Pawn pawn)
            {
                pawn.stances.stunner.StunFor((int)dinfo.Def.constantStunDurationTicks, dinfo.Instigator, showMote:false);
                IntVec3 pos = pawn.Position;
                SoundDefOf.MeleeHit_Unarmed.PlayOneShot((SoundInfo) (Thing) pawn);
                PawnFlyer flyer = PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer_Stun, pawn, dinfo.Instigator.Position, null,SoundDefOf.MeleeHit_Unarmed, overrideStartVec: (pos.ToVector3() + new Vector3(0.0f, 0.0f, -1f)));
                GenSpawn.Spawn(flyer, pos, dinfo.Instigator.Map);
                float distance = dinfo.Instigator.Position.DistanceTo(pawn.Position);
                float flightSpeed = flyer.def.pawnFlyer.flightSpeed;
                float flightDurationMin = flyer.def.pawnFlyer.flightDurationMin;
                float totalTicks = Mathf.CeilToInt(Mathf.Max(flightDurationMin * 60f, distance / flightSpeed * 60f))+(float)dinfo.Def.constantStunDurationTicks;
                MoteMaker.MakeAttachedOverlay(pawn, ExSymbiotesDefOf.ExSymbiotes_Mote_HarbingerTreeRoots, Vector3.zero, pawn.BodySize, totalTicks/60);
            }
            damageResult.stunned = true;
            return damageResult;
        }
    }
}