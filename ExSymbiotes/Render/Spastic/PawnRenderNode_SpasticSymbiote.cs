using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class PawnRenderNode_SpasticSymbiote: PawnRenderNode
    {
      public PawnRenderNode_SpasticSymbiote(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree) { }
      
      protected PawnRenderNode_SpasticSymbiote.SpasmData spasmData;

      public override GraphicMeshSet MeshSetFor(Pawn pawn)
      {
        return new GraphicMeshSet(MeshPool.GridPlane(this.props.overrideMeshSize ?? this.props.drawSize));
      }

      public bool CheckAndDoSpasm(
        PawnDrawParms parms,
        out PawnRenderNode_SpasticSymbiote.SpasmData dat,
        out float progress)
      {
        if (parms.pawn.Dead || !(this.props is PawnRenderNodeProperties_SpasticSymbiote props) || parms.Portrait || parms.Cache || parms.Statue)
        {
          progress = 0.0f;
          dat = (PawnRenderNode_SpasticSymbiote.SpasmData) null;
          return false;
        }
        if (this.spasmData == null)
          this.spasmData = new PawnRenderNode_SpasticSymbiote.SpasmData();
        if (Find.TickManager.TicksGame >= this.spasmData.nextSpasm)
        {
          this.spasmData.tickStart = Find.TickManager.TicksGame;
          this.spasmData.duration = (float) this.GetNextSpasmDurationTicks();
          this.spasmData.nextSpasm = this.GetNextSpasmTick();
          this.spasmData.rotationStart = this.spasmData.rotationTarget;
          this.spasmData.rotationTarget = props.rotationRange.RandomInRange;
          this.spasmData.scaleStart = this.spasmData.scaleTarget;
          this.spasmData.scaleTarget = props.scaleRange.RandomInRange;
          this.spasmData.offsetStart = this.spasmData.offsetTarget;
          this.spasmData.offsetTarget = new Vector3(props.offsetRangeX.RandomInRange, 0.0f, props.offsetRangeZ.RandomInRange);
        }
        progress = (float) (Find.TickManager.TicksGame - this.spasmData.tickStart) / Mathf.Max(this.spasmData.duration, 0.0001f);
        dat = this.spasmData;
        return true;
      }

      protected virtual int GetNextSpasmTick()
      {
        return this.props is PawnRenderNodeProperties_SpasticSymbiote props ? this.spasmData.tickStart + (int) this.spasmData.duration + props.nextSpasmTicksRange.RandomInRange : 0;
      }

      protected virtual int GetNextSpasmDurationTicks()
      {
        return this.props is PawnRenderNodeProperties_SpasticSymbiote props ? props.durationTicksRange.RandomInRange : 0;
      }

      public class SpasmData
      {
        public float rotationStart;
        public float rotationTarget;
        public float scaleStart;
        public float scaleTarget;
        public Vector3 offsetStart;
        public Vector3 offsetTarget;
        public int tickStart;
        public int nextSpasm;
        public float duration;

        public SpasmData()
        {
          this.duration = 1f;
          this.scaleStart = this.scaleTarget = 1f;
        }
      }
    }
}