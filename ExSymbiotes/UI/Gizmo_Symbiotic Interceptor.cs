using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    [StaticConstructorOnStartup]
    public class Gizmo_Symbiotic_Interceptor : Gizmo
    {
      public CompSymbioticInterceptor interceptor;
      private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
      private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
      private const float Width = 140f;
      public const int InRectPadding = 6;

      public Gizmo_Symbiotic_Interceptor() => this.Order = -100f;

      public override float GetWidth(float maxWidth) => Width;

      public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
      {
        Rect rect1 = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
        Rect rect2 = rect1.ContractedBy(InRectPadding);
        Widgets.DrawWindowBackground(rect1);
        int num = this.interceptor.ChargingTicksLeft > 0 ? 1 : 0;
        TaggedString label1 = $"{"ExSymbiotes.ShieldSymbiotic".Translate()} - {(this.interceptor.RemainingTicks / 60).ToString()}s";
        float fillPercent = num == 0 ? (float) this.interceptor.currentHitPoints / (float) this.interceptor.HitPointsMax : (float) this.interceptor.ChargingTicksLeft / (float) this.interceptor.Props.chargeDurationTicks;
        string label2 = num == 0 ? $"{this.interceptor.currentHitPoints.ToString()} / {this.interceptor.HitPointsMax.ToString()}" : this.interceptor.ChargingTicksLeft.ToStringTicksToPeriod();
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        Rect rect3 = new Rect(rect2.x, rect2.y - 2f, rect2.width, rect2.height / 2f);
        Widgets.Label(rect3, label1);
        Rect rect4 = new Rect(rect2.x, rect3.yMax, rect2.width, rect2.height / 2f);
        Widgets.FillableBar(rect4, fillPercent, Gizmo_Symbiotic_Interceptor.FullBarTex, Gizmo_Symbiotic_Interceptor.EmptyBarTex, false);
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect4, label2);
        Text.Anchor = TextAnchor.UpperLeft;
        return new GizmoResult(GizmoState.Clear);
      }
    }
}