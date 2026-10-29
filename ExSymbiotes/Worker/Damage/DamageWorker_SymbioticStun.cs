using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class DamageWorker_SymbioticStun : DamageWorker_Blunt
    {
        protected virtual ThingDef moteDef => ExSymbiotesDefOf.ExSymbiotes_Mote_HarbingerTreeRoots;
        
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
            if (victim is Pawn pawn && !pawn.Dead)
            {
                if (victim == dinfo.Instigator) return new DamageResult();

                float scale = pawn.BodySize>3 ? pawn.BodySize/2 : pawn.BodySize;
                pawn.stances.stunner.StunFor((int)dinfo.Def.constantStunDurationTicks, dinfo.Instigator, showMote:false);
                MoteMaker.MakeAttachedOverlay(pawn, moteDef, Vector3.zero, scale, (float)dinfo.Def.constantStunDurationTicks/60);
            }
            damageResult.stunned = true;
            return damageResult;
        }
    }
    
    public class DamageWorker_SymbioticStunBlack : DamageWorker_SymbioticStun
    {
        protected override ThingDef moteDef => ExSymbiotesDefOf.ExSymbiotes_Mote_HarbingerTreeRootsBlack;
    }
    
    public class DamageWorker_grendelStun : DamageWorker_SymbioticStunBlack
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
            if (victim is Pawn pawn && !pawn.Dead && victim == dinfo.Instigator)
                ((Pawn)victim).stances.stunner.StunFor(60, dinfo.Instigator, showMote:false);
            return damageResult;
        }
    }
}