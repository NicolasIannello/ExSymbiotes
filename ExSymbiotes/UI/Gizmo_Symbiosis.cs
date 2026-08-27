// using UnityEngine;
// using Verse;
//
// namespace ExSymbiotes
// {
//     [StaticConstructorOnStartup]
//     public class Gizmo_Symbiosis : Gizmo
//     {
//         public HediffComp_Symbiosis shield;
//         private readonly Texture2D FullShieldBarTex;
//         static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
//
//         public Gizmo_Symbiosis(Color color) {
//             this.Order = -100f;
//             FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(color);
//         }
//
//         public override float GetWidth(float maxWidth) => 140f;
//
//         public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
//         {
//             Rect rect1 = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
//             Rect rect2 = rect1.ContractedBy(6f);
//             Widgets.DrawWindowBackground(rect1);
//             Rect rect3 = new Rect(rect2.x, rect2.y, rect2.width, rect1.height / 2f);
//             Text.Font = GameFont.Tiny;
//             Widgets.Label(rect3, "ExSymbiotes.Symbiosis".Translate());
//             Rect rect4 = new Rect(rect2.x, rect2.y + (rect2.height / 2f), rect2.width, rect2.height / 2f);
//             float fillPercent = this.shield.Energy / Mathf.Max(1f, this.shield.EnergyMax);
//             Widgets.FillableBar(rect4, fillPercent, FullShieldBarTex, Gizmo_Symbiosis.EmptyShieldBarTex, false);
//             Text.Font = GameFont.Small;
//             Text.Anchor = TextAnchor.MiddleCenter;
//             Widgets.Label(rect4, $"{(this.shield.Energy).ToString("F0")} / {(this.shield.EnergyMax).ToString("F0")}");
//             Text.Anchor = TextAnchor.UpperLeft;
//             TooltipHandler.TipRegion(rect2, (TipSignal) "ExSymbiotes.SymbiosisTip".Translate());
//             return new GizmoResult(GizmoState.Clear);
//         }
//     } 
// }
