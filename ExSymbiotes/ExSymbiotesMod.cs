using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class ExSymbiotesMod : Mod
    {
        public static ExSymbiotesMod Instance;

        public ExSymbiotesMod(ModContentPack content) : base(content)
        {
            Instance = this;
        }

    }

    [DefOf]
    public static class ExSymbiotesDefOf
    {
        public static ThingDef ExSymbiotes_SymbioteMass_Black;
        public static ThingDef ExSymbiotes_SymbioteMassIncoming;
        public static PawnGroupKindDef ExSymbiotes_Symbiote_PawnGroupKind;
        public static EffecterDef ExSymbiotes_TachycardiacArrest;
        public static EffecterDef ExSymbiotes_MeatExplosion_Black;

        static ExSymbiotesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ExSymbiotesDefOf));
        }
    }
}