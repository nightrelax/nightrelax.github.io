using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc_2
{
    internal class Damages
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static float GetComboDamage(Obj_AI_Base target)
        {
            double dmg = 0;

            if (Spells.Q.IsReady())
                dmg += 2*Player.GetSpellDamage(target, SpellSlot.Q);

            if (Spells.W.IsReady())
                dmg += Player.GetSpellDamage(target, SpellSlot.W);

            if (Spells.E.IsReady())
                dmg += Player.GetSpellDamage(target, SpellSlot.E);

            if (target.HasBuff("leblancchaosorbm", true))
            {
                dmg += Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (target.HasBuff("leblancchaosorb", true))
            {
                dmg += Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (Spells.R.IsReady())
            {
                var maxQdmg = Player.CalcDamage(target, Damage.DamageType.Magical, new float[] { 200, 400, 600 }[Spells.R.Level] + 1.3 * Player.TotalMagicalDamage());
                switch (Spells.R.Instance.Name)
                {
                    case "LeblancChaosOrbM":
                    {
                        var qDmg = Player.CalcDamage(target, Damage.DamageType.Magical, new float[] { 100, 200, 300 }[Spells.R.Level] + 0.65 * Player.TotalMagicalDamage());

                        if (maxQdmg < qDmg)
                            dmg += maxQdmg;
                        else
                            dmg += qDmg;
                        break;
                    }
                    case "LeblancSlideM":
                    {
                        var wDmg = Player.CalcDamage(
                            target, Damage.DamageType.Magical,
                            new float[] { 100, 200, 300 }[Spells.R.Level] + 0.65 * Player.TotalMagicalDamage());

                        dmg += wDmg;
                        break;
                    }
                    case "LeblancSoulShackleM":
                    {
                        var eDmg = Player.CalcDamage(target, Damage.DamageType.Magical, new float[] { 100, 200, 300 }[Spells.R.Level] + 0.65 * Player.TotalMagicalDamage());

                        if (maxQdmg < eDmg)
                            dmg += maxQdmg;
                        else
                            dmg += eDmg;
                        break;
                    }
                }
            }

            return (float)dmg;
        }
    }
}
