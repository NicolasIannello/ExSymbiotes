using System.Collections.Generic;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace ExSymbiotes
{
    
    [HarmonyPatch(typeof(PawnRenderNodeWorker), "GetMaterial")]
    public static class Patch_PawnRenderNodeWorker_GetMaterial
    {
        private static readonly Dictionary<Material, Material> materials = new Dictionary<Material, Material>();

        public static void Postfix(PawnRenderNode node, PawnDrawParms parms, ref Material __result)
        {
            if (__result == null) return;
            Pawn pawn = parms.pawn;
            if (pawn == null) return;
            if (!HediffComp_SymbioticArmor.SymbioticArmorWeakTable.TryGetValue(pawn, out HediffComp_SymbioticArmor _)) return;

            GraphicStateDef state;
            GraphicStateDef graphicState = !parms.flags.FlagSet(PawnRenderFlags.Portrait) && node.TryGetAnimationGraphicState(parms, out state) ? state : (GraphicStateDef) null;
            Graphic graphic = graphicState == null ? node.PrimaryGraphic : node.GraphicForState(graphicState);
            
            // if (graphic == null) return;
            // if (node.Props.flipGraphic && parms.facing.IsHorizontal) parms.facing = parms.facing.Opposite;
            Material baseMat = graphic.NodeGetMat(parms);
            Material myMat;
            if (!materials.TryGetValue(baseMat, out myMat))
            {
                myMat = InvisibilityMatPool.GetInvisibleMat(baseMat);
                myMat.shader = ShaderDatabase.Cutout;// Cutout CutoutSkin Metalblood;
                myMat.color = pawn.story.SkinColor;
                materials.Add(baseMat, myMat);
            }
            
            __result = myMat;
        }
    }
}