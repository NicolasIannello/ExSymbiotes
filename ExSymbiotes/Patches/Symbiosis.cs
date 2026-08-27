using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
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
            if (node.Props.flipGraphic && parms.facing.IsHorizontal) parms.facing = parms.facing.Opposite;
            Material baseMat = graphic.NodeGetMat(parms);
            Material symbioticArmorMat;
            
            if (!materials.TryGetValue(baseMat, out symbioticArmorMat))
            {
                symbioticArmorMat = new Material(baseMat);
                symbioticArmorMat.shader = ShaderDatabase.CutoutSkin;
                symbioticArmorMat.color = pawn.story.SkinColor;
                symbioticArmorMat.SetColor("_ShadowColor", new Color(0.118f, 0, 0.812f));
                materials.Add(baseMat, symbioticArmorMat);
            }
            
            __result = symbioticArmorMat;
        }
    }

    [HarmonyPatch(typeof(Thing), nameof(Thing.Ingested))]
    public static class Patch_Thing_Ingested
    {
        public static void Postfix(Thing __instance, Pawn ingester)
        {
            if (!ingester.IsColonist) return;
            if(FoodUtility.IsHumanlikeCorpseOrHumanlikeMeat(__instance, __instance.def))
            {
                Hediff symbiosis = ingester.health.hediffSet.GetFirstHediffOfDef(ExSymbiotesDefOf.ExSymbiotes_Symbiosis);
                if (symbiosis != null)
                {
                    HediffComp_Symbiosis comp = symbiosis.TryGetComp<HediffComp_Symbiosis>();
                    comp.AddSymbiosis(2);
                }
            }
        }
    }
    
}