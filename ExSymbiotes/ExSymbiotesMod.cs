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
        public static ThingDef ExSymbiotes_Symbiote;
        public static PawnGroupKindDef ExSymbiotes_Symbiote_PawnGroupKind;
        public static PawnGroupKindDef ExSymbiotes_Symbiote_PawnGroupKindRed;
        public static EffecterDef ExSymbiotes_TachycardiacArrest;
        public static EffecterDef ExSymbiotes_MeatExplosion_Black;
        public static IncidentDef ExSymbiotes_SymbioteMass_Incident;
        public static HediffDef ExSymbiotes_Symbiosis;
        public static HediffDef ExSymbiotes_Symbiosis_Red;
        public static HediffDef ExSymbiotes_SymbioteControl;
        public static HediffDef ExSymbiotes_SymbioteControlRed;
        public static AbilityDef ExSymbiotes_SymbiosisLeap;
        public static JobDef ExSymbiotes_SymbioteDigest;
        
        static ExSymbiotesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ExSymbiotesDefOf));
        }
    }
}