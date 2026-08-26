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

        public static void Postfix(PawnRenderNodeWorker __instance, PawnRenderNode node, PawnDrawParms parms, ref Material __result)
        {
            if (__result == null) return;
            Pawn pawn = parms.pawn;
            if (pawn == null) return;
            if (!HediffComp_SymbioticArmor.SymbioticArmorWeakTable.TryGetValue(pawn, out HediffComp_SymbioticArmor _)) return;
            if (__instance is PawnRenderNodeWorker_Eye) return;
            
            GraphicStateDef state;
            GraphicStateDef graphicState = !parms.flags.FlagSet(PawnRenderFlags.Portrait) && node.TryGetAnimationGraphicState(parms, out state) ? state : (GraphicStateDef) null;
            Graphic graphic = graphicState == null ? node.PrimaryGraphic : node.GraphicForState(graphicState);
            Material baseMat = graphic.NodeGetMat(parms);
            Material myMat;
            
            if (!materials.TryGetValue(baseMat, out myMat))
            {
                if (!UnityData.IsInMainThread)
                {
                    Log.Error("Anomaly Symbiotes: Attempted to create duplicate material off main thread, do this in ensure materials initialized.");
                    return;
                }
                myMat = new Material(InvisibilityMatPool.GetInvisibleMat(baseMat));
                myMat.shader = ShaderDatabase.CutoutSkin; // Cutout CutoutSkin Metalblood;
                myMat.color = pawn.story.SkinColor;
                myMat.SetColor("_ShadowColor", new Color(0.118f, 0, 0.812f));
                materials.Add(baseMat, myMat);
            }
            
            __result = myMat;
        }
    }
}