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

        public static void Postfix(PawnRenderNodeWorker __instance, PawnRenderNode node, PawnDrawParms parms, ref Material __result)
        {
            if (__result == null) return;
            Pawn pawn = parms.pawn;
            if (pawn == null) return;
            if (!HediffComp_SymbioteBase.SymbioteWeakTable.TryGetValue(pawn, out HediffComp_SymbioteBase symbioteBase)) return;
            if (__instance is PawnRenderNodeWorker_Eye) return;
            
            GraphicStateDef state;
            GraphicStateDef graphicState = !parms.flags.FlagSet(PawnRenderFlags.Portrait) && node.TryGetAnimationGraphicState(parms, out state) ? state : (GraphicStateDef) null;
            Graphic graphic = graphicState == null ? node.PrimaryGraphic : node.GraphicForState(graphicState);
            if (node.Props.flipGraphic && parms.facing.IsHorizontal) parms.facing = parms.facing.Opposite;
            Material baseMat = graphic.NodeGetMat(parms);
            Material symbioticArmorMat;

            if (!materials.TryGetValue((symbioteBase.skin, baseMat), out symbioticArmorMat))
            {
                symbioticArmorMat = new Material(baseMat);
                symbioticArmorMat.shader = ShaderDatabase.CutoutSkin;
                symbioticArmorMat.color = symbioteBase.skin;
                symbioticArmorMat.SetColor("_ShadowColor", symbioteBase.color);
                materials.Add((symbioteBase.skin, baseMat), symbioticArmorMat);
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
    
    [HarmonyPatch(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.HairColor), MethodType.Setter)]
    public static class Patch_Pawn_StoryTracker_HairColorSetter
    {
        public static void Postfix(Pawn_StoryTracker __instance, Pawn ___pawn)
        {
            Hediff hediff = SymbioteUtility.HasSymbiosis(___pawn, false);
            if (hediff != null) hediff.TryGetComp<HediffComp_Symbiosis>().HairChanged(__instance.HairColor);
        }
    }
    
    [HarmonyPatch(typeof(Pawn_GeneTracker), "Notify_GenesChanged")]
    public static class Patch_Pawn_GeneTracker_Notify_GenesChanged
    {
        public static void Postfix(Pawn_GeneTracker __instance)
        {
            Hediff hediff = SymbioteUtility.HasSymbiosis(__instance.pawn, false);
            if (hediff != null) hediff.TryGetComp<HediffComp_Symbiosis>().SkinChanged(__instance.pawn.story.skinColorOverride);
        }
    }
    
    [HarmonyPatch(typeof(Pawn_DraftController), nameof(Pawn_DraftController.Drafted), MethodType.Setter)]
    public static class Patch_Pawn_DraftController_DraftedSetter
    {
        public static void Prefix(Pawn_DraftController __instance)
        {
            Hediff hediff = SymbioteUtility.HasSymbiosis(__instance.pawn, false);
            if (hediff != null) hediff.TryGetComp<HediffComp_Symbiosis>().ChangeVisual();
        }
    }
}