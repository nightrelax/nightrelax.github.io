#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

#endregion

namespace Stack_Overflow.Champions
{
    internal class Lulu : Plugin
    {

        public Items.Item Dfg;
        public Spell E;
        public Spell Q;
        public Spell W;
        public Spell R;


        public Lulu()
        {
            Q = new Spell(SpellSlot.Q, 925);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 900);

            Q.SetSkillshot(0.25f, 60, 1450, false, SkillshotType.SkillshotLine);

            Dfg = new Items.Item(3128, 750);

            Game.OnGameUpdate += GameOnOnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += InterrupterOnOnPossibleToInterrupt;
            Drawing.OnDraw += DrawingOnOnDraw;

            PrintChat("Lulu Carregada.");
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            var drawQ = GetBool("drawQ");
            var drawW = GetBool("drawW");
            var drawE = GetBool("drawE");
            var drawR = GetBool("drawR");
            var p = Player.Position;

            if (drawQ)
                Utility.DrawCircle(p, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);

            if (drawW)
                Utility.DrawCircle(p, W.Range, W.IsReady() ? Color.Aqua : Color.Red);

            if (drawE)
                Utility.DrawCircle(p, E.Range, E.IsReady() ? Color.Aqua : Color.Red);

            if (drawR)
                Utility.DrawCircle(p, R.Range, R.IsReady() ? Color.Aqua : Color.Red);
        }

        private void InterrupterOnOnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!GetBool("interromperW") || spell.DangerLevel != InterruptableDangerLevel.High)
                return;

            W.Cast(unit, Packets);

            if (!GetBool("interromperR") || spell.DangerLevel != InterruptableDangerLevel.High || unit.Distance(ObjectManager.Player.Position) < R.Range )
                return;

            R.Cast(ObjectManager.Player, Packets);
        }

        private void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!GetBool("gapcloseW"))
                return;

            W.Cast(gapcloser.Sender, Packets);
        }

        private void GameOnOnGameUpdate(EventArgs args)
        {
            switch (OrbwalkerMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    autoEscudo();
                    break;

                case Orbwalking.OrbwalkingMode.Combo:
                    Combar();
                    autoUlt();
                    autoEscudo();
                    break;
            }
        }

        private void Combar()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            var useDfg = GetBool("useDFG");

            if (useDfg && Dfg.IsReady())
                Dfg.Cast(target);

            if (GetBool("comboE") && E.IsReady())
            {
                if (E.IsInRange(target.ServerPosition))
                {
                    E.Cast(target);
                }
                else if( inimigoProximo(ObjectManager.Player) != null )
                {
                    E.Cast(inimigoProximo(ObjectManager.Player));
                }
            }


            if (GetBool("comboQ") && Q.IsReady()) 
            {
                if (GetBool("comboE") && E.IsReady()) 
                { 
                    if (E.IsInRange(target.ServerPosition))
                    {
                        E.Cast(target);
                        Q.Cast(target.Position);
                    }
                    else if (inimigoProximo(ObjectManager.Player) != null)
                    {
                        E.Cast(inimigoProximo(ObjectManager.Player));
                        Q.Cast(target.Position);
                    }
                }
                else
                {
                    if (Q.IsInRange(target))
                    {
                        Q.Cast(target.Position);
                    }
                }
            }

            if (GetBool("comboW") && W.IsReady() && W.IsInRange(target))
            {
                 W.Cast(target, Packets);
            }

            if (GetBool("comboWy") && W.IsReady() && !W.IsInRange(target))
            {
                W.Cast(ObjectManager.Player);
            }

            if (GetBool("comboR") && R.IsReady() && ObjectManager.Player.Health <= ObjectManager.Player.MaxHealth * 0.35)
                R.Cast(ObjectManager.Player,Packets);
        }

        private void autoUlt()
        {
            var nearAlly =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(ObjectManager.Player.Position) <= R.Range)
                        .Where(x => x.Health <= x.MaxHealth * (GetValue<Slider>("autoRprocentagem").Value / 100));

            if (GetBool("autoR"))
            {
                if (ObjectManager.Player.Health <= ObjectManager.Player.MaxHealth * (GetValue<Slider>("autoRprocentagem").Value / 100))
                {
                    R.Cast(ObjectManager.Player);
                }
                else
                {
                    if (nearAlly != null && GetBool("autoRaliados"))
                    {
                        R.Cast((Obj_AI_Base)nearAlly);
                    }
                }
            }

        }

        private void autoEscudo()
        {
            var nearAlly =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(ObjectManager.Player.Position) <= R.Range)
                        .Where(x => x.Health <= x.MaxHealth * (GetValue<Slider>("autoEprocentagem").Value / 100));

            if (GetBool("autoE"))
            {
                if (ObjectManager.Player.Health <= ObjectManager.Player.MaxHealth * (GetValue<Slider>("autoEprocentagem").Value / 100))
                {
                    E.Cast(ObjectManager.Player);
                }
                else
                {
                    if (nearAlly != null && GetBool("autoEaliados"))
                    {
                        E.Cast((Obj_AI_Base)nearAlly);
                    }
                }
            }

        }

        private Obj_AI_Base inimigoProximo(Obj_AI_Hero target)
        {
            var nearEnemy =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(x => x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(ObjectManager.Player.Position) <= E.Range)
                        .Where(x => x.Distance(target.Position) <= Q.Range)
                        .FirstOrDefault(
                            x => ObjectManager.Player.GetSpellDamage(x, "E") < x.Health);
            return nearEnemy;
        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            if (GetBool("harassE") && E.IsReady())
            {
                if (E.IsInRange(target.ServerPosition))
                {
                    E.Cast(target);
                }
                else if (inimigoProximo(ObjectManager.Player) != null)
                {
                    E.Cast(inimigoProximo(ObjectManager.Player));
                }
            }


            if (GetBool("harassQ") && Q.IsReady())
            {
                if (GetBool("harassE") && E.IsReady())
                {
                    if (E.IsInRange(target.ServerPosition))
                    {
                        E.Cast(target);
                        Q.Cast(target.Position);
                    }
                    else if (inimigoProximo(ObjectManager.Player) != null)
                    {
                        E.Cast(inimigoProximo(ObjectManager.Player));
                        Q.Cast(target.Position);
                    }
                }
                else
                {
                    if (Q.IsInRange(target))
                    {
                        Q.Cast(target.Position);
                    }
                }
            }

            if (GetBool("harassW") && W.IsReady() && W.IsInRange(target))
            {
                W.Cast(target, Packets);
            }

            if (GetBool("harassWy") && W.IsReady() && !W.IsInRange(target))
            {
                W.Cast(ObjectManager.Player);
            }

        }

        public override float GetComboDamage(Obj_AI_Hero target)
        {
            double dmg = 0;

            if (Q.IsReady())
                dmg += Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1);

            if (W.IsReady())
                dmg += Player.GetSpellDamage(target, SpellSlot.W);

            if (E.IsReady())
            {
                dmg += Player.GetSpellDamage(target, SpellSlot.E);
                dmg += dmg*0.2;
            }

            if (R.IsReady())
            {
                dmg += Player.GetSpellDamage(target, SpellSlot.R);
                dmg += dmg * 0.2;
            }

            if (!Dfg.IsReady()) return (float) dmg;
            dmg += Player.GetItemDamage(target, Damage.DamageItems.Dfg);
            dmg += dmg*0.2;

            return (float) dmg;
        }

        public override void Combo(Menu config)
        {
            config.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            config.AddItem(new MenuItem("comboWy", "Use W Yourself").SetValue(true));
            config.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
            config.AddItem(new MenuItem("comboR", "Use R").SetValue(true));
        }

        public override void Harass(Menu config)
        {
            config.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("harassW", "Use W").SetValue(false));
            config.AddItem(new MenuItem("harassWy", "Use W Yourself").SetValue(true));
            config.AddItem(new MenuItem("harassE", "Use E").SetValue(false));
        }

        public override void ItemMenu(Menu config)
        {
            config.AddItem(new MenuItem("useDFG", "Use DFG").SetValue(true));
        }

        public override void Misc(Menu config)
        {
            config.AddItem(new MenuItem("autoR", "Auto R if low HP", true).SetValue(true));
            config.AddItem(new MenuItem("autoRaliados", "Auto R if low HP Allies", true).SetValue(true));
            config.AddItem(new MenuItem("autoRprocentagem", "% Health to use R").SetValue(new Slider(15, 1)));
            config.AddItem(new MenuItem("autoE", "Auto E if low HP", true).SetValue(true));
            config.AddItem(new MenuItem("autoEaliados", "Auto E if low HP Allies", true).SetValue(true));
            config.AddItem(new MenuItem("autoEprocentagem", "% Health to use E").SetValue(new Slider(15, 1)));
            config.AddItem(new MenuItem("gapcloseW", "W on Gapcloser").SetValue(true));
            config.AddItem(new MenuItem("interromperW", "W to Interrupt", true).SetValue(true));
            config.AddItem(new MenuItem("interromperR", "R to Interrupt", true).SetValue(true));
        }

        public override void Drawings(Menu config)
        {
            config.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            config.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            config.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            config.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));
        }
    }
}
