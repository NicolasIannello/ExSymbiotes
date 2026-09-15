using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompProperties_Symbiote : CompProperties
    {
        [MustTranslate]
        public string messageDigested;
        [MustTranslate]
        public string messageEmerged;
        [MustTranslate]
        public string messageEmergedCorpse;
        [MustTranslate]
        public string messageDigestionCompleted;
        [MustTranslate]
        public string digestingInspector;
        public int digestTime = 20;
        public int digestTimeNonHumanlike = 5;
        [MustTranslate]
        public string messageRetaliate;
        [MustTranslate]
        public string messageEmergedCause;

        public CompProperties_Symbiote() => this.compClass = typeof (CompSymbiote);
    }
}