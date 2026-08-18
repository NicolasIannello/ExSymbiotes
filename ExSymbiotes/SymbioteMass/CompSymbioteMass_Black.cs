using Verse;
using RimWorld;

namespace ExSymbiotes
{
  public class CompSymbioteMass_Black : CompObelisk_ExplodingSpawner
  {
    public Building_SymbioteMass Heart => this.parent as Building_SymbioteMass;

    public override void TriggerInteractionEffect(Pawn interactor, bool triggeredByPlayer = false)
    {
      Log.Message(interactor.NameFullColored+" "+triggeredByPlayer);
      if (triggeredByPlayer) this.Heart?.StartTachycardiacOverload();
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
