using Verse;

namespace ExSymbiotes
{
    public class ExSymbiotesMod : Mod
    {
        public static ExSymbiotesMod Instance;

        public ExSymbiotesMod(ModContentPack content) : base(content)
        {
            Instance = this;
        }

    }
}