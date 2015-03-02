﻿using System;
﻿using System.Linq;
﻿using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle.Extensions
{
    internal static class Cleansers
    {
        private static Menu _menuConfig, _mainMenu;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            _mainMenu = new Menu("Cleansers", "cmenu");
            _menuConfig = new Menu("Cleansers Config", "cconfig");

            foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == Me.Team))
                _menuConfig.AddItem(new MenuItem("cccon" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);
            _mainMenu.AddSubMenu(_menuConfig);

            CreateMenuItem("Dervish Blade", "Dervish", 2);
            CreateMenuItem("Quicksilver Sash", "Quicksilver", 2);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 2);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 2);

            // delay the cleanse value * 100
            _mainMenu.AddItem(new MenuItem("cleansedelay", "Cleanse delay ")).SetValue(new Slider(0, 0, 25));

            _mainMenu.AddItem(
                new MenuItem("cmode", "Mode: "))
                .SetValue(new StringList(new[] {"Always", "Combo"}, 1));


            root.AddSubMenu(_mainMenu);
        }

        private static int GetBuffCount(Obj_AI_Base target)
        {
            int count = 0;

            foreach (var x in target.Buffs)
            {
                if (x.IsActive && x.Caster.IsEnemy && x.IsPositive) count++;
            }

            return count;
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (OC.Origin.Item("usecombo").GetValue<KeyBind>().Active ||
                _mainMenu.Item("cmode").GetValue<StringList>().SelectedIndex != 1)
            {
                UseItem("Mikaels", 3222, 600f);
                UseItem("Quicksilver", 3140);
                UseItem("Mercurial", 3139);
                UseItem("Dervish", 3137);
            }
        }

        private static void UseItem(string name, int itemId, float range = float.MaxValue)
        {
            if (!Items.CanUseItem(itemId) || !Items.HasItem(itemId))
                return;

            if (!_mainMenu.Item("use" + name).GetValue<bool>())
                return;

            var target = range > 5000 ? Me : OC.Friendly();
            if (_mainMenu.Item("cccon" + target.SkinName).GetValue<bool>())
            {
                if (target.Distance(Me.ServerPosition, true) <= range * range && target.IsValidState() &&
                    GetBuffCount(target) >= _mainMenu.Item(name + "Count").GetValue<Slider>().Value)
                {
                    var tHealthPercent = target.Health/target.MaxHealth*100;
                    var delay = _mainMenu.Item("cleansedelay").GetValue<Slider>().Value * 10;

                    foreach (var buff in GameBuff.CleanseBuffs)
                    {
                        var buffinst = target.Buffs;
                        if (buffinst.Any(aura => aura.Name.ToLower() == buff.BuffName ||
                                                 aura.Name.ToLower().Contains(buff.SpellName)))
                        {
                            if (!OC.Origin.Item("cure" + buff.BuffName).GetValue<bool>())
                            {
                                return;
                            }

                            Utility.DelayAction.Add(delay + buff.Delay, delegate
                            {
                                Items.UseItem(itemId, target);
                                OC.Logger(OC.LogType.Action,
                                    "Used " + name + " on " + target.SkinName + " (" + tHealthPercent + "%) for: " + buff.BuffName);
                            });
                        }
                    }

                    foreach (var b in target.Buffs)
                    {
                        var duration = Math.Ceiling(b.EndTime - b.StartTime);
                        if (OC.Origin.Item("slow").GetValue<bool>() && b.Type == BuffType.Slow &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (slow)");
                        }

                        if (OC.Origin.Item("stun").GetValue<bool>() && b.Type == BuffType.Stun &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (stun)");
                        }

                        if (OC.Origin.Item("charm").GetValue<bool>() && b.Type == BuffType.Charm &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (charm)");
                        }

                        if (OC.Origin.Item("taunt").GetValue<bool>() && b.Type == BuffType.Taunt &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (taunt)");
                        }

                        if (OC.Origin.Item("fear").GetValue<bool>() && b.Type == BuffType.Fear &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (fear)");
                        }

                        if (OC.Origin.Item("snare").GetValue<bool>() && b.Type == BuffType.Snare &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (snare)");
                        }

                        if (OC.Origin.Item("silence").GetValue<bool>() && b.Type == BuffType.Silence &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (silence)");
                        }

                        if (OC.Origin.Item("suppression").GetValue<bool>() && b.Type == BuffType.Suppression &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (suppression)");
                        }

                        if (OC.Origin.Item("polymorph").GetValue<bool>() && b.Type == BuffType.Polymorph &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (polymorph)");
                        }

                        if (OC.Origin.Item("blind").GetValue<bool>() && b.Type == BuffType.Blind &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (blind)");
                        }

                        if (OC.Origin.Item("poison").GetValue<bool>() && b.Type == BuffType.Poison &&
                            duration >= _mainMenu.Item(name + "Duration").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used  " + name + "  on " + target.SkinName + " (" + tHealthPercent + "%) (poison)");
                        }
                    }
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int ccvalue)
        {
            var menuName = new Menu(name, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + displayname)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "Min spells to use")).SetValue(new Slider(ccvalue, 1, 5));
            menuName.AddItem(new MenuItem(name + "Duration", "Buff duration to use")).SetValue(new Slider(2, 1, 5));
            _mainMenu.AddSubMenu(menuName);
        }
    }
}