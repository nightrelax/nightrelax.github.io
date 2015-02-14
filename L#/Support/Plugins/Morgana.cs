#region LICENSE

// Copyright 2014-2015 Support
// Morgana.cs is part of Support.
// 
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.
// 
// Filename: Support/Support/Morgana.cs
// Created:  01/10/2014
// Date:     24/01/2015/13:14
// Author:   h3h3

#endregion

namespace Support.Plugins
{
    #region

    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Support.Util;
    using ActiveGapcloser = Support.Util.ActiveGapcloser;

    #endregion

    #region

    #endregion

    public class Morgana : PluginBase
    {
        public Morgana()
        {
            Q = new Spell(SpellSlot.Q, 1175);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 80f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.28f, 175f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            try
            {
                if (ComboMode)
                {
                    if (Q.CastCheck(Target, "ComboQ") && Q.CastWithHitChance(Target, "ComboQHC"))
                    {
                        return;
                    }

                    if (W.CastCheck(Target, "ComboW"))
                    {
                        if (
                            HeroManager.Enemies
                                .Where(hero => (hero.IsValidTarget(W.Range) && hero.IsMovementImpaired()))
                                .Any(enemy => W.Cast(enemy.Position)))
                        {
                            return;
                        }

                        if (
                            HeroManager.Enemies
                                .Where(hero => hero.IsValidTarget(W.Range))
                                .Any(enemy => W.CastIfWillHit(enemy, 1)))
                        {
                            return;
                        }
                    }

                    if (R.CastCheck(Target, "ComboR") &&
                        Helpers.EnemyInRange(ConfigValue<Slider>("ComboCountR").Value, R.Range))
                    {
                        R.Cast();
                    }
                    return;
                }

                if (HarassMode)
                {
                    if (Q.CastCheck(Target, "HarassQ") && Q.CastWithHitChance(Target, "HarassQHC"))
                    {
                        return;
                    }

                    if (W.CastCheck(Target, "HarassW"))
                    {
                        if (
                            HeroManager.Enemies
                                .Where(hero => (hero.IsValidTarget(W.Range) && hero.IsMovementImpaired()))
                                .Any(enemy => W.Cast(enemy.Position)))
                        {
                            return;
                        }

                        if (
                            HeroManager.Enemies
                                .Where(hero => hero.IsValidTarget(W.Range))
                                .Any(enemy => W.CastIfWillHit(enemy, 1))) {}
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(gapcloser.Sender, "GapcloserQ"))
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        public override void ComboMenu(Menu config)
        {
            var comboQ = config.AddSubMenu(new Menu("Q Settings", "Q"));
            comboQ.AddBool("ComboQ", "Use Q", true);
            comboQ.AddHitChance("ComboQHC", "Min HitChance", HitChance.Medium);

            var comboW = config.AddSubMenu(new Menu("W Settings", "W"));
            comboW.AddBool("ComboW", "Use W", true);

            var comboE = config.AddSubMenu(new Menu("E Settings", "E"));
            comboE.AddBool("ComboE", "Use E", true);

            var comboR = config.AddSubMenu(new Menu("R Settings", "R"));
            comboR.AddBool("ComboR", "Use R", true);
            comboR.AddSlider("ComboCountR", "Targets in range to Ult", 2, 1, 5);
        }

        public override void HarassMenu(Menu config)
        {
            var harassQ = config.AddSubMenu(new Menu("Q Settings", "Q"));
            harassQ.AddBool("HarassQ", "Use Q", true);
            harassQ.AddHitChance("HarassQHC", "Min HitChance", HitChance.High);

            var harassW = config.AddSubMenu(new Menu("W Settings", "W"));
            harassW.AddBool("HarassW", "Use W", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserQ", "Use Q to Interrupt Gapcloser", true);
        }
    }
}