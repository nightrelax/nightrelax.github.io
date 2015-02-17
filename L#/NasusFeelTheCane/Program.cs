using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace NasusFeelTheCane
{
    class Program
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Obj_AI_Hero Player;
        public static Int32 Sheen = 3057, Iceborn = 3025;
        public static float respawnDelay;

        public static List<NewBuff> buffList = new List<NewBuff>
        {
            
            new NewBuff()
            {
                DisplayName = "PantheonPassiveShield", Name = "pantheonpassiveshield"
            },
            new NewBuff()
            {
                DisplayName = "FioraRiposte", Name = "FioraRiposte"
            },
        };
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                respawnDelay = Environment.TickCount + 500;
            var jungleMinions = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All,
                    MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var laneMinions = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (Config.Item("AutoLastHitQ").GetValue<KeyBind>().Active && !Player.HasBuff("Recall") && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                foreach (var minion in laneMinions)
                {
                    if (GetBonusDmg(minion) > minion.Health &&
                       Vector3.Distance(ObjectManager.Player.ServerPosition, minion.Position) < Orbwalking.GetRealAutoAttackRange(Player) + 50 && Q.IsReady())
                    {
                        Orbwalker.SetAttack(false);
                        Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        Orbwalker.SetAttack(true);
                        break;
                    }
                }
            }

            Obj_AI_Hero target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            if ((Player.Health / Player.MaxHealth * 100) <= Config.Item("minRHP").GetValue<Slider>().Value && !Player.InFountain() && Environment.TickCount >= respawnDelay)
            {
                if ((Config.Item("minRChamps").GetValue<Slider>().Value == 0) ||
                    (Config.Item("minRChamps").GetValue<Slider>().Value > 0) &&
                    Utility.CountEnemiesInRange(800) >= Config.Item("minRChamps").GetValue<Slider>().Value)
                {
                    R.Cast(true);
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target != null)
            {
                if (target.IsValidTarget(W.Range) && paramBool("ComboW")) W.CastOnUnit(target);
                if (target.IsValidTarget(E.Range + E.Width) && paramBool("ComboE")) E.Cast(target, Config.Item("packets").GetValue<bool>());
                if (hasAntiAA(target)) return;
                if (target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) + 200) && paramBool("ComboQ"))
                {
                    Q.Cast(Config.Item("packets").GetValue<bool>());
                }

            }
            if (isFarmMode())
            {

                if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear))
                {
                    if (jungleMinions.Count > 0)
                    {
                        if (Q.IsReady() && Q.IsReady() && paramBool("WaveClearQ"))
                        {
                            Q.Cast(Config.Item("packets").GetValue<bool>());
                        }
                        if (!E.IsReady() && paramBool("WaveClearE"))
                        {
                            List<Vector2> minionerinos2 =
                                (from minions in jungleMinions select minions.Position.To2D()).ToList();
                            var ePos2 =
                                MinionManager.GetBestCircularFarmLocation(minionerinos2, E.Width, E.Range).Position;
                            if (ePos2.Distance(Player.Position.To2D()) < E.Range)
                            {
                                E.Cast(ePos2, Config.Item("packets").GetValue<bool>());
                            }
                        }
                    }

                    if (jungleMinions.Count > 0) return;
                    foreach (var minion in laneMinions)
                    {
                        if (GetBonusDmg(minion) > minion.Health &&
                            Vector3.Distance(ObjectManager.Player.ServerPosition, minion.Position) < Orbwalking.GetRealAutoAttackRange(Player) + 50 && Q.IsReady() && paramBool("JungleQ"))
                        {
                            Orbwalker.SetAttack(false);
                            Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                            Orbwalker.SetAttack(true);
                            break;
                        }
                    }
                    if (!E.IsReady() && paramBool("JungleE"))
                    {
                        List<Vector2> minionerinos =
                            (from minions in laneMinions select minions.Position.To2D()).ToList();
                        var ePos2 =
                            MinionManager.GetBestCircularFarmLocation(minionerinos, E.Width, E.Range).Position;
                        if (ePos2.Distance(Player.Position.To2D()) < E.Range)
                        {
                            E.Cast(ePos2, Config.Item("packets").GetValue<bool>());
                        }
                    }
                }
                if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit) && paramBool("LastHitQ"))
                {
                    if (jungleMinions.Count > 0) return;
                    foreach (var minion in laneMinions)
                    {
                        if (GetBonusDmg(minion) > minion.Health &&
                            Vector3.Distance(ObjectManager.Player.ServerPosition, minion.Position) < Orbwalking.GetRealAutoAttackRange(Player) + 50 && Q.IsReady())
                        {
                            Orbwalker.SetAttack(false);
                            Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                            Orbwalker.SetAttack(true);
                            break;
                        }
                    }
                }
            }
        }

        public static bool hasAntiAA(Obj_AI_Hero target)
        {
            return buffList.Any(buff => target.HasBuff(buff.DisplayName) || target.HasBuff(buff.Name) || Player.HasBuffOfType(BuffType.Blind));
        }

        public static bool isFarmMode()
        {
            return Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear ||
                   Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit;
        }

        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!args.SData.Name.ToLower().Contains("attack") || !sender.IsMe) return;
            var unit = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(args.Target.NetworkId);
            if ((GetBonusDmg(unit) > unit.Health))
            {
                Q.Cast(Config.Item("packets").GetValue<bool>());
            }
        }

        // From Master of Nasus + modified by me
        private static double GetBonusDmg(Obj_AI_Base target)
        {
            double DmgItem = 0;
            if (Items.HasItem(Sheen) && (Items.CanUseItem(Sheen) || Player.HasBuff("sheen", true)) && Player.BaseAttackDamage > DmgItem) DmgItem = Damage.GetAutoAttackDamage(Player, target);
            if (Items.HasItem(Iceborn) && (Items.CanUseItem(Iceborn) || Player.HasBuff("itemfrozenfist", true)) && Player.BaseAttackDamage * 1.25 > DmgItem) DmgItem = Damage.GetAutoAttackDamage(Player, target) * 1.25;
            return Q.GetDamage(target) + Player.GetAutoAttackDamage(target) + DmgItem;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Nasus") return;
            Player = ObjectManager.Player;
            Q = new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(Player));
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 0);
            E.SetSkillshot(E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);

            Config = new Menu("Nasus - Feel The Cane", "nftc", true);

            var OWMenu = Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(OWMenu);
            var TSMenu = Config.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(TSMenu);
            var ComboMenu = Config.AddSubMenu(new Menu("Combo", "Combo"));
            ComboMenu.AddItem(new MenuItem("ComboQ", "Combo with Q").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboW", "Combo with W").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboE", "Combo with E").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ndskafjk", "-- R Settings"));
            ComboMenu.AddItem(new MenuItem("ComboR", "Combo with R").SetValue(true));
            ComboMenu.AddItem(new MenuItem("minRHP", "Min HP For R").SetValue(new Slider(1, 1)));
            ComboMenu.AddItem(new MenuItem("minRChamps", "Min Champs For R").SetValue(new Slider(0, 0, 5)));
            ComboMenu.AddItem(new MenuItem("fsffs", "Set to 0 to disable"));

            var FarmMenu = Config.AddSubMenu(new Menu("Farm", "Farm"));
            FarmMenu.AddItem(new MenuItem("AutoLastHitQ", "Auto LastHit with Q").SetValue(new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle)));
            FarmMenu.AddItem(new MenuItem("pratum", "-- Last Hit"));
            FarmMenu.AddItem(new MenuItem("LastHitQ", "LastHit with Q").SetValue(true));
            FarmMenu.AddItem(new MenuItem("pratum2", "-- WaveClear"));
            FarmMenu.AddItem(new MenuItem("WaveClearQ", "WaveClear with Q").SetValue(true));
            FarmMenu.AddItem(new MenuItem("WaveClearE", "WaveClear with E").SetValue(true));
            FarmMenu.AddItem(new MenuItem("pratum22", "-- Jungle"));
            FarmMenu.AddItem(new MenuItem("JungleQ", "Jungle with Q").SetValue(true));
            FarmMenu.AddItem(new MenuItem("JungleE", "Jungle with E").SetValue(true));

            var DrawMenu = Config.AddSubMenu(new Menu("HP Bar Indicator", "HP Bar Indicator"));
            DrawMenu.AddItem(new MenuItem("drawAA", "Draw AA on HP Bar").SetValue(false));
            DrawMenu.AddItem(new MenuItem("LineAAThicknessColour", "AA Linethickness / Colour").SetValue(new Circle(true, Color.CornflowerBlue, 10)));
            DrawMenu.AddItem(new MenuItem("drawHPBar", "Draw AA + Q + Item on HP Bar").SetValue(true));
            DrawMenu.AddItem(new MenuItem("LineThicknessColour", "Linethickness / Colour").SetValue(new Circle(true, Color.White, 10)));


            Config.AddItem(new MenuItem("packets", "Packet Cast?")).SetValue(true);

            Config.AddToMainMenu();

            respawnDelay = Environment.TickCount;

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            List<Obj_AI_Base> minionList = MinionManager.GetMinions(Player.Position,
                Orbwalking.GetRealAutoAttackRange(Player) + 500, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).ToList();
            foreach (var minion in minionList.Where(minion => minion.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) + 500)))
            {
                var attackToKill = Math.Ceiling(minion.MaxHealth / ((minion.MaxHealth > GetBonusDmg(minion)) ? GetBonusDmg(minion) : minion.MaxHealth -1));
                var hpBarPosition = minion.HPBarPosition;
                var barWidth = minion.IsMelee() ? 75 : 80;
                if (minion.HasBuff("turretshield", true))
                    barWidth = 70;

                
                var barDistance = (int) (barWidth / attackToKill);
                if (Config.Item("drawHPBar").GetValue<bool>())
                {
                    var startposition = hpBarPosition.X + 45 + barDistance;
                    Drawing.DrawLine(
                        new Vector2(startposition, hpBarPosition.Y + 18),
                        new Vector2(startposition, hpBarPosition.Y + 23),
                        2,
                        Config.Item("LineThicknessColour").GetValue<Circle>().Color);
                }
                if (Config.Item("drawAA").GetValue<bool>())
                {
                    var ad = Player.CalcDamage(minion, Damage.DamageType.Physical,
                        Player.FlatPhysicalDamageMod + Player.BaseAttackDamage);
                    var amount = Math.Floor(minion.Health/ad);
                    for (int i = 0; i >= amount; i++)
                    {
                        barDistance = barWidth / (int) (i*ad > minion.Health ? minion.MaxHealth/(minion.MaxHealth-1) : minion.MaxHealth/(i*ad));

                        var startposition = hpBarPosition.X + 45 + barDistance;
                        Drawing.DrawLine(
                            new Vector2(startposition, hpBarPosition.Y + 18),
                            new Vector2(startposition, hpBarPosition.Y + 23),
                            2,
                            Config.Item("LineAAThicknessColour").GetValue<Circle>().Color);
                    }
                }
            }
        }

        public static bool paramBool(String menuName)
        {
            return Config.Item(menuName).GetValue<bool>();
        }
    }
}
