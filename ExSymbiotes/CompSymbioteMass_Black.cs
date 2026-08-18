using Verse;
using RimWorld;

namespace ExSymbiotes
{
  public class CompSymbioteMass_Black : CompObelisk_ExplodingSpawner
  {

    public override void TriggerInteractionEffect(Pawn interactor, bool triggeredByPlayer = false)
    {
      Log.Message(interactor.NameFullColored+" "+triggeredByPlayer);
      if (triggeredByPlayer)
      {
        ThingDef serumDef = ThingDef.Named("VoidsightSerum");
        Thing serum = ThingMaker.MakeThing(serumDef);
        serum.stackCount = 1;

        GenPlace.TryPlaceThing(serum, parent.Position, parent.Map, ThingPlaceMode.Direct);
      }
      else
      {
        
      }
    }

    public override void OnActivityActivated()
    {
      base.OnActivityActivated();
      Log.Message("OnActivityActivated");
    }

    public override void CompTick()
    {
      base.CompTick();
      Log.Message("CompTick");
    }
    
    // public override void PostExposeData()
    // {
    //   base.PostExposeData();
    // }
  }
}
