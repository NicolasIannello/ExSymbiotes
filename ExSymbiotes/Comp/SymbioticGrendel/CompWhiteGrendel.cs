using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    [StaticConstructorOnStartup]
    public class CompWhiteGrendel : ThingComp
    {
      public CompProperties_WhiteGrendel Props
      {
        get => (CompProperties_WhiteGrendel) this.props;
      }
      private static readonly MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();
      private static readonly Material mat_east = MaterialPool.MatFrom("Things/Pawn/Grendel/WhiteGrendelSymbiote_east", ShaderDatabase.Cutout);
      private static readonly Material mat_north = MaterialPool.MatFrom("Things/Pawn/Grendel/WhiteGrendelSymbiote_north", ShaderDatabase.Cutout);
      private static readonly Material mat_south = MaterialPool.MatFrom("Things/Pawn/Grendel/WhiteGrendelSymbiote_south", ShaderDatabase.Cutout);
      private static readonly Material mat_west = MaterialPool.MatFrom("Things/Pawn/Grendel/WhiteGrendelSymbiote_west", ShaderDatabase.Cutout);
      private static readonly Material mat_east_fly = MaterialPool.MatFrom("Things/Pawn/Grendel/WhiteGrendelSymbiote_Flying_east", ShaderDatabase.Cutout);
      private static readonly Material mat_north_fly = MaterialPool.MatFrom("Things/Pawn/Grendel/WhiteGrendelSymbiote_Flying_north", ShaderDatabase.Cutout);
      private static readonly Material mat_south_fly = MaterialPool.MatFrom("Things/Pawn/Grendel/WhiteGrendelSymbiote_Flying_south", ShaderDatabase.Cutout);
      private static readonly Material mat_west_fly = MaterialPool.MatFrom("Things/Pawn/Grendel/WhiteGrendelSymbiote_Flying_west", ShaderDatabase.Cutout);

      public override void PostDraw()
      {
        base.PostDraw();
        GrendelThingWithComps comp = ((GrendelThingWithComps)parent);
        Vector3 drawPos = this.parent.DrawPos;
        drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
        Material material = mat_south;
        bool flying = comp.flying;
        switch (comp.target.Rotation.AsInt)
        {
          case 0: 
            material = flying ? mat_north_fly : mat_north;
            drawPos += this.Props.offsetNorth;
            break;
          case 1: 
            material = flying ? mat_east_fly : mat_east; 
            drawPos += this.Props.offsetEast;
            break;
          case 2: 
            material = flying ? mat_south_fly : mat_south;
            drawPos += this.Props.offsetSouth;
            break;
          case 3:
            material = flying ? mat_west_fly : mat_west;
            drawPos += this.Props.offsetWest;
            break;
        }
        CompWhiteGrendel.MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, Color.white);
        Matrix4x4 matrix1 = new Matrix4x4();
        matrix1.SetTRS(drawPos, Quaternion.identity, new Vector3(this.Props.size, 1f, this.Props.size));
        Graphics.DrawMesh(MeshPool.plane10, matrix1, material, 0, (Camera) null, 0, CompWhiteGrendel.MatPropertyBlock);
      }
    }
}