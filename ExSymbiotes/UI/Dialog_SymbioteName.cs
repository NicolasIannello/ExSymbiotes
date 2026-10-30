using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{

  public class Dialog_SymbioteName : Dialog_GiveName
  {
    protected override int FirstCharLimit => 20;
    private HediffComp_Symbiosis hediffComp;

    public Dialog_SymbioteName(HediffComp_Symbiosis hediff)
    {
      this.suggestingPawn = hediff.parent.pawn;
      this.hediffComp = hediff;
      this.nameMessageKey =
        hediff.bond <= 2 ? "ExSymbiotes.NameSymbioteMessageGood" : "ExSymbiotes.NameSymbioteMessageBad";
      this.gainedNameMessageKey = "ExSymbiotes.SymbioteGainsName";
      this.invalidNameMessageKey = "ExSymbiotes.SymbioteNameIsInvalid";
    }

    public override void DoWindowContents(Rect rect)
    {
      Text.Font = GameFont.Small;
      bool flag = false;
      if (Event.current.type == EventType.KeyDown &&
          (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
      {
        flag = true;
        Event.current.Use();
      }

      Rect rect1;
      Widgets.Label(new Rect(0.0f, 0.0f, rect.width, rect.height),
        this.nameMessageKey
          .Translate((NamedArgument)this.suggestingPawn.LabelShort, (NamedArgument)(Thing)this.suggestingPawn)
          .CapitalizeFirst());
      this.curName = Widgets.TextField(new Rect(0.0f, 80f, (float)((double)rect.width / 2.0 + 70.0), 35f), this.curName,
        this.FirstCharLimit);
      rect1 = new Rect((float)((double)rect.width / 2.0 + 90.0), rect.height - 35f,
        (float)((double)rect.width / 2.0 - 90.0), 35f);
      if (!(Widgets.ButtonText(rect1, (string)"OK".Translate()) | flag))
        return;
      string s1 = this.curName?.Trim();
      if (this.IsValidName(s1))
      {
        this.Named(s1);
        Messages.Message((string)this.gainedNameMessageKey.Translate(this.suggestingPawn.LabelShort, (NamedArgument)s1),
          MessageTypeDefOf.TaskCompletion, false);
        Find.WindowStack.TryRemove((Window)this);
      }
      else
        Messages.Message((string)this.invalidNameMessageKey.Translate(), MessageTypeDefOf.RejectInput, false);

      Event.current.Use();
    }

    protected override bool IsValidName(string s) => NamePlayerFactionDialogUtility.IsValidName(s);

    protected override void Named(string s)
    {
      hediffComp.named = true;
      hediffComp.name = s;
    }
  }
}