using HarmonyLib;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class ExSymbiotesMod : Mod
    {
        public static ExSymbiotesMod Instance;
        public static Harmony Harmony;

        public ExSymbiotesMod(ModContentPack content) : base(content)
        {
            Harmony = new Harmony("com.eximeisty.ExSymbiotes");
            Harmony.PatchAll();
            Instance = this;
        }

    }

    [DefOf]
    public static class ExSymbiotesDefOf
    {
        public static ThingDef ExSymbiotes_SymbioteMass_Black;
        public static ThingDef ExSymbiotes_SymbioteMass_Red;
        public static ThingDef ExSymbiotes_SymbioteMassIncoming;
        public static PawnGroupKindDef ExSymbiotes_Symbiote_PawnGroupKind;
        public static EffecterDef ExSymbiotes_TachycardiacArrest;
        public static EffecterDef ExSymbiotes_MeatExplosion_Black;
        public static IncidentDef ExSymbiotes_SymbioteMass_Incident;
        public static HediffDef ExSymbiotes_Symbiosis;
        
        static ExSymbiotesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ExSymbiotesDefOf));
        }
    }
}