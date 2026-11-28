using Verse;

namespace ExSymbiotes
{
    public class HediffComp_SymbioticScythe: HediffComp
    {
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            Thing item = ThingMaker.MakeThing(ExSymbiotesDefOf.ExSymbiotes_SymbioteScythe);
            item.stackCount = 1;
            
            ThingWithComps weapon = Pawn.equipment.Primary;
            if (weapon != null) Pawn.equipment.TryDropEquipment(weapon, out ThingWithComps _, Pawn.Position, forbid: true);
            
            Pawn.equipment.AddEquipment((ThingWithComps)item);
        }

        public override void CompPostPostRemoved()
        {
            if (Pawn.equipment.Primary != null && Pawn.equipment.Primary.def == ExSymbiotesDefOf.ExSymbiotes_SymbioteScythe)
                Pawn.equipment.TryDropEquipment(Pawn.equipment.Primary, out ThingWithComps _, Pawn.Position, forbid: true);
        }
    }
}