#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using Stack_Overflow.Utilitarios;

#endregion

namespace Stack_Overflow.Champions
{
    internal class Galio : Plugin
    {

        public Items.Item Dfg;
        public Spell Q;
        public Spell W;
        public Spell E;
        public Spell R;

        private bool ultado = false;

        public Galio()
        {
            Q = new Spell(SpellSlot.Q, 940);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 1180);
            R = new Spell(SpellSlot.R, 570); //Decreased range on purpose

            Q.SetSkillshot(0.25f, 150, 1250, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 90, 1250, false, SkillshotType.SkillshotLine);

            Dfg = new Items.Item(3128, 750);

            Game.OnGameUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            Interrupter.OnPossibleToInterrupt += InterrupterOnOnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;

            PrintChat("Galio Carregado.");

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

        private void GameOnOnGameUpdate(EventArgs args)
        {
            if (ultado)
            {
                //Orbwalker
            }
            switch (OrbwalkerMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.Combo:
                    Combar();
                    break;
            }
        }

        private void Combar()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);

            var useDfg = GetBool("useDFG");

            if (GetBool("comboE") && E.IsReady() && !ultado)
            {
                E.CastIfHitchanceEquals(target, HitChance.High, Packets);
            }

            if (useDfg && Dfg.IsReady() && !ultado)
                Dfg.Cast(target);

            if (GetBool("comboQ") && Q.IsReady() && ObjectManager.Player.Distance(target.Position) < Q.Range && !ultado)
            {
                Q.CastIfHitchanceEquals(target, HitChance.High, Packets);
            }

            if (GetBool("comboW") && W.IsReady() && !ultado)
            {
                W.Cast(ObjectManager.Player);
            }

            if (GetBool("comboR") && R.IsReady())
            {
                R.CastIfWillHit(target, GetValue<Slider>("minR").Value, Packets);
                ultado = true;
                Utility.DelayAction.Add(2000, () => ultado = false);
            }

        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            if (GetBool("harassE") && E.IsReady())
            {
                E.CastIfHitchanceEquals(target, HitChance.High, Packets);
            }

            if (GetBool("harassQ") && Q.IsReady() && ObjectManager.Player.Distance(target.Position) < Q.Range)
            {
                Q.CastIfHitchanceEquals(target, HitChance.High, Packets);
            }

            if (GetBool("harassW") && W.IsReady())
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
                dmg += dmg * 0.2;
            }

            if (R.IsReady())
            {
                dmg += Player.GetSpellDamage(target, SpellSlot.R);
                dmg += dmg * 0.2;
            }

            if (!Dfg.IsReady()) return (float) dmg;
            dmg += Player.GetItemDamage(target, Damage.DamageItems.Dfg);
            dmg += dmg*0.2;

            return (float)dmg;
        }

        private void InterrupterOnOnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!GetBool("interromperR") || spell.DangerLevel != InterruptableDangerLevel.High || unit.Distance(ObjectManager.Player.Position) < R.Range)
                return;

            R.Cast(Packets);
        }

        private void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!GetBool("gapcloser"))
                return;

            if (W.IsReady() && GetBool("gapcloserW"))
                W.Cast(ObjectManager.Player, Packets);

            if (Q.IsReady() && GetBool("gapcloserQ"))
                Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.Medium, Packets);

            if (E.IsReady() && GetBool("gapcloserE"))
                E.Cast(Game.CursorPos);
        }


        public override void Combo(Menu config)
        {
            config.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            config.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
            config.AddItem(new MenuItem("comboR", "Use R").SetValue(true));
        }

        public override void Harass(Menu config)
        {
            config.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("harassW", "Use W").SetValue(false));
            config.AddItem(new MenuItem("harassE", "Use E").SetValue(false));
        }

        public override void ItemMenu(Menu config)
        {
            config.AddItem(new MenuItem("useDFG", "Use DFG").SetValue(true));
        }

        public override void Misc(Menu config)
        {
            config.AddItem(new MenuItem("minR", "Use R if will hit").SetValue(new Slider(1, 1, 5)));
            config.AddItem(new MenuItem("interruptR", "Use R to interrupt").SetValue(true));
            config.AddItem(new MenuItem("gapcloser", "Anti Gap Closer").SetValue(true));
            config.AddItem(new MenuItem("gapcloserQ", "Anti Gap Closer with Q").SetValue(true));
            config.AddItem(new MenuItem("gapcloserW", "Anti Gap Closer with W").SetValue(true));
            config.AddItem(new MenuItem("gapcloserE", "Anti Gap Closer with E").SetValue(true));
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