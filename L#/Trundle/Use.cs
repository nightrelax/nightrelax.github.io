using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Trundle
{
    class Use
    {
        public static void UseWCombo(Obj_AI_Hero target)
        {
            if (T.W.IsReady() && target.Distance(ObjectManager.Player) < T.W.Range)
            {
                T.W.Cast(target.Position);
            }
        }

        public static void UseECombo(Obj_AI_Hero target)
        {
            if (ObjectManager.Player.CountEnemiesInRange(2000f) > 2 && TMenu.Config.Item("manualE").GetValue<bool>())
            {
                Console.WriteLine("Returning");
                return;
            }
            if ((from spell in target.Spellbook.Spells from gapcloser in AntiGapcloser.Spells where spell.Name.ToLower() == gapcloser.SpellName && 
                     target.Spellbook.CanUseSpell(spell.Slot) != SpellState.Cooldown &&
                     TMenu.Config.Item("waitGap" + spell.Slot + target.ChampionName).GetValue<bool>()
                     select spell).Any())
            {
                return;
            }

            if (T.E.IsReady() && ObjectManager.Player.Distance(target) < T.E.Range)
            {
                var pred = T.E.GetPrediction(target);
                T.E.Cast(target.Position.Extend(pred.UnitPosition, T.E.Width / 2 + target.BoundingRadius));
            }
        }

        public static void UseRCombo(Obj_AI_Hero target)
        {
            if (T.R.IsReady() && target.Distance(ObjectManager.Player) < T.R.Range)
            {
                T.R.Cast(target);
            }
        }

        public static void UseRTanks()
        {
            if (!T.R.IsReady())
            {
                return;
            }
            try
            {
                var i = 0;
                var enemiesInRange = ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy && enemy.Distance(ObjectManager.Player) < T.R.Range && enemy.IsValidTarget()).ToList();
                if (!enemiesInRange.Any())
                {
                    return;
                }
                var tank = enemiesInRange.First();
                for (i = 0; i < enemiesInRange.Count(); i++)
                {
                    if (enemiesInRange[i].Armor > tank.Armor)
                        tank = enemiesInRange[i];
                }
                T.R.Cast(tank);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void UseIgnite(Obj_AI_Hero target)
        {
            if (T.IgniteSlot != SpellSlot.Unknown &&
                T.Player.Spellbook.CanUseSpell(T.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= T.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                {
                    T.Player.Spellbook.CastSpell(T.IgniteSlot, target);
                }
            }
        }

        public static void UseHydra(Obj_AI_Hero target)
        {
            if (Items.CanUseItem(3074) && target.Distance(T.Player) < 420)
            {
                Items.UseItem(3074);
            }
            if (Items.CanUseItem(3077) && target.Distance(T.Player) < 420)
            {
                Items.UseItem(3077);
            }
        }

        public static void UseHydraLc()
        {
            var minions = MinionManager.GetMinions(T.Player.Position, 420).ToArray();
            if (minions.Length > 1)
            {
                if (Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }
                if (Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                }
            }
        }

        public static void UseComboItems(Obj_AI_Hero target)
        {
            //BOTRK and Cutlass
            if ((!T.E.IsReady() &&
                 target.Distance(T.Player) > T.Player.AttackRange + target.BoundingRadius) ||
                T.Player.HealthPercentage() < 40)
            {
                if (Items.CanUseItem(3153))
                {
                    Items.UseItem(3153, target);
                }
                if (Items.CanUseItem(3144))
                {
                    Items.UseItem(3144, target);
                }
            }
            //Ghostblade
            if (Items.CanUseItem(3142))
            {
                Items.UseItem(3142);
            }

            //Hydra and Tiamat
            if (Items.CanUseItem(3074) && target.Distance(T.Player) < 420)
            {
                Items.UseItem(3074);
            }
            if (Items.CanUseItem(3077) && target.Distance(T.Player) < 420)
            {
                Items.UseItem(3077);
            }
        }

        public static void UseSmiteOnChamp(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(T.E.Range) && T.smiteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell((T.smiteSlot)) == SpellState.Ready &&
                (T.GetSmiteType() == "s5_summonersmiteplayerganker" ||
                 T.GetSmiteType() == "s5_summonersmiteduel"))
            {
                ObjectManager.Player.Spellbook.CastSpell(T.smiteSlot, target);
            }
        }
    }

}
