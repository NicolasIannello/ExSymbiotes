using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

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
        public static AbilityDef ExSymbiotes_SymbiosisLeap;
        public static DamageDef ExSymbiotes_SymbioticStun;
        public static DutyDef ExSymbiotes_SymbioteStalkWander;
        public static EffecterDef ExSymbiotes_TachycardiacArrest;
        public static EffecterDef ExSymbiotes_MeatExplosion_Black;
        public static EffecterDef ExSymbiotes_VoidStructureActivated;
        public static FactionDef ExSymbiotes_Symbiotes;
        public static HediffDef ExSymbiotes_Symbiosis;
        public static HediffDef ExSymbiotes_Symbiosis_Red;
        public static HediffDef ExSymbiotes_SymbioteControl;
        public static HediffDef ExSymbiotes_SymbioteControlRed;
        public static HediffDef ExSymbiotes_Symbiosis_White;
        public static HediffDef ExSymbiotes_Symbiosis_Yellow;
        public static IncidentDef ExSymbiotes_SymbioteMass_Incident;
        public static JobDef ExSymbiotes_SymbioteDigest;
        public static JobDef ExSymbiotes_SymbioteSwitchToAttackMode;
        public static PawnGroupKindDef ExSymbiotes_Symbiote_PawnGroupKind;
        public static PawnGroupKindDef ExSymbiotes_Symbiote_PawnGroupKindRed;
        public static PawnGroupKindDef ExSymbiotes_Symbiote_PawnGroupKindBlackRed;
        public static PawnGroupKindDef ExSymbiotes_Symbiote_PawnGroupKindRedBlack;
        public static ThingDef ExSymbiotes_SymbioteMass_Black;
        public static ThingDef ExSymbiotes_SymbioteMass_Red;
        public static ThingDef ExSymbiotes_SymbioteMassIncoming;
        public static ThingDef ExSymbiotes_Symbiote;
        public static ThingDef ExSymbiotes_SymbioteRed;
        public static ThoughtDef ExSymbiotes_SymbioteControlledAfter;
        public static ThoughtDef ExSymbiotes_BondedSymbioteLost;

        [MayRequire("Ludeon.RimWorld.Odyssey,Ludeon.RimWorld.Biotech")]
        public static GeneDef VacuumResistance_Total;
        [MayRequireBiotech]
        public static GeneDef PerfectImmunity;
        
        static ExSymbiotesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ExSymbiotesDefOf));
        }
    }
}