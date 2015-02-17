using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace LeBlanc_2
{
    internal class Configs
    {
        public static Menu LeBlancConfig;
        public static Menu TargetSelectorMenu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void Init()
        {
            LeBlancConfig = new Menu("Princess " + ObjectManager.Player.ChampionName, "Princess" + ObjectManager.Player.ChampionName, true);

            LeBlancConfig.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(LeBlancConfig.SubMenu("Orbwalking"));

            TargetSelectorMenu = new Menu("Target Selector", "Common_TargetSelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            LeBlancConfig.AddSubMenu(TargetSelectorMenu);

            AssassinManager.Load();

            LeBlancConfig.AddSubMenu(new Menu("Combo", "Combo"));
            LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(LeBlancConfig.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));
            LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E").SetValue(true));
            LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));
            LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("preE", "Minimum HitChance E").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
            LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("preR", "Minimum HitChance R(E)").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));

            LeBlancConfig.AddSubMenu(new Menu("LaneClear", "laneclear"));
            LeBlancConfig.SubMenu("laneclear").AddItem(new MenuItem("laneClearActive", "LaneClear").SetValue(new KeyBind(LeBlancConfig.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));
            LeBlancConfig.SubMenu("laneclear").AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            LeBlancConfig.SubMenu("laneclear").AddItem(new MenuItem("LaneClearW", "Use W").SetValue(true));
            LeBlancConfig.SubMenu("laneclear").AddItem(new MenuItem("LaneClear2W", "Use Second W").SetValue(true));
            LeBlancConfig.SubMenu("laneclear").AddItem(new MenuItem("LaneClearWHit", "Min minions by W").SetValue(new Slider(2, 0, 5)));
            LeBlancConfig.SubMenu("laneclear").AddItem(new MenuItem("LaneClearManaPercent", "Minimum Mana Percent").SetValue(new Slider(30)));

            LeBlancConfig.AddSubMenu(new Menu("Harass", "harass"));
            LeBlancConfig.SubMenu("harass").AddItem(new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind(LeBlancConfig.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));
            LeBlancConfig.SubMenu("harass").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            LeBlancConfig.SubMenu("harass").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            LeBlancConfig.SubMenu("harass").AddItem(new MenuItem("use2W", "Use Second W").SetValue(true));
            LeBlancConfig.SubMenu("harass").AddItem(new MenuItem("useE", "Use E").SetValue(true));
            LeBlancConfig.SubMenu("harass").AddItem(new MenuItem("HarassManaPercent", "Minimum Mana Percent").SetValue(new Slider(30)));
            LeBlancConfig.SubMenu("harass").AddItem(new MenuItem("haraKey", "Harass Toggle").SetValue(new KeyBind('Y', KeyBindType.Toggle)));

            LeBlancConfig.AddSubMenu(new Menu("Flee", "Flee"));
            LeBlancConfig.SubMenu("Flee").AddItem(new MenuItem("FleeK", "Key").SetValue(new KeyBind('A', KeyBindType.Press)));
            LeBlancConfig.SubMenu("Flee").AddItem(new MenuItem("FleeW", "Use W").SetValue(true));
            LeBlancConfig.SubMenu("Flee").AddItem(new MenuItem("FleeE", "Use E").SetValue(true));
            LeBlancConfig.SubMenu("Flee").AddItem(new MenuItem("FleeR", "Use R").SetValue(true));
            LeBlancConfig.SubMenu("Flee").AddItem(new MenuItem("FleeSecondW", "Use Second W if Cursor over it").SetValue(true));

            LeBlancConfig.AddSubMenu(new Menu("Drawing", "Draw"));
            LeBlancConfig.SubMenu("Draw").AddItem(new MenuItem("drawQ", "Draw Q").SetValue(new Circle(true, Color.AntiqueWhite)));
            LeBlancConfig.SubMenu("Draw").AddItem(new MenuItem("drawW", "Draw W").SetValue(new Circle(true, Color.AntiqueWhite)));
            LeBlancConfig.SubMenu("Draw").AddItem(new MenuItem("drawE", "Draw E").SetValue(new Circle(true, Color.AntiqueWhite)));
            MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
            MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
            LeBlancConfig.SubMenu("Draw").AddItem(drawComboDamageMenu);
            LeBlancConfig.SubMenu("Draw").AddItem(drawFill);
            DamageIndicator.DamageToUnit = Damages.GetComboDamage;
            DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
            drawComboDamageMenu.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };
            drawFill.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };

            LeBlancConfig.AddSubMenu(new Menu("Misc", "Misc"));
            LeBlancConfig.SubMenu("Misc").AddSubMenu(new Menu("Second W Logic", "backW"));
            LeBlancConfig.SubMenu("Misc").SubMenu("backW").AddItem(new MenuItem("SWpos", "If Cursor is hover Second W").SetValue(true));
            LeBlancConfig.SubMenu("Misc").AddItem(new MenuItem("Interrupt", "Interrupt with E").SetValue(true));
            LeBlancConfig.SubMenu("Misc").AddItem(new MenuItem("Gapclose", "Anit Gapclose with E").SetValue(true));
            LeBlancConfig.SubMenu("Misc").AddItem(new MenuItem("UsePacket", "Use Packets").SetValue(false));
            LeBlancConfig.SubMenu("Misc").AddItem(new MenuItem("Clone", "Clone Logic").SetValue(true));

            LeBlancConfig.AddToMainMenu();
        }
    }
}
