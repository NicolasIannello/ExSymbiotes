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
                MoteMaker.MakeAttachedOverlay(pawn, ExSymbiotesDefOf.ExSymbiotes_Mote_HarbingerTreeRoots, new Vector3(), pawn.BodySize, (float)dinfo.Def.constantStunDurationTicks/60);
                IntVec3 pos = pawn.Position;
                SoundDefOf.MeleeHit_Unarmed.PlayOneShot((SoundInfo) (Thing) pawn);
                PawnFlyer flyer = PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer_Stun, pawn, dinfo.Instigator.Position, null,SoundDefOf.MeleeHit_Unarmed, overrideStartVec: (pos.ToVector3() + new Vector3(0.0f, 0.0f, -1f)));
                GenSpawn.Spawn(flyer, pos, dinfo.Instigator.Map);
            }
            damageResult.stunned = true;
            return damageResult;
        }
    }
}