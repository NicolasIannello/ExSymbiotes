using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class Verb_CastAbilityGrendelLeap : Verb_CastAbilityJump
    {
        public override ThingDef JumpFlyerDef => ExSymbiotesDefOf.ExSymbiotes_PawnFlyer_GrendelLeap;
        private float radius = 2.5f;
        
        public override void DrawHighlight(LocalTargetInfo target)
        {
            base.DrawHighlight(target);
            if (target.IsValid && JumpUtility.ValidJumpTarget((Thing)this.CasterPawn, this.caster.Map, target.Cell))
            {
                GenDraw.DrawRadiusRing(target.Cell, radius, Color.white);
            }
        }
    }
}