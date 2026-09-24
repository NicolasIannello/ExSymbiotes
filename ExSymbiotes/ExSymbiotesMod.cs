using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
    public enum SymbiosisVisual { Always, Drafted, Never }
    
    public class ExSymbiotesMod : Mod
    {
        public static ExSymbiotesMod Instance;
        public static Harmony Harmony;
        public ExSymbiotesSettings Settings;
        
        public ExSymbiotesMod(ModContentPack content) : base(content)
        {
            Harmony = new Harmony("com.eximeisty.ExSymbiotes");
            Harmony.PatchAll();
            Instance = this;
            Settings = GetSettings<ExSymbiotesSettings>();
        }

        public static SymbiosisVisual Visual => Instance.Settings.Visual;

        public override string SettingsCategory() => "ExSymbiotes";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("ExSymbiotes.SymbiosisSetting".Translate());
            if (listing.ButtonText(Settings.Visual.ToString()))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (SymbiosisVisual visual in Enum.GetValues(typeof(SymbiosisVisual)))
                {
                    options.Add(new FloatMenuOption(("ExSymbiotes.SymbiosisSetting"+visual).Translate(), () => Settings.Visual = visual));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
            
            listing.End();
        }
    }

    public class ExSymbiotesSettings : ModSettings
    {
        public SymbiosisVisual Visual = SymbiosisVisual.Always;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Visual, nameof(Visual), SymbiosisVisual.Always);
        }
    }
    
    [DefOf]
    public static class ExSymbiotesDefOf
    {
        public static AbilityDef ExSymbiotes_SymbiosisLeap;
        public static DamageDef ExSymbiotes_SymbioticStun;
        public static DamageDef ExSymbiotes_VenomLeap;
        public static DutyDef ExSymbiotes_SymbioteStalkWander;
        public static EffecterDef ExSymbiotes_TachycardiacArrest;
        public static EffecterDef ExSymbiotes_MeatExplosion_Black;
        public static EffecterDef ExSymbiotes_VoidStructureActivated;
        public static FactionDef ExSymbiotes_Symbiotes;
        public static FleckDef ExSymbiotes_PsycastPsychicLineBack;
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
        public static ThingDef ExSymbiotes_Mote_HarbingerTreeRoots;
        public static ThingDef ExSymbiotes_Mote_HarbingerTreeRootsBlack;
        public static ThingDef ExSymbiotes_PawnFlyer_VenomLeap;
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