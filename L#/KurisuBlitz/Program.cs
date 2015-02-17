using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace KurisuBlitz
{
    //   _____       _    _____           _ 
    //  |   __|___ _| |  |  |  |___ ___ _| |
    //  |  |  | . | . |  |     | .'|   | . |
    //  |_____|___|___|  |__|__|__,|_|_|___|
    //  Copyright © Kurisu Solutions 2015
      
    internal class Program
    {
        private static Spell q;
        private static Spell e;
        private static Spell r;

        private static Menu _menu;
        private static Obj_AI_Hero _target;
        private static Orbwalking.Orbwalker _orbwalker;
        private static  Obj_AI_Hero _player = ObjectManager.Player;

        static void Main(string[] args)
        {
            Console.WriteLine("Blitzcrank injected...");
            CustomEvents.Game.OnGameLoad += BlitzOnLoad;
        }

        private static void BlitzOnLoad(EventArgs args)
        {
            if (_player.ChampionName != "Blitzcrank")
            {
                return;
            }

            // Set spells      
            q = new Spell(SpellSlot.Q, 1050f);
            q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);

            e = new Spell(SpellSlot.E, 150f);
            r = new Spell(SpellSlot.R, 550f);

            // Load Menu
            BlitzMenu();

            // Load Drawings
            Drawing.OnDraw += BlitzOnDraw;

            // OnUpdate
            Game.OnGameUpdate += BlitzOnUpdate;

            // Interrupter
            Interrupter.OnPossibleToInterrupt += BlitzOnInterrupt;

        }

        private static void BlitzOnInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (_menu.Item("interrupt").GetValue<bool>())
            {
                var prediction = q.GetPrediction(unit);
                if (prediction.Hitchance >= HitChance.Low)
                {
                    q.Cast(prediction.CastPosition);
                }

                else if (unit.Distance(_player.Position) < r.Range)
                {
                    r.Cast();
                }
            }
        }

        private static void BlitzOnDraw(EventArgs args)
        {
            if (!_player.IsDead)
            {
                var rcircle = _menu.SubMenu("drawings").Item("drawR").GetValue<Circle>();
                var qcircle = _menu.SubMenu("drawings").Item("drawQ").GetValue<Circle>();

                if (qcircle.Active)
                    Render.Circle.DrawCircle(_player.Position, q.Range, qcircle.Color);

                if (rcircle.Active)
                    Render.Circle.DrawCircle(_player.Position, r.Range, qcircle.Color);
            }

            if (_target.IsValidTarget(q.Range * 2))
                Render.Circle.DrawCircle(_target.Position, _target.BoundingRadius, Color.Red, 10);
        }


        private static void BlitzOnUpdate(EventArgs args)
        {
            _target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

            // do KS
            GodKS(q);
            GodKS(r);
            GodKS(e);

            var actualHealthSetting = _menu.Item("hneeded").GetValue<Slider>().Value;
            var actualHealthPercent = (int)(_player.Health / _player.MaxHealth * 100);

            if (actualHealthPercent < actualHealthSetting)
            {
                return;
            }

            // use the god hand
            GodHand(_target);

            var powerfistTarget = 
                ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsValidTarget(_player.AttackRange));
            if (powerfistTarget.Distance(_player.ServerPosition) <= _player.AttackRange)
            {
                if (_menu.Item("useE").GetValue<bool>() && !q.IsReady())
                    e.CastOnUnit(_player);
            }


            var itarget =
                ObjectManager.Get<Obj_AI_Hero>()
                    .FirstOrDefault(h => h.IsEnemy && h.Distance(_player.ServerPosition, true) <= q.RangeSqr);

            if (itarget.IsValidTarget() &&
                _menu.Item("dograb" + itarget.SkinName).GetValue<StringList>().SelectedIndex == 2)
            {
                if (itarget.Distance(_player.ServerPosition) > _menu.Item("dneeded").GetValue<Slider>().Value)
                {
                    var prediction = q.GetPrediction(itarget);
                    if (prediction.Hitchance == HitChance.Immobile &&
                        _menu.Item("immobile").GetValue<bool>())
                    {
                        q.Cast(prediction.CastPosition);
                    }

                    else if (prediction.Hitchance == HitChance.Dashing &&
                        _menu.Item("dashing").GetValue<bool>())
                    {
                        q.Cast(prediction.CastPosition);
                    }
                }        
            }
        }

        private static void GodHand(Obj_AI_Base target)
        {
            if (!target.IsValidTarget() || !q.IsReady())
            {
                return;
            }

            if (!_menu.Item("combokey").GetValue<KeyBind>().Active)
            {
                return;
            }

            if ((target.Distance(_player.Position) > _menu.Item("dneeded").GetValue<Slider>().Value) &&
                (target.Distance(_player.Position) < _menu.Item("dneeded2").GetValue<Slider>().Value))
            {
                var prediction = q.GetPrediction(target);
                if (_menu.Item("dograb" + target.SkinName).GetValue<StringList>().SelectedIndex != 0)
                {
                    if (prediction.Hitchance >= HitChance.High &&
                        _menu.Item("hitchance").GetValue<StringList>().SelectedIndex == 2)
                    {
                        q.Cast(prediction.CastPosition);
                    }

                    else if (prediction.Hitchance >= HitChance.Medium &&
                             _menu.Item("hitchance").GetValue<StringList>().SelectedIndex == 1)
                    {
                        q.Cast(prediction.CastPosition);
                    }

                    else if (prediction.Hitchance >= HitChance.Low &&
                             _menu.Item("hitchance").GetValue<StringList>().SelectedIndex == 0)
                    {
                        q.Cast(prediction.CastPosition);
                    }
                }
            }
        }




        private static void GodKS(Spell spell)
        {
            if (_menu.Item("killsteal" + spell.Slot).GetValue<bool>() && spell.IsReady())
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(e => e.IsValidTarget(spell.Range)))
                {
                    var ksDmg = _player.GetSpellDamage(enemy, spell.Slot);
                    if (ksDmg > enemy.Health)
                    {
                        if (spell.Slot.ToString() == "Q")
                        {
                            var po = spell.GetPrediction(enemy);
                            if (po.Hitchance >= HitChance.Medium)
                                spell.Cast(po.CastPosition);
                        }

                        else
                        {
                            spell.CastOnUnit(_player);
                        }
                    }
                }
            }
        }

        private static void BlitzMenu()
        {
            _menu = new Menu("Kurisu: Blitz", "blitz", true);

            var blitzOrb = new Menu("Blitz: Orbwalker", "orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(blitzOrb);
            _menu.AddSubMenu(blitzOrb);

            var blitzTS = new Menu("Blitz: Selector", "tselect");
            TargetSelector.AddToMenu(blitzTS);
            _menu.AddSubMenu(blitzTS);

            var menuD = new Menu("Blitz: Drawings", "drawings");
            menuD.AddItem(new MenuItem("drawQ", "Draw Q")).SetValue(new Circle(true, Color.FromArgb(150, Color.White)));
            menuD.AddItem(new MenuItem("drawR", "Draw R")).SetValue(new Circle(true, Color.FromArgb(150, Color.White)));
            _menu.AddSubMenu(menuD);

            var menuG = new Menu("Blitz: GodHand", "autograb");
            menuG.AddItem(new MenuItem("hitchance", "Hitchance"))
                .SetValue(new StringList(new[] { "Low", "Medium", "High" }, 2));
            menuG.AddItem(new MenuItem("dneeded", "Mininum distance to Q")).SetValue(new Slider(255, 0, (int)q.Range));
            menuG.AddItem(new MenuItem("dneeded2", "Maximum distance to Q")).SetValue(new Slider((int)q.Range, 0, (int)q.Range));
            menuG.AddItem(new MenuItem("dashing", "Auto Q dashing enemies")).SetValue(true);
            menuG.AddItem(new MenuItem("immobile", "Auto Q immobile enemies")).SetValue(true);
            menuG.AddItem(new MenuItem("hneeded", "Dont grab below health %")).SetValue(new Slider(0));
            menuG.AddItem(new MenuItem("sep", ""));

            foreach (
                var e in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            e =>
                                e.Team != _player.Team))
            {
                menuG.AddItem(new MenuItem("dograb" + e.SkinName, e.SkinName))
                    .SetValue(new StringList(new[] { "Dont Grab ", "Normal Grab ", "Auto Grab " }, 1));
            }

            _menu.AddSubMenu(menuG);

            var menuK = new Menu("Blitz: Killsteal", "blitzks");
            menuK.AddItem(new MenuItem("killstealQ", "Use Q")).SetValue(false);
            menuK.AddItem(new MenuItem("killstealE", "Use E")).SetValue(false);
            menuK.AddItem(new MenuItem("killstealR", "Use R")).SetValue(false);
            _menu.AddSubMenu(menuK);

            _menu.AddItem(new MenuItem("interrupt", "Interrupt spells")).SetValue(true);
            _menu.AddItem(new MenuItem("useE", "Powerfist after grab")).SetValue(true);
            _menu.AddItem(new MenuItem("combokey", "Combo Key")).SetValue(new KeyBind(32, KeyBindType.Press));
            _menu.AddToMainMenu();
        }
    }
}
