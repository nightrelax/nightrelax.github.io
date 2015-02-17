﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using System.Threading.Tasks;
using Version = System.Version;


namespace SigmaSeries.Plugins
{
    public class Singed : PluginBase
    {
        public Singed()
            : base(new Version(0, 1, 1))
        {

            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 1175);
            E = new Spell(SpellSlot.E, 125);
            R = new Spell(SpellSlot.R, 0);

            useQAgain = true;

            W.SetSkillshot(0.5f, 350, 700, false, SkillshotType.SkillshotCircle);
            checkTime = Environment.TickCount;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
        }

        public static int checkTime = 0;

        void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Q.Instance.Name == args.SData.Name)
            {
                    checkTime = Environment.TickCount + 1000;
            }
        }

        public bool useQAgain;

        public override void ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            config.AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            config.AddItem(new MenuItem("exploit", "Exploit Enabled [RISKY]").SetValue(false));
            config.AddItem(new MenuItem("delayms", "Delay (MS)").SetValue(new Slider(150, 0, 1000)));

        }

        public override void HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("UseWHarass", "Use W").SetValue(true));
            config.AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
        }

        public override void FarmMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQWC", "Use Q WC").SetValue(true));
            config.AddItem(new MenuItem("useEFarm", "E").SetValue(new StringList(new[] { "Freeze", "WaveClear", "Both", "None" }, 2)));
            config.AddItem(new MenuItem("JungleActive", "JungleActive!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            config.AddItem(new MenuItem("UseQJung", "Use Q").SetValue(true));
        }

        public override void BonusMenu(Menu config)
        {
            config.AddItem(new MenuItem("packetCast", "Packet Cast").SetValue(true));
        }

        public static bool hasQ()
        {
            return ObjectManager.Player.Buffs.Any(a => a.DisplayName == "Poison Trail");
        }

        public static bool waitforQ = false;

        public override void OnUpdate(EventArgs args)
        {
            if (ComboActive)
            {
                var useQ = Config.Item("UseQCombo").GetValue<bool>();
                var useW = Config.Item("UseWCombo").GetValue<bool>();
                var useE = Config.Item("UseECombo").GetValue<bool>();
                var exploit = Config.Item("exploit").GetValue<bool>();
                var delay = Config.Item("delayms").GetValue<Slider>().Value;
                var eTarget = TargetSelector.GetTarget(500f, TargetSelector.DamageType.Magical);

                if (eTarget == null && hasQ() && !waitforQ)
                {
                    Q.Cast();
                    waitforQ = true;
                    Utility.DelayAction.Add(1000, () => waitforQ = false);
                }

                if (eTarget != null)
                {
                    if (Q.IsReady() && useQ)
                    {
                        if (!exploit && checkTime <= Environment.TickCount)
                        {
                            if (!hasQ() && !waitforQ)
                            {
                                Q.CastOnUnit(Player);
                                waitforQ = true;
                                Utility.DelayAction.Add(1000, () => waitforQ = false);
                            }
                        }
                        if (exploit)
                        {
                            if (hasQ())
                            {
                                Q.CastOnUnit(Player);
                            }
                            if (hasQ() == false && useQAgain)
                            {
                                Q.CastOnUnit(Player);
                                useQAgain = false;
                                Utility.DelayAction.Add(delay, () => useQAgain = true);
                            }
                        }
                    }
                    if (hasQ())
                    {
                        if (eTarget.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)) == false && useW)
                        {
                            W.Cast(eTarget);
                        }
                        if (eTarget.IsValidTarget(E.Range) && useE)
                        {
                            E.CastOnUnit(eTarget);
                        }
                    }
                }
            }

            if (HarassActive)
            {
                var useQ = Config.Item("UseQHarass").GetValue<bool>();
                var useW = Config.Item("UseWHarass").GetValue<bool>();
                var useE = Config.Item("UseEHarass").GetValue<bool>();
                var exploit = Config.Item("exploit").GetValue<bool>();
                var delay = Config.Item("delayms").GetValue<Slider>().Value;
                var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (eTarget != null)
                {
                    if (Q.IsReady() && useQ)
                    {
                        if (!exploit && checkTime >= Environment.TickCount)
                        {
                            if (!hasQ() && useQAgain)
                            {
                                Q.CastOnUnit(Player);
                            }
                        }
                        if (exploit)
                        {
                            if (hasQ())
                            {
                                Q.CastOnUnit(Player);
                            }
                            if (hasQ() == false && useQAgain)
                            {
                                Q.CastOnUnit(Player);
                                useQAgain = false;
                                Utility.DelayAction.Add(delay, () => useQAgain = true);
                            }
                        }
                    }
                    if (hasQ())
                    {
                        if (eTarget.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)) == false && useW)
                        {
                            W.Cast(eTarget);
                        }
                        if (eTarget.IsValidTarget(E.Range) && useE)
                        {
                            E.CastOnUnit(eTarget);
                        }
                    }
                }
            }

            if (Config.Item("JungleActive").GetValue<KeyBind>().Active)
            {
                Jungle();
            }

            if (WaveClearActive)
            {
                WaveClear();
            }
            if (FreezeActive)
            {
                Freeze();
            }
        }

        private void Freeze()
        {
            var useE = Config.Item("useEFarm").GetValue<StringList>().SelectedIndex == 0 || Config.Item("useEFarm").GetValue<StringList>().SelectedIndex == 2;
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, 400, MinionTypes.All);
            if (minions.Count > 1)
            {
                foreach (var minion in minions)
                {
                    var predHP = HealthPrediction.GetHealthPrediction(minion, (int)E.Delay);

                    if (E.GetDamage(minion) > minion.Health && predHP > 0 && minion.IsValidTarget(E.Range) && useE)
                    {
                        E.CastOnUnit(minion, true);
                    }
                }
            }
        }

        private void Jungle()
        {
            var useQ = Config.Item("UseQJung").GetValue<bool>();

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, 400, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (JungleMinions.Count > 0)
            {
                foreach (var minion in JungleMinions)
                {
                    if (Q.IsReady() && useQ)
                    {
                        if (!hasQ())
                        {
                            Q.Cast(true);
                        }
                    }
                }
            }
            else
            {
                if (Q.IsReady() && useQ)
                {
                    if (hasQ())
                    {
                        Q.Cast(true);
                    }
                }
            }
        }
        private void WaveClear()
        {
            var useQ = Config.Item("UseQWC").GetValue<bool>();
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, 400, MinionTypes.All);
            if (minions.Count > 0)
            {
                if (Q.IsReady() && useQ)
                {
                    if (hasQ() == false)
                    {
                        Q.Cast(true);
                    }
                }

            }
            else
            {
                if (Q.IsReady() && useQ)
                {
                    if (hasQ())
                    {
                        Q.Cast(true);
                    }
                }
            }
        }
    }
}