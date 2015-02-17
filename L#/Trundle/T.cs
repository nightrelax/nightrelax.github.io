using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Trundle
{
    internal class T
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Orbwalking.Orbwalker Orbwalker;

        public static SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");
        public static SpellSlot smiteSlot = SpellSlot.Unknown;

        //Credits to Kurisu for Smite Stuff :^)
        public static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        public static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        public static Spellbook SBook = Player.Spellbook;
        public static SpellDataInst Qdata = SBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = SBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = SBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = SBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 125f);
        public static Spell W = new Spell(SpellSlot.W, 900f);
        public static Spell E = new Spell(SpellSlot.E, 1000f);
        public static Spell R = new Spell(SpellSlot.R, 700f);
        public static Spell Smite;

        public static void SetSkillShots()
        {
            W.SetSkillshot(.5f, 750f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);
        }

        public static void Combo(Obj_AI_Hero target)
        {
            if (TMenu.Config.Item("useIgniteCombo").GetValue<bool>())
            {
                Use.UseIgnite(target);
            }
            if (TMenu.Config.Item("comboItems").GetValue<bool>())
            {
                Use.UseComboItems(target);
            }
            if (TMenu.Config.Item("useSmiteCombo").GetValue<bool>())
            {
                Use.UseSmiteOnChamp(target);
            }
            if ((!OutgoingDamage.IsMovingToMe(target) && 
                target.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(target)) ||
                (target.Distance(ObjectManager.Player) <= Orbwalking.GetRealAutoAttackRange(target) && 
                ObjectManager.Player.HealthPercentage() < 60))
            {
                Use.UseWCombo(target);
            }
            if (!OutgoingDamage.IsMovingToMe(target) &&
                target.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(target))
            {
                Use.UseECombo(target);
            }

            if (ObjectManager.Player.HealthPercentage() < 60 || target.Health < R.GetDamage(target) &&
                !TMenu.Config.Item("useRTanks").GetValue<bool>())
            {
                Use.UseRCombo(target);
            }

            if (ObjectManager.Player.HealthPercentage() < 70 &&
               TMenu.Config.Item("useRTanks").GetValue<bool>() && ObjectManager.Player.CountEnemysInRange(2000f) >= 2)
            {
                Use.UseRTanks();
            }
        }

        public static void Mixed(Obj_AI_Hero target)
        {
            if (TMenu.Config.Item("useHydraMix").GetValue<bool>())
            {
                Use.UseHydra(target);
            }
        }

        public static void LaneClear()
        {
            if (TMenu.Config.Item("useHydraLC").GetValue<bool>())
            {
                Use.UseHydraLc();
            }
            if (TMenu.Config.Item("useWLC").GetValue<bool>())
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, W.Range);
                if (allMinions.Count > 4)
                {
                    W.Cast(ObjectManager.Player.Position);
                }
            }

        }

        public static string GetSmiteType()
        {
            if (SmiteBlue.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(id => Items.HasItem(id)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        public static void GetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => String.Equals(spell.Name, GetSmiteType(), StringComparison.CurrentCultureIgnoreCase)))
            {
                smiteSlot = spell.Slot;
                Smite = new Spell(smiteSlot, 700);
                return;
            }
        }
    }
}