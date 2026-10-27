using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes;

public class SymbioteMoteAtached : Mote
{
  public float Lifespan => this.def.mote.fadeInTime + SolidTime + this.def.mote.fadeOutTime;
  protected override bool EndOfLife => (double) this.AgeSecs >= Lifespan;

  private static readonly List<Vector3> animalHeadOffsets = new List<Vector3>()
  {
    new Vector3(0.0f, 0.0f, 0.4f),
    new Vector3(0.4f, 0.0f, 0.25f),
    new Vector3(0.0f, 0.0f, 0.1f),
    new Vector3(-0.4f, 0.0f, 0.25f)
  };

  public override void SpawnSetup(Map map, bool respawningAfterLoad)
  {
    base.SpawnSetup(map, respawningAfterLoad);
    this.exactPosition = this.exactPosition + this.def.mote.attachedDrawOffset;
  }

  protected override void TimeInterval(float deltaTime)
  {
    base.TimeInterval(deltaTime);
    if (!this.link1.Linked)
      return;
    if (!this.link1.Target.ThingDestroyed & (this.detachAfterTicks == -1 || Find.TickManager.TicksGame - this.spawnTick < this.detachAfterTicks))
    {
      this.link1.UpdateDrawPos();
      if (this.link1.rotateWithTarget)
        this.Rotation = this.link1.Target.Thing.Rotation;
    }
    Vector3 vector3 = this.def.mote.attachedDrawOffset;
    if (this.def.mote.attachedToHead && this.link1.Target.Thing is Pawn thing)
    {
      bool humanlike = thing.RaceProps.Humanlike;
      List<Vector3> headPosPerRotation = thing.RaceProps.headPosPerRotation;
      Rot4 rotation = thing.GetPosture() == PawnPosture.Standing ? (humanlike ? Rot4.North : thing.Rotation) : thing.Drawer.renderer.LayingFacing();
      if (humanlike)
      {
        vector3 = thing.Drawer.renderer.BaseHeadOffsetAt(rotation).RotatedBy(thing.Drawer.renderer.BodyAngle(PawnRenderFlags.None));
      }
      else
      {
        float bodySizeFactor = thing.ageTracker.CurLifeStage.bodySizeFactor;
        Vector2 vector2 = thing.ageTracker.CurKindLifeStage.bodyGraphicData.drawSize * bodySizeFactor;
        vector3 = !headPosPerRotation.NullOrEmpty<Vector3>() ? headPosPerRotation[rotation.AsInt].ScaledBy(new Vector3(vector2.x, 1f, vector2.y)) : SymbioteMoteAtached.animalHeadOffsets[rotation.AsInt] * thing.BodySize;
      }
    }
    this.exactPosition = this.link1.LastDrawPos + vector3;
    IntVec3 intVec3 = this.exactPosition.ToIntVec3();
    if (this.Spawned && !intVec3.InBounds(this.Map))
      this.Destroy();
    else
      this.Position = intVec3;
  }
}