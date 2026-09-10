using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNodeProperties_SpasticSymbiote : PawnRenderNodeProperties
    {
        public bool rotateFacing = true;
        public FloatRange scaleRange = FloatRange.One;
        public FloatRange rotationRange = FloatRange.Zero;
        public FloatRange offsetRangeX = FloatRange.Zero;
        public FloatRange offsetRangeZ = FloatRange.Zero;
        public IntRange durationTicksRange = new IntRange(60, 60);
        public IntRange nextSpasmTicksRange = new IntRange(60, 60);

        public PawnRenderNodeProperties_SpasticSymbiote()
        {
            this.nodeClass = typeof (PawnRenderNode_SpasticSymbiote);
            this.workerClass = typeof (PawnRenderNodeWorker_SpasticSymbiote);
        }
    }
}