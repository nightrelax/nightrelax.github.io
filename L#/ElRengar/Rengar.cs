using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace ElRengar
{
    enum Spells
    {
        Q, W, E, R
    }

    /// <summary>
    ///     Handle all stuff what is going on with Rengar.
    /// </summary>
    internal class Rengar
    {

        private static String hero = "Rengar";
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Orbwalking.Orbwalker _orbwalker;

        private static SpellSlot Ignite;
        private static SpellSlot smiteSlot;
        private static bool checkSmite = false;
        public static Obj_AI_Base minionerimo;


        private const float Jqueryluckynumber = 400f;
        private static Items.Item Youmuu, Cutlass, Blade, Tiamat, Hydra;

        private static readonly string[] epics =
        {
            "SRU_Baron", "SRU_Dragon"
        };
        private static readonly string[] buffs =
        {
            "SRU_Red", "SRU_Blue"
        };
        private static readonly string[] buffandepics =
        {
            "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron"
        };

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>()
        {
            { Spells.Q, new Spell(SpellSlot.Q, 0)},
            { Spells.W, new Spell(SpellSlot.W, 500)},
            { Spells.E, new Spell(SpellSlot.E, 1000)},
            { Spells.R, new Spell(SpellSlot.R, 2000)}
        };

        #region Gameloaded 

        public static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.BaseSkinName != "Rengar")
                return;

            Youmuu = new Items.Item(3142, 0f);
            Cutlass = new Items.Item(3144, 450f);
            Blade = new Items.Item(3153, 450f);

            Tiamat = new Items.Item(3077, 400f);
            Hydra = new Items.Item(3074, 400f);
            Ignite = Player.GetSpellSlot("summonerdot");

            Notifications.AddNotification("ElRengar by jQuery v1.0.2.0", 10000);
            spells[Spells.E].SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);

            ElRengarMenu.Initialize();
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Orbwalking.AfterAttack += AfterAttack;
            new AssassinManager();
        }

        #endregion


        #region OnGameUpdate

        private static void OnGameUpdate(EventArgs args)
        {

            smiteSlot = Player.GetSpellSlot(smitetype());

            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                break;
            }
            SelfHealing();


            if (ElRengarMenu._menu.Item("ElRengar.smiteEnabled").GetValue<KeyBind>().Active) smiter();
            if (paramBool("ElRengar.smiteSave")) SaveMe();
        }

        #endregion

        #region itemusage

        private static void fightItems()
        {
            var target = GetEnemy(spells[Spells.W].Range);

            var TiamatItem = ElRengarMenu._menu.Item("ElRengar.Combo.Tiamat").GetValue<bool>();
            var HydraItem = ElRengarMenu._menu.Item("ElRengar.Combo.Hydra").GetValue<bool>();

            if (Items.CanUseItem(3074) && HydraItem && Player.Distance(target) <= Jqueryluckynumber)
                Items.UseItem(3074);

            if (Items.CanUseItem(3077) && TiamatItem && Player.Distance(target) <= Jqueryluckynumber)
                Items.UseItem(3077);
        }

        #endregion

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (unit.IsMe && spells[Spells.Q].IsReady() && target is Obj_AI_Hero)
                    {
                        /*Q.Cast();
                        fightItems();*/
                        Orbwalking.ResetAutoAttackTimer();
                    }
               break;
            }
        }

        private static void Combo()
        {
            var target = GetEnemy(spells[Spells.W].Range);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var qCombo = ElRengarMenu._menu.Item("ElRengar.Combo.Q").GetValue<bool>();
            var wCombo = ElRengarMenu._menu.Item("ElRengar.Combo.W").GetValue<bool>();
            var eCombo = ElRengarMenu._menu.Item("ElRengar.Combo.E").GetValue<bool>();
            var eComboOOR = ElRengarMenu._menu.Item("ElRengar.Combo.EOOR").GetValue<bool>();
            var cutlassItem = ElRengarMenu._menu.Item("ElRengar.Combo.Cutlass").GetValue<bool>();
            var prioCombo = ElRengarMenu._menu.Item("ElRengar.Combo.Prio").GetValue<StringList>();
            var useYoumuu = ElRengarMenu._menu.Item("ElRengar.Combo.Youmuu").GetValue<bool>();
            var bladeItem = ElRengarMenu._menu.Item("ElRengar.Combo.Blade").GetValue<bool>();

            fightItems();

            if (Player.Mana <= 4)
            {
                if (qCombo && Player.Distance(target) <= Player.AttackRange && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast();
                }

                if (wCombo && spells[Spells.W].IsReady() && Player.Distance(target) <= spells[Spells.W].Range)
                {
                    spells[Spells.W].Cast();
                }

                if (eCombo && spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(target);
                }
            }

            if (Player.Mana == 5)
            {
                if (spells[Spells.Q].IsReady() && qCombo &&
                    prioCombo.SelectedIndex == 0 && Player.Distance(target) <= Player.AttackRange)
                {
                    spells[Spells.Q].Cast();
                }

                if (spells[Spells.W].IsReady() && wCombo &&
                    prioCombo.SelectedIndex == 1 && Player.Distance(target) <= spells[Spells.W].Range
                    )
                {
                    spells[Spells.W].Cast();
                }

                if (eCombo &&
                    prioCombo.SelectedIndex == 2 && Player.Distance(target) <= spells[Spells.E].Range)
                {
                    spells[Spells.E].Cast(target);
                }

                if (eComboOOR && Player.Distance(target) > Player.AttackRange + 100) 
                {
                    spells[Spells.E].Cast(target);
                }  
            }

            if (cutlassItem && Player.Distance(target) <= 450 && Cutlass.IsReady())
            {
                Cutlass.Cast(target);
            }

            if (bladeItem && Player.Distance(target) <= 450 && Blade.IsReady())
            {
                Blade.Cast(target);
            }

            if (useYoumuu && Player.Distance(target) <= Jqueryluckynumber && Youmuu.IsReady())
            {
                Youmuu.Cast(Player);
            }
            
            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health &&
                ElRengarMenu._menu.Item("ElRengar.Combo.Ignite").GetValue<bool>())
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        #region harass

        private static void Harass()
        {
            var target = GetEnemy(spells[Spells.R].Range);
            if (target == null || !target.IsValid)
                return;

            var qHarass = ElRengarMenu._menu.Item("ElRengar.Harass.Q").GetValue<bool>();
            var wHarass = ElRengarMenu._menu.Item("ElRengar.Harass.W").GetValue<bool>();
            var eHarass = ElRengarMenu._menu.Item("ElRengar.Harass.E").GetValue<bool>();
            var prioHarass = ElRengarMenu._menu.Item("ElRengar.Harass.Prio").GetValue<StringList>();

            if (Player.Mana <= 4)
            {
                if (qHarass &&
                    Player.Distance(target) <= 175f && Player.Distance(target) <= Player.AttackRange && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast();
                }

                if (wHarass &&
                    Player.Distance(target) <= spells[Spells.W].Range && spells[Spells.W].IsReady())
                {
                    spells[Spells.W].Cast();
                }

                if (eHarass && spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(target);
                }
            }

            if (Player.Mana >= 5)
            {

                if (qHarass &&
                    Player.Distance(target) <= Player.AttackRange && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast();
                }

                if (wHarass &&
                    prioHarass.SelectedIndex == 0 && Player.Distance(target) <= spells[Spells.W].Range && spells[Spells.W].IsReady())
                {
                    spells[Spells.W].Cast();
                }

                if (eHarass &&
                    prioHarass.SelectedIndex == 1 && Player.Distance(target) <= spells[Spells.E].Range)
                {
                    //Spells.E.Cast(target);
                    spells[Spells.E].CastIfHitchanceEquals(target, HitChance.Medium);
                }
                Console.WriteLine("Empowered Harass");
            }
            
        }
        #endregion

        #region jungle

        private static void JungleClear()
        {
            var qWaveClear = ElRengarMenu._menu.Item("ElRengar.Clear.Q").GetValue<bool>();
            var wWaveClear = ElRengarMenu._menu.Item("ElRengar.Clear.W").GetValue<bool>();
            var eWaveClear = ElRengarMenu._menu.Item("ElRengar.Clear.E").GetValue<bool>();
            var hydraClear = ElRengarMenu._menu.Item("ElRengar.Clear.Hydra").GetValue<bool>();
            var tiamatClear = ElRengarMenu._menu.Item("ElRengar.Clear.Tiamat").GetValue<bool>();
            var saveClear = ElRengarMenu._menu.Item("ElRengar.Clear.Save").GetValue<bool>();
            var prioClear = ElRengarMenu._menu.Item("ElRengar.Clear.Prio").GetValue<StringList>();

            var Target = MinionManager.GetMinions(
                Player.Position, 700, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (Player.Mana <= 4)
            {
                if (qWaveClear && spells[Spells.Q].IsReady() &&
                    Target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                {
                    spells[Spells.Q].Cast();
                }
                if (wWaveClear && spells[Spells.W].IsReady() &&
                    Target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)) &&
                    Player.Distance(Target) <= spells[Spells.W].Range)
                {
                    spells[Spells.W].Cast();
                }
                if (eWaveClear && spells[Spells.E].IsReady() && Target.IsValidTarget(spells[Spells.E].Range))
                {
                    spells[Spells.E].Cast(Target);
                }
            }

            if (Player.Mana == 5)
            {
                if (saveClear)
                    return;

                if (prioClear.SelectedIndex == 0 && qWaveClear && spells[Spells.Q].IsReady() &&
                   Target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                {
                    spells[Spells.Q].Cast();
                }
                if (prioClear.SelectedIndex == 1 && wWaveClear && spells[Spells.W].IsReady() &&
                    Target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)) &&
                    Player.Distance(Target) <= spells[Spells.W].Range)
                {
                    spells[Spells.W].Cast();
                }
                if (prioClear.SelectedIndex == 2 && eWaveClear && spells[Spells.E].IsReady() && Target.IsValidTarget(spells[Spells.E].Range))
                {
                    spells[Spells.E].Cast(Target);
                }
            }

            if (Items.CanUseItem(3074) && hydraClear && Target.IsValidTarget(400f))
                Items.UseItem(3074);

            if (Items.CanUseItem(3077) && tiamatClear && Target.IsValidTarget(400f))
                Items.UseItem(3077);
        }

        #endregion

        #region laneclear

        private static void LaneClear()
        {
            var minion = MinionManager.GetMinions(Rengar.Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward")) return;

            var qWaveClear = ElRengarMenu._menu.Item("ElRengar.Clear.Q").GetValue<bool>();
            var wWaveClear = ElRengarMenu._menu.Item("ElRengar.Clear.W").GetValue<bool>();
            var eWaveClear = ElRengarMenu._menu.Item("ElRengar.Clear.E").GetValue<bool>();
            var hydraClear = ElRengarMenu._menu.Item("ElRengar.Clear.Hydra").GetValue<bool>();
            var tiamatClear = ElRengarMenu._menu.Item("ElRengar.Clear.Tiamat").GetValue<bool>();
            var saveClear = ElRengarMenu._menu.Item("ElRengar.Clear.Save").GetValue<bool>();
            var prioClear = ElRengarMenu._menu.Item("ElRengar.Clear.Prio").GetValue<StringList>();

            var bestFarmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(spells[Spells.W].Range, MinionTypes.All, MinionTeam.Enemy).Select(m => m.ServerPosition.To2D()).ToList(), spells[Spells.W].Width, spells[Spells.W].Range);
            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.R].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);


            if (Player.Mana <= 4)
            {
                if (wWaveClear && minion.IsValidTarget() && spells[Spells.W].IsReady())
                {
                    //Spells.W.Cast();
                    spells[Spells.W].Cast(bestFarmLocation.Position);
                }

                if (qWaveClear && minion.IsValidTarget() && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast();
                }

                if (eWaveClear && minion.IsValidTarget() && spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(minion);
                }
            }

            if (Player.Mana == 5)
            {
                if (saveClear)
                    return;

                if (prioClear.SelectedIndex == 0 && wWaveClear && minion.IsValidTarget() && spells[Spells.W].IsReady())
                {
                    spells[Spells.W].Cast();
                }

                if (prioClear.SelectedIndex == 1 && qWaveClear && minion.IsValidTarget() && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast();
                }

                if (prioClear.SelectedIndex == 2 && eWaveClear && minion.IsValidTarget() && spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(minion);
                }
            }

            if (Items.CanUseItem(3074) && hydraClear && minion.IsValidTarget(400f) && minions.Count() > 1)
                Items.UseItem(3074);

            if (Items.CanUseItem(3077) && tiamatClear && minion.IsValidTarget(400f) && minions.Count() > 1)
                Items.UseItem(3077);
        }
        
        #endregion


        #region selfheal

        private static void SelfHealing()
        {
            if (Player.HasBuff("Recall") || Player.InFountain() || Player.Mana <= 4)
                return;
            if (
                ElRengarMenu._menu.Item("ElRengar.Heal.AutoHeal").GetValue<bool>() && (Player.Health / Player.MaxHealth) * 100
                    <= ElRengarMenu._menu.Item("ElRengar.Heal.HP").GetValue<Slider>().Value && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }
        }

        #endregion

        #region Ignite

        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        #endregion

        #region AssassinManager
        private static Obj_AI_Hero GetEnemy(float vDefaultRange = 0, TargetSelector.DamageType vDefaultDamageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(vDefaultRange) < 0.00001)
                vDefaultRange = spells[Spells.R].Range;

            if (!ElRengarMenu._menu.Item("AssassinActive").GetValue<bool>())
                return TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType);

            var assassinRange = ElRengarMenu._menu.Item("AssassinSearchRange").GetValue<Slider>().Value;

            var vEnemy =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        enemy =>
                            enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible &&
                            ElRengarMenu._menu.Item("Assassin" + enemy.ChampionName) != null &&
                            ElRengarMenu._menu.Item("Assassin" + enemy.ChampionName).GetValue<bool>() &&
                            ObjectManager.Player.Distance(enemy) < assassinRange);

            if (ElRengarMenu._menu.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex == 1)
            {
                vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
            }

            Obj_AI_Hero[] objAiHeroes = vEnemy as Obj_AI_Hero[] ?? vEnemy.ToArray();

            Obj_AI_Hero t = !objAiHeroes.Any()
                ? TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType)
                : objAiHeroes[0];

            return t;
        }

        #endregion

        private static bool hpLowerParam(Obj_AI_Base obj, String paramName)
        {
            return ((obj.Health / obj.MaxHealth) * 100) <= ElRengarMenu._menu.Item(paramName).GetValue<Slider>().Value;
        }

        #region Autosmite
        private static double SmiteDmg()
        {
            int[] dmg =
            {
                20*Player.Level + 370, 30*Player.Level + 330, 40*+Player.Level + 240, 50*Player.Level + 100
            };
            return Player.Spellbook.CanUseSpell(smiteSlot) == SpellState.Ready ? dmg.Max() : 0;
        }

        private static bool paramBool(String paramName)
        {
            return (ElRengarMenu._menu.Item(paramName).GetValue<bool>());
        }

        private static void smiter()
        {
            //var minion = ObjectManager.Get<Obj_AI_Minion>().Where(a => buffandepics.Contains(a.BaseSkinName) && a.Distance(Player) <= 1300).FirstOrDefault();
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(a => buffandepics.Contains(a.BaseSkinName) && a.Distance(Player) <= 1300);
            if (minion != null)
            {
                if (ElRengarMenu._menu.Item(minion.BaseSkinName).GetValue<bool>())
                {
                    minionerimo = minion;
                    if (SmiteDmg() > minion.Health && minion.IsValidTarget(780) && paramBool("ElRengar.normalSmite")) Player.Spellbook.CastSpell(smiteSlot, minion);
                    if (minion.Distance(Player) < 100 && checkSmite)
                    {
                        checkSmite = false;
                        Player.Spellbook.CastSpell(smiteSlot, minion);
                    }

                }
            }
        }

        #endregion

        #region SmiteSaver
        private static void SaveMe()
        {
            if ((Player.Health / Player.MaxHealth * 100) > ElRengarMenu._menu.Item("hpPercentSM").GetValue<Slider>().Value || Player.Spellbook.CanUseSpell(smiteSlot) != SpellState.Ready) return;
            var epicSafe = false;
            var buffSafe = false;
            foreach (
                var minion in
                    MinionManager.GetMinions(Player.Position, 1100f, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.None))
            {
                foreach (var minionName in epics)
                {
                    if (minion.BaseSkinName == minionName && hpLowerParam(minion, "hpEpics") && paramBool("dEpics"))
                    {
                        epicSafe = true;
                        break;
                    }
                }
                foreach (var minionName in buffs)
                {
                    if (minion.BaseSkinName == minionName && hpLowerParam(minion, "hpBuffs") && paramBool("dBuffs"))
                    {
                        buffSafe = true;
                        break;
                    }
                }
            }

            if (epicSafe || buffSafe) return;
        }
        #endregion

        //Start Credits to Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        private static string smitetype()
        {
            if (SmiteBlue.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(a => Items.HasItem(a)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }
        //End credits
    }
}
