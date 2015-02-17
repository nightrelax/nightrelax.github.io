using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Trundle
{
    class TMenu
    {
        public static Menu Config;

        public static void AddMenu()
        {
            Config = new Menu("Trundle", "Trundle", true);

            //Orbwalker
            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            T.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            //TS
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Combo
            Config.AddSubMenu(new Menu("Combo", "combo"));
            Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "Use Items")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("useIgniteCombo", "Use Ignite")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("useSmiteCombo", "Use Smite on champ")).SetValue(true);

            //Mixed
            Config.AddSubMenu(new Menu("Mixed", "mix"));
            Config.SubMenu("mix").AddItem(new MenuItem("useHydraMix", "Use Hydra to harass")).SetValue(true);

            //LaneClear
            Config.AddSubMenu(new Menu("LaneClear", "lClear"));
            Config.SubMenu("lClear").AddItem(new MenuItem("useQLC", "Use Q to LaneClear")).SetValue(true);
            Config.SubMenu("lClear").AddItem(new MenuItem("useWLC", "Use W to LaneClear")).SetValue(true);
            Config.SubMenu("lClear").AddItem(new MenuItem("useHydraLC", "Use Hydra")).SetValue(true);

            //TODO: Flee mode
            //Config.AddSubMenu(new Menu("Flee", "flee"));
            //Config.SubMenu("flee").AddItem(new MenuItem("useWFlee", "Use W to flee")).SetValue(true);
            //Config.SubMenu("flee").AddItem(new MenuItem("useEFlee", "Use E to flee")).SetValue(true);

            //Utilities
            Config.AddSubMenu(new Menu("Utilities", "utils"));
            Config.SubMenu("utils").AddItem(new MenuItem("useRTanks", "Force R on tanks in Teamfight")).SetValue(false);
            Config.SubMenu("utils").AddSubMenu(new Menu("Anti Gapcloser", "GC"));
            Config.SubMenu("utils").AddSubMenu(new Menu("Wait for Gapcloser", "waitGC"));
            foreach (var gc in from gc in AntiGapcloser.Spells
                from enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy)
                where gc.ChampionName == enemy.ChampionName
                select gc)
            {
                Config.SubMenu("utils").SubMenu("GC").AddItem(new MenuItem("Gap" + gc.Slot + gc.ChampionName, gc.ChampionName + " " + gc.Slot.ToString())).SetValue(true);
                Config.SubMenu("utils").SubMenu("waitGC").AddItem(new MenuItem("waitGap" + gc.Slot + gc.ChampionName, gc.ChampionName + " " + gc.Slot.ToString())).SetValue(true);
            }
            Config.SubMenu("utils").SubMenu("GC").AddItem(new MenuItem("EGap", "Use E on gapcloser")).SetValue(true);
            Config.SubMenu("utils").AddSubMenu(new Menu("Interrupter", "Inte"));
            foreach (var interr in from interr in Interrupter.Spells
                from enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy)
                where interr.ChampionName == enemy.ChampionName
                select interr)
            {
                Config.SubMenu("utils").SubMenu("Inte").AddItem(new MenuItem("Interr" + interr.Slot + interr.ChampionName, interr.ChampionName + " " + interr.Slot.ToString())).SetValue(true);
            }
            Config.SubMenu("utils").SubMenu("Inte").AddItem(new MenuItem("EInterrupt", "Use E to interrupt important spells")).SetValue(true);
            Config.SubMenu("utils").AddItem(new MenuItem("manualE", "Manual E in Teamfights")).SetValue(true);


            //Drawings
            Config.AddSubMenu(new Menu("Drawings", "drawings"));
            Config.SubMenu("drawings").AddItem(new MenuItem("drawW", "Draw W range")).SetValue(false);
            Config.SubMenu("drawings").AddItem(new MenuItem("drawE", "Draw E range")).SetValue(false);
            Config.SubMenu("drawings").AddItem(new MenuItem("drawR", "Draw R range")).SetValue(false);

            Config.AddToMainMenu();
        }
    }
}
