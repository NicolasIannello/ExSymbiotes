using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNodeProperties_EyeSymbiote : PawnRenderNodeProperties_Eye
    {
        public override void ResolveReferences() => this.skipFlag = RenderSkipFlagDefOf.None;
    }
}