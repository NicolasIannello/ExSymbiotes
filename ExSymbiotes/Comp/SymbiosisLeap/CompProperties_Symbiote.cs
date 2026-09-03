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
        public int digestTime = 30;

        public CompProperties_Symbiote() => this.compClass = typeof (CompSymbiote);
    }
}