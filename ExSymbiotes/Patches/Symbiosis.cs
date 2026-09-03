using System.Collections.Generic;
using ExSymbiotes.Utils;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace ExSymbiotes
{
    
    [HarmonyPatch(typeof(PawnRenderNodeWorker), "GetMaterial")]
    public static class Patch_PawnRenderNodeWorker_GetMaterial
    {
        private static readonly Dictionary<(Color color, Material mat), Material> materials = new Dictionary<(Color color, Material mat), Material>();
        private static readonly Color blue= new Color(0.118f, 0, 0.812f);
        private static readonly Color red= new Color(0.812f, 0, 0.118f);

        public static void Postfix(PawnRenderNodeWorker __instance, PawnRenderNode node, PawnDrawParms parms, ref Material __result)
        {
            if (__result == null) return;
            Pawn pawn = parms.pawn;
            if (pawn == null) return;
            if (!HediffComp_SymbioticArmor.SymbioticArmorWeakTable.TryGetValue(pawn, out HediffComp_SymbioticArmor armor) &&
                !HediffComp_SymbioteControl.SymbioteControlWeakTable.TryGetValue(pawn, out HediffComp_SymbioteControl _)) return;
            if (__instance is PawnRenderNodeWorker_Eye) return;
            
            GraphicStateDef state;
            GraphicStateDef graphicState = !parms.flags.FlagSet(PawnRenderFlags.Portrait) && node.TryGetAnimationGraphicState(parms, out state) ? state : (GraphicStateDef) null;
            Graphic graphic = graphicState == null ? node.PrimaryGraphic : node.GraphicForState(graphicState);
            if (node.Props.flipGraphic && parms.facing.IsHorizontal) parms.facing = parms.facing.Opposite;
            Material baseMat = graphic.NodeGetMat(parms);
            Material symbioticArmorMat;
            Color color = armor != null ? blue : red;

            if (!materials.TryGetValue((color, baseMat), out symbioticArmorMat))
            {
                symbioticArmorMat = new Material(baseMat);
                symbioticArmorMat.shader = ShaderDatabase.CutoutSkin;
                symbioticArmorMat.color = pawn.story.SkinColor;
                symbioticArmorMat.SetColor("_ShadowColor", color);
                materials.Add((color, baseMat), symbioticArmorMat);
            }
            
            __result = symbioticArmorMat;
        }
    }

    [HarmonyPatch(typeof(Corpse), "IngestedCalculateAmounts")]
    public static class Patch_Corpse_IngestedCalculateAmounts
    {
        public static void Prefix(Corpse __instance, Pawn ingester, float nutritionWanted, out bool __state)
        {
            if (!ingester.IsColonist)
            {
                __state = false;
                return;
            }
            
            BodyPartRecord brain = __instance.InnerPawn.health.hediffSet.GetBrain();
            bool hasBrain = brain != null && !__instance.InnerPawn.health.hediffSet.PartIsMissing(brain);
            __state = hasBrain;
        }

        public static void Postfix(Corpse __instance, Pawn ingester, float nutritionWanted, bool __state)
        {
            if (!ingester.IsColonist || !__state) return;
            
            Hediff symbiosis = SymbioteUtility.HasSymbiosis(ingester);
            if (symbiosis != null)
            {
                BodyPartRecord brain = __instance.InnerPawn.health.hediffSet.GetBrain();
                bool hasBrain = brain != null && !__instance.InnerPawn.health.hediffSet.PartIsMissing(brain);

                if (hasBrain)
                {
                    Hediff_MissingPart hediffMissingPart = (Hediff_MissingPart) HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, __instance.InnerPawn, brain);
                    hediffMissingPart.lastInjury = HediffDefOf.Bite;
                    hediffMissingPart.IsFresh = true;
                    __instance.InnerPawn.health.AddHediff((Hediff) hediffMissingPart);
                }
                
                HediffComp_Symbiosis comp = symbiosis.TryGetComp<HediffComp_Symbiosis>();
                comp.AddSymbiosis(2);
            }
        }
    }
}