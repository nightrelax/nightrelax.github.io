using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace PerplexedEzreal
{
    class Config
    {
        public static Menu Settings = new Menu("Perplexed Ezreal", "menu", true);
        public static Orbwalking.Orbwalker Orbwalker;

        public static string[] Marksmen = { "Kalista", "Jinx", "Lucian", "Quinn", "Draven",  "Varus", "Graves", "Vayne", "Caitlyn",
                                                                    "Urgot", "Ezreal", "KogMaw", "Ashe", "MissFortune", "Tristana", "Teemo", "Sivir",
                                                                    "Twitch", "Corki"};

        public static void Initialize()
        {
            //Orbwalker
            Settings.AddSubMenu(new Menu("Orbwalker", "orbMenu"));
            Orbwalker = new Orbwalking.Orbwalker(Settings.SubMenu("orbMenu"));
            //Target Selector
            Settings.AddSubMenu(new Menu("Target Selector", "ts"));
            TargetSelector.AddToMenu(Settings.SubMenu("ts"));
            //Combo
            Settings.AddSubMenu(new Menu("Combo", "menuCombo"));
            Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboQ", "Q").SetValue(true));
            Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboW", "W").SetValue(true));
            //Harass
            Settings.AddSubMenu(new Menu("Harass", "menuHarass"));
            Settings.SubMenu("menuHarass").AddItem(new MenuItem( "harassQ", "Q").SetValue(true));
            Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassW", "W").SetValue(true));
            //Auto Harass
            Settings.AddSubMenu(new Menu("Auto Harass", "menuAuto"));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("toggleAuto", "Toggle Auto").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle, true)));
            Settings.SubMenu("menuAuto").AddSubMenu(new Menu("Champions", "autoChamps"));
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValid && hero.IsEnemy))
                Settings.SubMenu("menuAuto").SubMenu("autoChamps").AddItem(new MenuItem("auto" + hero.ChampionName, hero.ChampionName).SetValue(Marksmen.Contains(hero.ChampionName)));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoQ", "Q").SetValue(true));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoW", "W").SetValue<bool>(false));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("manaER", "Save Mana For E/R").SetValue(true));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoTurret", "Harass Enemy Under Turret").SetValue<bool>(false));
            //Last Hit
            Settings.AddSubMenu(new Menu("Last Hitting", "menuLastHit"));
            Settings.SubMenu("menuLastHit").AddItem(new MenuItem("lastHitQ", "Q").SetValue(true));
            //Anti-Gapcloser
            Settings.AddSubMenu(new Menu("Anti-Gapcloser", "menuGapCloser"));
            Settings.SubMenu("menuGapCloser").AddItem(new MenuItem("gapcloseE", "Dodge With E").SetValue(true));
            //Ultimate
            Settings.AddSubMenu(new Menu("Ult Settings", "menuUlt"));
            Settings.SubMenu("menuUlt").AddItem(new MenuItem("ultLowest", "Ult Lowest Target").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Settings.SubMenu("menuUlt").AddItem(new MenuItem("ks", "Kill Steal With R").SetValue(true));
            Settings.SubMenu("menuUlt").AddItem(new MenuItem("ultRange", "Ult Range").SetValue<Slider>(new Slider(1000, 1000, 5000)));
            //Summoners
            Settings.AddSubMenu(new Menu("Summoners", "menuSumms"));
            Settings.SubMenu("menuSumms").AddSubMenu(new Menu("Heal", "summHeal"));
            Settings.SubMenu("menuSumms").SubMenu("summHeal").AddItem(new MenuItem("useHeal", "Enabled").SetValue(true));
            Settings.SubMenu("menuSumms").SubMenu("summHeal").AddItem(new MenuItem("healPct", "Use On % Health").SetValue(new Slider(35, 10, 90)));
            Settings.SubMenu("menuSumms").AddSubMenu(new Menu("Ignite", "summIgnite"));
            Settings.SubMenu("menuSumms").SubMenu("summIgnite").AddItem(new MenuItem("useIgnite", "Enabled").SetValue(true));
            Settings.SubMenu("menuSumms").SubMenu("summIgnite").AddItem(new MenuItem("igniteMode", "Use Ignite For").SetValue(new StringList(new string[] { "Execution", "Combo" })));
            //Items
            Settings.AddSubMenu(new Menu("Items", "menuItems"));
            Settings.SubMenu("menuItems").AddSubMenu(new Menu("Offensive", "offItems"));
            foreach (var offItem in ItemManager.Items.Where(item => item.Type == ItemType.Offensive))
                Settings.SubMenu("menuItems").SubMenu("offItems").AddItem(new MenuItem("use" + offItem.ShortName, offItem.Name).SetValue(true));
            Settings.SubMenu("menuItems").AddSubMenu(new Menu("Defensive", "defItems"));
            foreach (var defItem in ItemManager.Items.Where(item => item.Type == ItemType.Defensive))
            {
                Settings.SubMenu("menuItems").SubMenu("defItems").AddSubMenu(new Menu(defItem.Name, "menu" + defItem.ShortName));
                Settings.SubMenu("menuItems").SubMenu("defItems").SubMenu("menu" + defItem.ShortName).AddItem(new MenuItem("use" + defItem.ShortName, "Enable").SetValue(true));
                Settings.SubMenu("menuItems").SubMenu("defItems").SubMenu("menu" + defItem.ShortName).AddItem(new MenuItem("pctHealth" + defItem.ShortName, "Use On % Health").SetValue(new Slider(35, 10, 90)));
            }
            Settings.SubMenu("menuItems").AddSubMenu(new Menu("Cleanse", "cleanseItems"));
            foreach (var cleanseItem in ItemManager.Items.Where(item => item.Type == ItemType.Cleanse))
                Settings.SubMenu("menuItems").SubMenu("cleanseItems").AddItem(new MenuItem("use" + cleanseItem.ShortName, cleanseItem.Name).SetValue(true));
            //Drawing
            Settings.AddSubMenu(new Menu("Drawing", "menuDrawing"));
            Settings.SubMenu("menuDrawing").AddSubMenu(new Menu("Damage Indicator", "menuDamage"));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawAADmg", "Draw Auto Attack Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawQDmg", "Draw Q Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawWDmg", "Draw W Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawEDmg", "Draw E Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawRDmg", "Draw R Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawW", "Draw W Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawR", "Draw R Range").SetValue(new Circle(true, Color.Yellow)));
            //Other
            Settings.AddItem(new MenuItem("dmgMode", "Damage Mode").SetValue(new StringList(new string[] { "AD", "AP" })));
            Settings.AddItem(new MenuItem("recallBlock", "Recall Block").SetValue(true));
            Settings.AddItem(new MenuItem("usePackets", "Use Packets").SetValue(true));
            //Finish
            Settings.AddToMainMenu();
        }

        public static bool ComboQ { get { return Settings.Item("comboQ").GetValue<bool>(); } }
        public static bool ComboW { get { return Settings.Item("comboW").GetValue<bool>(); } }

        public static bool HarassQ { get { return Settings.Item("harassQ").GetValue<bool>(); } }
        public static bool HarassW { get { return Settings.Item("harassW").GetValue<bool>(); } }

        public static bool LastHitQ { get { return Settings.Item("lastHitQ").GetValue<bool>(); } }

        public static bool GapcloseE { get { return Settings.Item("gapcloseE").GetValue<bool>();  } }

        public static KeyBind UltLowest { get { return Settings.Item("ultLowest").GetValue<KeyBind>(); } }
        public static bool KillSteal { get { return Settings.Item("ks").GetValue<bool>(); } }
        public static int UltRange { get { return Settings.Item("ultRange").GetValue<Slider>().Value; } }

        public static KeyBind ToggleAuto { get { return Settings.Item("toggleAuto").GetValue<KeyBind>(); } }
        public static bool ShouldAuto(string championName)
        {
            return Settings.Item("auto" + championName).GetValue<bool>();
        }
        public static bool AutoQ { get { return Settings.Item("autoQ").GetValue<bool>(); } }
        public static bool AutoW { get { return Settings.Item("autoW").GetValue<bool>(); } }
        public static bool ManaER { get { return Settings.Item("manaER").GetValue<bool>(); } }
        public static bool AutoTurret { get { return Settings.Item("autoTurret").GetValue<bool>(); } }

        public static bool UseHeal { get { return Settings.Item("useHeal").GetValue<bool>(); } }
        public static int HealPct { get { return Settings.Item("healPct").GetValue<Slider>().Value; } }
        public static bool UseIgnite { get { return Settings.Item("useIgnite").GetValue<bool>(); } }
        public static string IgniteMode { get { return Settings.Item("igniteMode").GetValue<StringList>().SelectedValue; } }

        public static bool ShouldUseItem(string shortName)
        {
            return Settings.Item("use" + shortName).GetValue<bool>();
        }
        public static int UseOnPercent(string shortName)
        {
            return Settings.Item("pctHealth" + shortName).GetValue<Slider>().Value;
        }

        public static bool DrawAADmg { get { return Settings.Item("drawAADmg").GetValue<bool>(); } }
        public static bool DrawQDmg { get { return Settings.Item("drawQDmg").GetValue<bool>(); } }
        public static bool DrawWDmg { get { return Settings.Item("drawWDmg").GetValue<bool>(); } }
        public static bool DrawEDmg { get { return Settings.Item("drawEDmg").GetValue<bool>(); } }
        public static bool DrawRDmg { get { return Settings.Item("drawRDmg").GetValue<bool>(); } }
        public static bool DrawQ { get { return Settings.Item("drawQ").GetValue<Circle>().Active; } }
        public static bool DrawW { get { return Settings.Item("drawW").GetValue<Circle>().Active; } }
        public static bool DrawR { get { return Settings.Item("drawR").GetValue<Circle>().Active; } }

        public static string DamageMode { get { return Settings.Item("dmgMode").GetValue<StringList>().SelectedValue; } }
        public static bool RecallBlock { get { return Settings.Item("recallBlock").GetValue<bool>(); } }
        public static bool UsePackets { get { return Settings.Item("usePackets").GetValue<bool>(); } }
    }
}
