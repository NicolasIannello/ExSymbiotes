using Verse;

namespace ExSymbiotes
{
    public class CompGrendelSymbiote : ThingComp
    {
      public bool flying = false;

      public override void PostExposeData()
      {
        base.PostExposeData();
        Scribe_Values.Look<bool>(ref flying, "flying");
      }
    }
}