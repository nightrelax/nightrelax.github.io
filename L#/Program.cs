/// Tools For L# NewBie Level :( Developer BR
/// NewBie :( Developer BR

#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
#endregion

#region
namespace GNomeProject
{
    //public class Program
    //{
    //    public static Obj_AI_Hero _Player { get { return ObjectManager.Player; } }
    //    public static List<string> _Hero = new List<string>();
    //    static void Main(string[] args)
    //    {
    //        //LoadHeroList();
    //            if (_Player.BaseSkinName == "Annie")
    //            {
    //                //Hero.Annie.AnnieLoad();
    //                // Game.OnGameStart += GNomeProject.Hero.Annie.Game_OnGameLoad;
    //                //CustomEvents.Game.OnGameLoad += Annie.Game_OnGameLoad;
    //                Annie.AnnieLoad();
    //            }
    //            else
    //            {
    //                return;
    //            }
    //    }
    //    //static void LoadHeroList()
    //    //{
    //    //    _Hero.Add("Annie");
    //    //}
    //}


    public class Program 
    {
        #region DECLARE
        public static string _GVersao = "GN Annie";
        public static string _Me = "Annie";
        public static Obj_AI_Hero _Player { get { return ObjectManager.Player; } }
        public static Menu _Menu;
        public static Orbwalking.Orbwalker _Orbwalker;

        public static readonly List<Spell> _SpellList = new List<Spell>();
        public static readonly List<SpellSlot> _SumList = new List<SpellSlot>();
        public static SpellSlot _Ingnite;
        public static SpellSlot _Flash;
        public static Spell _Q, _W, _E, _R, _R1;
        private static int[] _AnnieLevel = { 2, 1, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };

        public static Obj_AI_Hero _LineTarget;
        private static Obj_AI_Base _Tibbers;

        public static int StunCount
        {
            get
            {
                foreach (var buff in
                    ObjectManager.Player.Buffs.Where(
                        buff => buff.Name == "pyromania" || buff.Name == "pyromania_particle"))
                {
                    switch (buff.Name)
                    {
                        case "pyromania":
                            return buff.Count;
                        case "pyromania_particle":
                            return 4;
                    }
                }

                return 0;
            }
        }
        #endregion

        static void Main(string[] args)
        {
            //if (_Player.BaseSkinName == "Annie") { CustomEvents.Game.OnGameLoad += Game_OnGameLoad; ; } else { return; }
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        //public static void AnnieLoad()
        //{
        //    CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        //}
        //public static void AnnieLoad(EventArgs args)
        //{

        //    Console.WriteLine("Annie Load . . .");
        //    CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        //Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        //AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        //}

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_Menu.Item("gape").GetValue<bool>())
            {
                if (_Menu.Item("gapuseq").GetValue<bool>())
                {
                    if (ObjectManager.Player.Distance(gapcloser.Sender, false) < ObjectManager.Player.AttackRange)
                    { _Q.CastOnUnit(gapcloser.Sender); }
                }
            }

        }

        public static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (_Menu.Item("gape").GetValue<bool>())
            {
                if (_Menu.Item("gapuseq").GetValue<bool>())
                {
                    if (ObjectManager.Player.Distance(sender, false) < ObjectManager.Player.AttackRange)
                    { _Q.CastOnUnit(sender); }
                }
            }

        }

        public static int _EnemyRange(Vector3 _pos, float _range)
        {
            return
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero.Team != ObjectManager.Player.Team)
                    .Count(hero => Vector3.Distance(_pos, hero.ServerPosition) <= _range);

        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            if (_Player.BaseSkinName != "Annie") { return; }
            LoadMenuAnnie();
            LoadSpellAnnie();
            Console.WriteLine("Annie Load . . .");
           
            Drawing.OnDraw += Drawing_OnDraw;
            //Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;

            if (_Menu.Item("modautolevel").GetValue<bool>())
            {
                AutoLevelAnnie(null, null);
            }

            Game.OnGameUpdate += Game_OnGameUpdate;
            
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.PrintChat("<font color='#FFFF00'>#Hi  </font><font color='#35DE4C'>" + _GVersao + "</font> Loaded#");
            Console.WriteLine("Annie Inject . . .");
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }



        public static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.Name.Equals("Tibbers"))
                return;
            Console.WriteLine("TibbersAnnieDead");
            _Tibbers = null;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //Console.WriteLine(sender.Name);
            if (sender == null || !sender.IsValid || !sender.Name.Equals("Tibbers")) { return; }
            Console.WriteLine("TibbersAnnie");
            _Tibbers = (Obj_AI_Base)sender;
        }


        public static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {

        }

        public static void AutoLevelAnnie(object sender, OnValueChangeEventArgs e)
        {
            var _LevelUP = new AutoLevel(_AnnieLevel);
        }

        public static void LoadSpellAnnie()
        {
            _Q = new Spell(SpellSlot.Q, 625f);
            _Q.SetTargetted(0.15f, 1500f);

            _W = new Spell(SpellSlot.W, 610f);
            _W.SetSkillshot(0.15f, 75f, 1500f, false, SkillshotType.SkillshotCone);

            _E = new Spell(SpellSlot.E);

            _R = new Spell(SpellSlot.R, 625);
            _R.SetSkillshot(0.15f, 75f, 1500f, false, SkillshotType.SkillshotCircle);

            _R1 = new Spell(SpellSlot.Unknown, 400f);
            _R1.SetSkillshot(0.15f, 75f, 1500f, false, SkillshotType.SkillshotCircle);

            _Ingnite = _Player.GetSpellSlot("SummonerDot");
            _Flash = _Player.GetSpellSlot("SummonerFlash");

            _SpellList.Add(_Q);
            _SpellList.Add(_W);
            _SpellList.Add(_E);
            _SpellList.Add(_R);
            _SpellList.Add(_R1);
            _SumList.Add(_Ingnite);
            _SumList.Add(_Flash);
        }

        private static void LoadMenuAnnie()
        {
            _Menu = new Menu("Gnome Annie", "gnomeannie", true);

            var targetSelector = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(targetSelector);
            _Menu.AddSubMenu(targetSelector);


            _Menu.AddSubMenu(new Menu("Drawing", "drawing"));
            _Menu.SubMenu("drawing").AddItem(new MenuItem("drawenabled", "Draw Enabled").SetValue(true));
            _Menu.SubMenu("drawing").AddItem(new MenuItem("aa", "AA").SetValue(true)); // Auto Atk 
            _Menu.SubMenu("drawing").AddItem(new MenuItem("qq", "Spell Q").SetValue(true)); // Spell Q  
            _Menu.SubMenu("drawing").AddItem(new MenuItem("ww", "Spell W").SetValue(false)); // Spell W
            _Menu.SubMenu("drawing").AddItem(new MenuItem("rr", "Spell R").SetValue(false)); // Spell R
            _Menu.SubMenu("drawing").AddItem(new MenuItem("r1", "Flash + R").SetValue(true)); // Flash + R
            _Menu.SubMenu("drawing").AddItem(new MenuItem("dmodflash", "Status Flash R").SetValue(true));
            _Menu.SubMenu("drawing").AddItem(new MenuItem("dmodstun", "Status Stun Load").SetValue(true));
            _Menu.SubMenu("drawing").AddItem(new MenuItem("dmodlevel", "Status Level").SetValue(true));
            //_Menu.SubMenu("")



            _Menu.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
            _Orbwalker = new Orbwalking.Orbwalker(_Menu.SubMenu("orbwalker"));
            _Menu.SubMenu("orbwalker").AddItem(new MenuItem("flee", "Flee").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Press)));


            _Menu.AddSubMenu(new Menu("Annie Style", "anniestyle"));
            _Menu.SubMenu("anniestyle").AddItem(new MenuItem("modflash", "Flash + R").SetValue(false)); // Menu Flash R
            _Menu.SubMenu("anniestyle").AddItem(new MenuItem("modflash.enabled.count", "Enabled IF xEnemy").SetValue(false)); // So flash Com tibber com x inimigos ... 
            _Menu.SubMenu("anniestyle").AddItem(new MenuItem("mod.flash.count.inimigo", "IF FLASH xEnemy ").SetValue(new Slider(4))); // Valos dos inimigos
            _Menu.SubMenu("anniestyle").AddItem(new MenuItem("modstun", "Load Stun").SetValue(true));  // Load Stun
            _Menu.SubMenu("anniestyle").AddItem(new MenuItem("modautolevel", "Auto Level Spell").SetValue(true)); // Auto Level Spell
            _Menu.SubMenu("anniestyle").AddItem(new MenuItem("modks", "Auto Ingnite").SetValue(true)); // Ingnite 



            _Menu.AddSubMenu(new Menu("Gapcloser", "gapcloser"));
            _Menu.SubMenu("gapcloser").AddItem(new MenuItem("gape", "Enabled Gapcloser").SetValue(true));
            _Menu.SubMenu("gapcloser").AddItem(new MenuItem("gapuseq", "Spell Q").SetValue(false));
            _Menu.SubMenu("gapcloser").AddItem(new MenuItem("gapusew", "Spell W").SetValue(false));
            //_Menu.SubMenu("gapcloser").AddItem(new MenuItem("gapuser", "Spell R").SetValue(false));


            _Menu.AddSubMenu(new Menu("Farm", "farmstyle"));
            _Menu.SubMenu("farmstyle").AddItem(new MenuItem("useq", "Use Q Last Hit").SetValue(false)); // Use Spell Q Last Hit
            _Menu.SubMenu("farmstyle").AddItem(new MenuItem("usew", "Use W Lane Clear").SetValue(false)); // Use Spell W Last Hit
            _Menu.SubMenu("farmstyle").AddItem(new MenuItem("notfarmstun", "Not Farm Stun").SetValue(true)); //Caso Esteja com Stun ela nao Farma

            // 27/02 Adcionado - Mana -Health
            _Menu.AddSubMenu(new Menu("Manager potion", "managerpotion"));
            _Menu.SubMenu("managerpotion").AddItem(new MenuItem("autopotion", "Enabled Auto Potion").SetValue(true));
            _Menu.SubMenu("managerpotion").AddItem(new MenuItem("mana.mana", "Mana % <= Use Mana").SetValue(new Slider(30)));
            _Menu.SubMenu("managerpotion").AddItem(new MenuItem("mana.health", "Health % <= Use Health").SetValue(new Slider(30)));


            // 27 /02  Adcionado Carregar stun rapido na base
            _Menu.AddSubMenu(new Menu("Load stun fast base", "load.fast.stun.base"));
            _Menu.SubMenu("load.fast.stun.base").AddItem(new MenuItem("load.fast.enabled", "Load Enabled").SetValue(true));
            _Menu.SubMenu("load.fast.stun.base").AddItem(new MenuItem("load.fast.cast.w", "Cast W").SetValue(true));
            _Menu.SubMenu("load.fast.stun.base").AddItem(new MenuItem("load.fast.cast.e", "Cast E").SetValue(true));




            _Menu.AddToMainMenu();
        }

        #region Method AutoPot - requeriments  27/02 Adcionado
        // 27/02 - Adcionado AutoPot 
        private static void AutoPotUse()
        {
            if (_Player.HasBuff("Recall") || _Player.InFountain() && _Player.InShop()) { return; }

            // Pote de Vida
            if (_Menu.Item("autopotion").GetValue<bool>() && GetPerValue(false) <= _Menu.Item("mana.health").GetValue<Slider>().Value && !_Player.HasBuff("RegenerationPotion", true)) { UseItem(2003); }
            //else if (_Menu.Item("autopotion").GetValue<bool>() && GetPerValue(false) <= _Menu.Item("mana.health").GetValue<Slider>().Value && !_Player.HasBuff("RegenerationPotion", true)) { UseItem(2041); } 

            // Pote de Mana
            if (_Menu.Item("autopotion").GetValue<bool>() && GetPerValue(true) <= _Menu.Item("mana.mana").GetValue<Slider>().Value && !_Player.HasBuff("FlaskOfCrystalWater", true)) { UseItem(2004); }
            //else if (_Menu.Item("autopotion").GetValue<bool>() && GetPerValue(true) <= _Menu.Item("mana.mana").GetValue<Slider>().Value && !_Player.HasBuff("FlaskOfCrystalWater", true)) { UseItem(2041); } 
        }

        public static float GetPerValue(bool mana)
        {
            return mana ? _Player.ManaPercentage() : _Player.HealthPercentage();
        }

        public static void UseItem(int id, Obj_AI_Hero target = null)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                //if (Items.HasItem(2041) && Items.CanUseItem(2014))
                //{
                //    Items.UseItem(2014, target); return; 
                //}

                Items.UseItem(id, target);
            }
        }
        #endregion

        public static void Game_OnGameUpdate(EventArgs args)
        {

            if (ObjectManager.Player.HasBuff("Recall") || ObjectManager.Player.IsDead) return;

            var target = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
            // Se Target Estiver Meado Incendiar

            if (CastIncendiar(target) && _Ingnite.IsReady() && target != null)
            {
                ObjectManager.Player.Spellbook.CastSpell(_Ingnite, target);
            }

            //Load Stun
            if (_Menu.Item("modstun").GetValue<bool>())
            {
                LoadStun(true);
            }
            else
            {
                LoadStun(false);
            }

            //Flee
            if (_Menu.Item("flee").GetValue<KeyBind>().Active)
            {
                _Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            //Auto Level
            if (_Menu.Item("modautolevel").GetValue<bool>())
            {
                AutoLevelAnnie(null, null);
            }



            //Orbwalker Modes
            switch (_Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.LaneClear:
                    _ModFarm(true);
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    _ModFarm(false);
                    break;

                case Orbwalking.OrbwalkingMode.Combo:
                    Combo_(target, true);
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harras_(target);
                    break;
            }

            // 27/02 Adcionado -Teste Potion manager 
            AutoPotUse();

            // 27 / 02  Adcionado Cast Spell carregar stun rapido
            if (_Menu.Item("load.fast.enabled").GetValue<bool>() && ObjectManager.Player.InFountain() && StunCount != 4)
            {
                if (_W.IsReady() || _E.IsReady()) // { _E.Cast(); _W.Cast(_Player.Position, false); }
                {
                    if (_Menu.Item("load.fast.cast.w").GetValue<bool>()) { _W.Cast(_Player.Position, false); }
                    if (_Menu.Item("load.fast.cast.e").GetValue<bool>()) { _E.Cast(); }
                }
            }



        }

        public static void _ModFarm(bool _ModFarmStyle)
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _Q.Range);
            var jungleMinions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, _Q.Range, MinionTypes.All, MinionTeam.Neutral);
            minions.AddRange(jungleMinions);
            if (StunCount == 4 && _Menu.Item("notfarmstun").GetValue<bool>()) { return; }

            if (_Menu.Item("usew").GetValue<bool>() && _ModFarmStyle == true && _W.IsReady() && minions.Count != 0)
            {
                _W.Cast(_W.GetLineFarmLocation(minions).Position);
            }
            else if (_Menu.Item("useq").GetValue<bool>() && _Q.IsReady() && minions.Count >= 0)
            {
                foreach (var minion in
                from minion in
                    minions.OrderByDescending(Minions => Minions.MaxHealth)
                        .Where(minion => minion.IsValidTarget(_Q.Range))
                let predictedHealth = _Q.GetHealthPrediction(minion)
                where
                    predictedHealth < ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) * 0.85 &&
                    predictedHealth > 0
                select minion)
                {
                    _Q.CastOnUnit(minion, _Menu.Item("useq").GetValue<bool>());
                }
            }

        }

        public static bool CastIncendiar(Obj_AI_Base _target)
        {
            if (_target == null) return false;
            int _dmg_Incediar_Base = 50 + (_Player.Level * 20);

            if (_target.Health <= _dmg_Incediar_Base)
            {
                return true;
            }
            else if (_target.Health == _dmg_Incediar_Base)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void LoadStun(bool _enabledOrFalse)
        {
            if (ObjectManager.Player.HasBuff("Recall") || _Player.IsDead || _enabledOrFalse == false || StunCount == 4) return;
            if (_Menu.Item("modstun").GetValue<bool>() || _E.IsReady())
            {
                _E.Cast();
                return;
            }
        }

        public static void Harras_(Obj_AI_Base _target)
        {
            if (_target == null)
            {
                return;
            }

            if (_Q.IsReady() || _W.IsReady())
            {
                _Q.CastOnUnit(_target, false);
                _W.CastOnUnit(_target, false);

            }
            return;
        }

        public static void Combo_(Obj_AI_Base _target, bool _AstyleHarass)
        {
            if (_target == null)
            {
                _Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                return;
            }


            if (CastIncendiar(_target) && _Ingnite.IsReady() && _Menu.Item("modks").GetValue<bool>())
            {

                ObjectManager.Player.Spellbook.CastSpell(_Ingnite, _target);

            }

            //Combo
            //Stun R Q W 
            if (StunCount == 4 && _R.IsReady() && _Q.IsReady() && _W.IsReady() && _target != null)
            {

                if (_Menu.Item("modflash").GetValue<bool>() && StunCount == 4 && _R.IsReady() && _Tibbers == null)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_Flash, _R1.GetPrediction(_target, true).UnitPosition);
                }
                _R.CastOnUnit(_target, false);
                _Q.CastOnUnit(_target, false);
                _W.CastOnUnit(_target, false);
                _Orbwalker.SetAttack(true);
                _Player.IssueOrder(GameObjectOrder.AutoAttackPet, _target);


            }
            else if (_Q.IsReady() || _W.IsReady() || _R.IsReady())
            {

                _R.CastOnUnit(_target, false);
                _Q.CastOnUnit(_target, false);
                _W.CastOnUnit(_target, false);
                _Orbwalker.SetAttack(true);
                _Player.IssueOrder(GameObjectOrder.AutoAttackPet, _target);

            }


            else if (_Q.IsReady() == false && _R.IsReady() == false && _W.IsReady() == false)
            {
                _Orbwalker.SetAttack(true);
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (_Menu.Item("drawenabled").GetValue<bool>() == false || _Player.IsDead == true) return;

            // Range Auto Atk 
            if (_Menu.Item("aa").GetValue<bool>())
            {
                if (_EnemyRange(_Player.ServerPosition, 670f) >= 1) // Auto Atk Annie 670...
                {
                    Drawing.DrawCircle(_Player.Position, 670f, Color.DarkRed);
                }
                else
                {
                    Drawing.DrawCircle(_Player.Position, 670f, Color.DarkSlateGray);
                }
            }

            //Spell Q
            if (_Menu.Item("qq").GetValue<bool>())
            {
                if (_EnemyRange(_Player.Position, 625f) >= 1)
                {
                    Drawing.DrawCircle(_Player.Position, 625f, Color.DarkRed);
                }
                else
                {
                    Drawing.DrawCircle(_Player.Position, 625f, Color.DarkSlateGray);
                }

            }

            //Spell W
            if (_Menu.Item("ww").GetValue<bool>())
            {
                if (_EnemyRange(_Player.Position, 610f) >= 1)
                {
                    Drawing.DrawCircle(_Player.Position, 610f, Color.DarkRed);
                }
                else
                {
                    Drawing.DrawCircle(_Player.Position, 610f, Color.DarkSlateGray);
                }

            }

            //Spell R
            if (_Menu.Item("rr").GetValue<bool>())
            {
                if (_EnemyRange(_Player.Position, 625f) >= 1)
                {
                    Drawing.DrawCircle(_Player.Position, 625f, Color.DarkRed);
                }
                else
                {
                    Drawing.DrawCircle(_Player.Position, 625f, Color.DarkSlateGray);
                }
            }

            //Spell Flash R
            if (_Menu.Item("r1").GetValue<bool>())
            {
                if (_EnemyRange(_Player.Position, 1025f) >= 1)
                {
                    Drawing.DrawCircle(_Player.Position, 1025f, Color.DarkRed);
                }
                else
                {
                    Drawing.DrawCircle(_Player.Position, 1025f, Color.DarkSlateGray);
                }
            }

            //Flash R Status
            if (_Menu.Item("modflash").GetValue<bool>() && _Menu.Item("dmodflash").GetValue<bool>())
            {

                Drawing.DrawText(1000f, 50f, Color.White, "Flash R On");
            }
            else
            {
                if (_Menu.Item("dmodflash").GetValue<bool>()) { Drawing.DrawText(1000f, 50f, Color.DarkSlateGray, "Flash R Off"); ;}
            }

            //Mod Stun
            if (_Menu.Item("modstun").GetValue<bool>() && _Menu.Item("dmodstun").GetValue<bool>())
            {
                Drawing.DrawText(1000f, 60f, Color.White, "Load Stun On");
            }
            else
            {
                if (_Menu.Item("dmodstun").GetValue<bool>()) { Drawing.DrawText(1000f, 60f, Color.DarkSlateGray, "Load Stun Off"); ;}
            }

            //Mod AutoLevel
            if (_Menu.Item("modautolevel").GetValue<bool>() && _Menu.Item("dmodlevel").GetValue<bool>())
            {
                Drawing.DrawText(1000f, 70f, Color.White, "Auto Level On");
            }
            else
            {
                if (_Menu.Item("dmodlevel").GetValue<bool>()) { Drawing.DrawText(1000f, 70f, Color.DarkSlateGray, "Auto Level Off"); ;}
            }


        }
    }

    #region Class PotionManager 27 / 02
    // Teste Beta Class potion manager
    internal class PotionManager
    {

        class Potion
        {
            public String Name { get; set; }
            public PotionType Type { get; set; }
            public String BuffName { get; set; }
            public ItemId ItemId { get; set; }
            public int Priority { get; set; }
            public bool IsRunning
            {
                get { return ObjectManager.Player.HasBuff(BuffName, true); }
            }
        }

        enum PotionType
        {
            Health,
            Mana,
            Flask
        }

        private static readonly List<Potion> Potions = new List<Potion>
        {
            new Potion
            {
                Name = "Health Potion",
                BuffName = "RegenerationPotion",
                ItemId = (ItemId)2003,
                Type =  PotionType.Health,
                Priority = 2
            },
            new Potion
            {
                Name = "Mana Potion",
                BuffName = "FlaskOfCrystalWater",
                ItemId = (ItemId)2004,
                Type =  PotionType.Mana,
                Priority = 2
            },
            //new Potion
            //{
            //    Name = "Crystal Flask",
            //    BuffName = "ItemCrystalFlask",
            //    ItemId = (ItemId)2041,
            //    Type =  PotionType.Flask,
            //    Priority = 3
            //},
            new Potion
            {
                Name = "Biscuit",
                BuffName = "ItemMiniRegenPotion",
                ItemId = (ItemId)2010,
                Type =  PotionType.Flask,
                Priority = 1
            },
        };

        private static void UsePotion()
        {
            var hpSlot = GetHpSlot();
            if (hpSlot != SpellSlot.Unknown && hpSlot.IsReady())
            {
                ObjectManager.Player.Spellbook.CastSpell(hpSlot, ObjectManager.Player);
                return;
            }


            var manaSlot = GetManaSlot();
            if (manaSlot != SpellSlot.Unknown && manaSlot.IsReady())
            {
                ObjectManager.Player.Spellbook.CastSpell(manaSlot, ObjectManager.Player);
            }

        }
        private static List<int> GetManaIds()
        {
            var ManaIds = new List<int>();
            foreach (var pot in Potions)
            {
                if (pot.Type == PotionType.Mana || pot.Type == PotionType.Flask && Items.HasItem((int)pot.ItemId))
                {
                    ManaIds.Add((int)pot.ItemId);
                }
            }
            return ManaIds;
        }
        private static SpellSlot GetManaSlot()
        {
            var ordered = Potions.Where(p => p.Type == PotionType.Health || p.Type == PotionType.Flask).OrderByDescending(pot => pot.Priority);
            var potSlot = SpellSlot.Unknown;
            var lastPriority = ordered.First().Priority;

            foreach (
                var Item in
                    ObjectManager.Player.InventoryItems.Where(
                        item =>
                            GetManaIds().Contains((int)item.Id)))  //&&
            //PennyJinx.IsMenuEnabled(((int)item.Id).ToString())))
            {
                var currentPriority = Potions.First(it => it.ItemId == Item.Id).Priority;
                if (currentPriority <= lastPriority)
                {
                    potSlot = Item.SpellSlot;
                }
            }
            return potSlot;
        }
        private static SpellSlot GetHpSlot()
        {
            var ordered = Potions.Where(p => p.Type == PotionType.Health || p.Type == PotionType.Flask).OrderByDescending(pot => pot.Priority);
            var potSlot = SpellSlot.Unknown;
            var lastPriority = ordered.First().Priority;

            foreach (
                var Item in
                    ObjectManager.Player.InventoryItems.Where(
                        item =>
                            GetHpIds().Contains((int)item.Id)))// &&
            //PennyJinx.IsMenuEnabled(((int)item.Id).ToString())))
            {
                var currentPriority = Potions.First(it => it.ItemId == Item.Id).Priority;
                if (currentPriority <= lastPriority)
                {
                    potSlot = Item.SpellSlot;
                }
            }
            return potSlot;
        }

        private static List<int> GetHpIds()
        {
            var HPIds = new List<int>();
            foreach (var pot in Potions)
            {
                if (pot.Type == PotionType.Health || pot.Type == PotionType.Flask && Items.HasItem((int)pot.ItemId))
                {
                    HPIds.Add((int)pot.ItemId);
                }
            }
            return HPIds;
        }

    }
    #endregion
}
#endregion