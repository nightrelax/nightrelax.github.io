using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
namespace GNomeProject.Hero
{
    public partial class Diana
    {
        #region DECLARE
        public static string _GVersao = "Gn Diana";
        public static string _Me = "Diana";
        public static Obj_AI_Hero _Player { get { return ObjectManager.Player; } }
        public static Menu _Menu;
        public static Orbwalking.Orbwalker _Orbwalker;
        public static readonly List<Spell> _SpellList = new List<Spell>();
        public static readonly List<SpellSlot> _SumList = new List<SpellSlot>();
        public static SpellSlot _Incendiar;
        public static SpellSlot _Flash;
        public static Spell _Q, _W, _E, _R, _R1;
        
        #endregion Game_OnGameLoad

        public static void Game_OnGameLoad(EventArgs args)
        {
                
            #region Load Menu
            _Menu = new Menu("Gnome Diana", "gnomediana", true);
            _Menu.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
            _Orbwalker = new Orbwalking.Orbwalker(_Menu.SubMenu("orbwalker"));

            _Menu.AddSubMenu(new Menu("Drawing", "draw"));
            _Menu.SubMenu("draw").AddItem(new MenuItem("drawenabled", "Drawing Enabled").SetValue(true));
            _Menu.SubMenu("draw").AddItem(new MenuItem("aa", "Attack").SetValue(true));
            _Menu.SubMenu("draw").AddItem(new MenuItem("qq", "Spell Q").SetValue(true));
            _Menu.SubMenu("draw").AddItem(new MenuItem("ee", "Spell E").SetValue(true));
            _Menu.SubMenu("draw").AddItem(new MenuItem("rr", "Spell R").SetValue(true));
            _Menu.SubMenu("draw").AddItem(new MenuItem("r1", "Spell Flash Q R").SetValue(true));
            _Menu.SubMenu("draw").AddItem(new MenuItem("qc", "Circle Q").SetValue(true));

            _Menu.AddSubMenu(new Menu("Farm", "farm"));
            _Menu.SubMenu("farm").AddItem(new MenuItem("useq", "Use Spell Q").SetValue(true));

            _Menu.AddSubMenu(new Menu("Haras", "haras"));
            _Menu.SubMenu("haras").AddItem(new MenuItem("usq", "Use Spell Q").SetValue(true));

            _Menu.AddSubMenu(new Menu("AntiGapcloser", "gapcloser"));
            _Menu.SubMenu("gapcloser").AddItem(new MenuItem("gapusee", "Use E").SetValue(true));

            //_Menu.AddSubMenu(new Menu("Spell Level", "spelllevel"));

            _Menu.AddSubMenu(new Menu("Use combo", "usespellcombo"));
            _Menu.SubMenu("usespellcombo").AddItem(new MenuItem("user2", "Use Rx2").SetValue(false));
            _Menu.SubMenu("usespellcombo").AddItem(new MenuItem("useincendiar", "Use ignite").SetValue(true));
            _Menu.SubMenu("usespellcombo").AddItem(new MenuItem("user2kill", "Use Rx2 IF Kill").SetValue(true));
            _Menu.SubMenu("usespellcombo").AddItem(new MenuItem("userpass", "Use R IF Q Passive").SetValue(true));

            _Menu.AddSubMenu(new Menu("Potion Manager","potionmanager"));
            _Menu.SubMenu("potionmanager").AddItem(new MenuItem("potionhealth","Health %").SetValue(new Slider(30,10,50)));
            _Menu.SubMenu("potionmanager").AddItem(new MenuItem("potionmana", "Mana %").SetValue(new Slider(30, 10, 50)));


            _Menu.AddSubMenu(new Menu("Misc", "misc"));
            _Menu.SubMenu("misc").AddItem(new MenuItem("autolevel", "Auto Level Spell").SetValue(true));
            _Menu.SubMenu("misc").AddItem(new MenuItem("autoks", "Ks Kill").SetValue(true));
            _Menu.SubMenu("misc").AddItem(new MenuItem("autowdmg", "Auto W Shield").SetValue(true));

            _Menu.AddToMainMenu();
            #endregion

            #region Load Spells
            _Q = new Spell(SpellSlot.Q, 900f);
            _Q.SetTargetted(0.15f, 1500f);
            _W = new Spell(SpellSlot.W);
            _E = new Spell(SpellSlot.E);
            _R = new Spell(SpellSlot.R, 900f);
            _R.SetSkillshot(0.15f, 0.75f, 1500f, false, SkillshotType.SkillshotCircle);
            _R1 = new Spell(SpellSlot.Unknown, 900f + 400f);
            _R1.SetSkillshot(0.15f, 0.75f, 1500f, false, SkillshotType.SkillshotCircle);
            _Incendiar = _Player.GetSpellSlot("SummonerDot");
            _Flash = _Player.GetSpellSlot("SummonerFlash");
            _SpellList.Add(_Q);
            _SpellList.Add(_W);
            _SpellList.Add(_E);
            _SpellList.Add(_R);
            _SpellList.Add(_R1);
            _SumList.Add(_Incendiar);
            _SumList.Add(_Flash);

            //Eventos
            Game.OnGameUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;

            Console.WriteLine("Diana Load . . .");
            Game.PrintChat("<font color='#FFFF00'>#Hi  </font><font color='#35DE4C'>" + _GVersao + "</font> Loaded#");


            #endregion
        }

        #region Eventos
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (_Menu.Item("drawenabled").GetValue<bool>() == false || _Player.IsDead == true) return;

            if (_Menu.Item("aa").GetValue<bool>()) 
            {
                if (_EnemyRange(_Player.ServerPosition, 210f) >= 1) {Drawing.DrawCircle(_Player.Position,210f,Color.DarkRed); }
                else { Drawing.DrawCircle(_Player.Position, 210f, Color.DarkSlateGray); }
            }

            if(_Menu.Item("qq").GetValue<bool>())
            {
                if (_EnemyRange(_Player.ServerPosition, 900f) >= 1) { Drawing.DrawCircle(_Player.Position,900f,Color.DarkRed) ;}
                else { Drawing.DrawCircle(_Player.Position, 900f, Color.DarkSlateGray); }
            }

            if(_Menu.Item("ee").GetValue<bool>())
            {
                if (_EnemyRange(_Player.ServerPosition, 450f) >= 1) { Drawing.DrawCircle(_Player.Position, 450f, Color.DarkRed); }
                else { Drawing.DrawCircle(_Player.Position,450f,Color.DarkSlateGray);}
            }

            if(_Menu.Item("rr").GetValue<bool>())
            {
                if (_EnemyRange(_Player.ServerPosition, 830f) >= 1) { Drawing.DrawCircle(_Player.Position, 830f, Color.DarkRed); }
                else { Drawing.DrawCircle(_Player.Position, 830, Color.DarkSlateGray); }
            }

            if(_Menu.Item("qc").GetValue<bool>())
            {
                //Vector4 _MyPosHero = new Vector4(_Player.Position.To2D(), _Player.Position.To2D().X, _Player.Position.To2D().Y);
                //Drawing.DrawCircle(Game.CursorPos, 210f, Color.DarkSlateGray);
                //var _endline = new Vector2(_Player.Position.To2D().X + 400, _Player.Position.To2D().Y + 400);
                //Drawing.DrawLine(_Player.Position.To2D(), _endline, 1, Color.Red);
            }
        }

        public static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {

        }
        #endregion

        #region Misc
        public static int _EnemyRange(Vector3 _pos , float _range)
        {
            return
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero.Team != ObjectManager.Player.Team)
                    .Count(hero => Vector3.Distance(_pos, hero.ServerPosition) <= _range);
        }       
        #endregion
    }
}
