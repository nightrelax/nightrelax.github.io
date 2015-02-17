using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc_2
{
    internal class Spells
    {
        public static Spell Q, W, E, R;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 970);
            R = new Spell(SpellSlot.R);

            W.SetSkillshot(0.5f, 220, 1500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.366f, 70, 1600, true, SkillshotType.SkillshotLine);

            var name = R.Instance.Name;

            switch (name)
            {
                case "LeblancChaosOrbM":
                    {
                        R = new Spell(SpellSlot.R, Q.Range);
                        break;
                    }
                case "LeblancSlideM":
                    {
                        R = new Spell(SpellSlot.R, W.Range);
                        R.SetSkillshot(0.5f, 220, 1500, false, SkillshotType.SkillshotCircle);
                        break;
                    }
                case "LeblancSoulShackleM":
                    {
                        R = new Spell(SpellSlot.R, E.Range);
                        R.SetSkillshot(0.366f, 70, 1600, true, SkillshotType.SkillshotLine);
                        break;
                    }
            }
        }
    }
}
