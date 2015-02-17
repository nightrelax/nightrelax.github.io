using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing;

namespace Skarner
{
    class Skarny 
    {
        private const String ChampName = "Skarner";
        public static Spell Q, W, E, R;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static SpellSlot SmiteSlot;
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != ChampName)
            {
                return;
            }
            SmiteSlot = SpellSlot.Unknown;
            MenuConfig();
            SetSmiteSlot();
            SmiteType();
            SpellConfig();
            Game.PrintChat("<font color='#7A6EFF'>Skarner by Konoe</font> <font color='#FFFFFF'>Loaded!</font>");

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            Combo();
            Laneclear();
            UseShield();
            if (Menu.Item("ManualR").GetValue<KeyBind>().Active)
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                if (R.CanCast(target))
                {
                    R.Cast(target);
                }
            }
        }

        #region Spells
        static void SpellConfig()
        {
            Q = new Spell(SpellSlot.Q, 350f);
            W = new Spell(SpellSlot.W, 0f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 350f);
            E.SetSkillshot(0.25f, 70f, 1500f, false, SkillshotType.SkillshotLine);           
        }
        #endregion

        #region Utility

        private static HitChance GetHitchance()
        {
            switch (Menu.Item("CustomHit").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }
        private static void UseShield()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var WMana = GetSlider("WManaS");
            if (Player.Distance(target) <= E.Range && W.IsReady() && ObjectManager.Player.HealthPercentage() <= Menu.Item("WHealth").GetValue<Slider>().Value && GetPercentage(true) >= WMana)
            {
                W.Cast();
            }

        }

        private static int GetSlider(String opt)
        {
            return Menu.Item(opt).GetValue<Slider>().Value;
        }

        private static float GetPercentage(bool mana)
        {
            return mana ? Player.ManaPercentage() : Player.HealthPercentage();
        }

        public static string SmiteType()
        {
            if (SmiteBlue.Any(itemId => Items.HasItem(itemId)))
            {
                return "s5_summonersmiteplayerganker";
            }
            return SmiteRed.Any(itemId => Items.HasItem(itemId)) ? "s5_summonersmiteduel" : "summonersmite";
        }

        public static void SetSmiteSlot()
        {
            foreach (var spell in
                ObjectManager.Player.Spellbook.Spells.Where(
                    spell => String.Equals(spell.Name, SmiteType(), StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
                return;
            }
        }
        #endregion

        #region Combo

        private static void Combo()
        {
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var QMana = GetSlider("QManaC");
            var EMana = GetSlider("EManaC");
            var RMana = GetSlider("RManaC");

                if (SmiteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready && Menu.Item("UseSmite").GetValue<bool>())
                {
                    Player.Spellbook.CastSpell(SmiteSlot, target);
                }

                if (R.CanCast(target) && Menu.Item("UseRC").GetValue<bool>() && ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health && GetPercentage(true) >= RMana)
                {
                    R.Cast(target);
                }

                if (Q.CanCast(target) && Menu.Item("UseQC").GetValue<bool>() && GetPercentage(true) >= QMana)
                {
                    Q.Cast();
                }

                if (E.CanCast(target) && Menu.Item("UseEC").GetValue<bool>() && E.GetPrediction(target).Hitchance >= CustomHitChance && GetPercentage(true) >= EMana)
                {
                    var pred = E.GetPrediction(target);
                    E.Cast(pred.CastPosition);
                }
                if (Items.HasItem(3143) && Items.CanUseItem(3143) && Player.CountEnemiesInRange(450f) >= 2 && Menu.Item("OmenC").GetValue<bool>())
                {
                Items.UseItem(3143);
                }
        }

        #endregion

        #region Laneclear

        private static void Laneclear()
        {
            var monsters = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
            if (monsters.Count <= 0) return;
            var monster = monsters[0];
            var QManaC = GetSlider("QManaJ");
            var EManaC = GetSlider("EManaJ");
            var WManaC = GetSlider("WManaJ");
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
            {
                return;
            }
            if (Q.CanCast(monster) && Menu.Item("UseQJ").GetValue<bool>() && GetPercentage(true) >= QManaC)
            {
                Q.Cast();
            }
            if (Q.CanCast(monster) && Menu.Item("UseWJ").GetValue<bool>() && GetPercentage(true) >= WManaC)
            {
                W.Cast();

            } if (E.CanCast(monster) && Menu.Item("UseEJ").GetValue<bool>() && GetPercentage(true) >= EManaC)
            {
                E.Cast(monster);
            }


        }

        #endregion

        #region Menu
        static void MenuConfig()
        {
            Menu = new Menu("KonoeSkarner", "Skarner", true);

            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            var tsMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(tsMenu);
            Menu.AddSubMenu(orbwalkMenu);
            Menu.AddSubMenu(tsMenu);
            var comboMenu = new Menu("Combo", "Combo");
            {
                comboMenu.AddItem(new MenuItem("UseQC", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("UseEC", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("UseRC", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("UseSmite", "Use Smite").SetValue(true));

            }
            var manaManagerCombo = new Menu("Mana Manager", "MMCombo");
            {
                manaManagerCombo.AddItem(new MenuItem("QManaC", "Q Mana Combo").SetValue(new Slider(15)));
                manaManagerCombo.AddItem(new MenuItem("EManaC", "E Mana Combo").SetValue(new Slider(25)));
                manaManagerCombo.AddItem(new MenuItem("RManaC", "R Mana Combo").SetValue(new Slider(5)));
            }
            comboMenu.AddSubMenu(manaManagerCombo);
            Menu.AddSubMenu(comboMenu);

            var farmMenu = new Menu("Farm", "Clear");
            {
                farmMenu.AddItem(new MenuItem("UseQJ", "Use Q Clear").SetValue(true));
                farmMenu.AddItem(new MenuItem("UseWJ", "Use W Clear").SetValue(true));
                farmMenu.AddItem(new MenuItem("UseEJ", "Use E Clear").SetValue(true));
            }
            var manaManagerJungle = new Menu("Mana Manager", "MMJungle");
            {
                manaManagerJungle.AddItem(new MenuItem("QManaJ", "Q Mana Jungle").SetValue(new Slider(15)));
                manaManagerJungle.AddItem(new MenuItem("WManaJ", "W Mana Jungle").SetValue(new Slider(35)));
                manaManagerJungle.AddItem(new MenuItem("EManaJ", "E Mana Jungle").SetValue(new Slider(35)));
            }

            farmMenu.AddSubMenu(manaManagerJungle);
            Menu.AddSubMenu(farmMenu);

            var miscMenu = new Menu("Miscellaneous", "Misc");
            {
                miscMenu.AddItem(new MenuItem("WHealth", "W Shield %").SetValue(new Slider(35)));
                miscMenu.AddItem(new MenuItem("WManaS", "W Mana Shield%").SetValue(new Slider(35)));
                miscMenu.AddItem(new MenuItem("CustomHit", "Hitchance").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2)));
                miscMenu.AddItem(new MenuItem("ManualR", "Manual R").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            }
            Menu.AddSubMenu(miscMenu);


            var itemsMenu = new Menu("Item Usage", "Items");
            {
                itemsMenu.AddItem(new MenuItem("OmenC", "Omen Combo").SetValue(true));

            }
            Menu.AddSubMenu(itemsMenu);

            var drawMenu = new Menu("Drawings", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q").SetValue(new Circle(true, System.Drawing.Color.Red, 350f)));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E").SetValue(new Circle(true, System.Drawing.Color.MediumPurple, 1000f)));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R").SetValue(new Circle(true, System.Drawing.Color.MediumPurple, 350f)));
            }
            Menu.AddSubMenu(drawMenu);
            Menu.AddToMainMenu();
        }
        #endregion

        #region Drawing

        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = Menu.Item("DrawQ").GetValue<Circle>();
            var drawE = Menu.Item("DrawE").GetValue<Circle>();
            var drawR = Menu.Item("DrawR").GetValue<Circle>();
            if (drawQ.Active)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, drawQ.Color);
            }

            if (drawE.Active)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, drawE.Color);
            }

            if (drawR.Active)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, drawR.Color);
            }
        }

        #endregion
    }
}
