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
        public int completeDigestionDamage = 125;
        public SimpleCurve bodySizeDigestTimeCurve = new SimpleCurve()
        {
            {
                new CurvePoint(0.2f, 10f),
                true
            },
            {
                new CurvePoint(1f, 60f),
                true
            },
            {
                new CurvePoint(3.5f, 90f),
                true
            }
        };
        public SimpleCurve timeDamageCurve = new SimpleCurve()
        {
            {
                new CurvePoint(0.0f, 5f),
                true
            },
            {
                new CurvePoint(60f, 35f),
                true
            }
        };

        public CompProperties_Symbiote() => this.compClass = typeof (CompSymbiote);
    }
}