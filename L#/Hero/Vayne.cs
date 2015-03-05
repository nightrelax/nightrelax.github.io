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
    public partial class Vayne
    {
        #region Declare
        public static string _GVersao = "GN Vayne";
        public static string _Me = "Vayne";
        public static Obj_AI_Hero _Player { get { return ObjectManager.Player; } }
        public static Menu _Menu;
        public static Orbwalking.Orbwalker _Orbwalker;

        public static readonly List<Spell> _SpellList = new List<Spell>();
        public static readonly List<SpellSlot> _SumList = new List<SpellSlot>();
        public static SpellSlot _Barreira;
        public static SpellSlot _Curar;
        public static SpellSlot _Flash;
        public static Spell _Q, _W, _E, _R;

        //public static TargetSelector _NewTarget;// = new TargetSelector();
        //private static int[] _VayneLevel = { };
        #endregion

        public static void Game_OnGameLoad(EventArgs args)
        {
            Console.WriteLine("Vayne Carregando . . .");
            CarregarMenu();
            CarregarSpell();
            Game.OnGameUpdate +=Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.PrintChat("<font color='#FFFF00'>#Hi  </font><font color='#35DE4C'>" + _GVersao + "</font> Loaded#");
        }

        public static void Drawing_OnDraw(EventArgs args)
        {

          
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(_Player.AttackRange, TargetSelector.DamageType.Magical);
           
            switch (_Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(target);
                    break;
                
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harrass();
                    break;
            }
            
        }

        private static void Harrass()
        {

        }

        public static void Combo(Obj_AI_Base _target)
        {

            _R.Cast();
            _Q.CastOnUnit(_target, false);
            
            /*target.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2); */
            //if(_target.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2)
            //{
            //    _Q.Cast(Game.CursorPos, false);
            //}

        }

        #region Menu
        public static void CarregarMenu()
        {
            _Menu = new Menu("GN Vayne", "gnvayne",true);

                var _TargetSelector = new Menu("Target Selector", "targetselector");
                TargetSelector.AddToMenu(_TargetSelector);
                _Menu.AddSubMenu(_TargetSelector);

            _Menu.AddSubMenu(new Menu("Drawing", "drawing"));
                _Menu.SubMenu("drawing").AddItem(new MenuItem("draw", "Draw Enabled").SetValue(true));
                _Menu.SubMenu("drawing").AddItem(new MenuItem("drawaa","Auto Atk").SetValue(true));
                _Menu.SubMenu("drawing").AddItem(new MenuItem("drawq","Spell Q").SetValue(true));
                _Menu.SubMenu("drawing").AddItem(new MenuItem("drawe", "Spell E").SetValue(true));


           _Menu.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
                _Orbwalker = new Orbwalking.Orbwalker(_Menu.SubMenu("orbwalker"));
                _Menu.SubMenu("orbwalker").AddItem(new MenuItem("flee", "Flee").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Press)));

          _Menu.AddToMainMenu();  
        }
        #endregion

        #region Spells
        public static void CarregarSpell()
        {
            _Q = new Spell(SpellSlot.Q);
            _W = new Spell(SpellSlot.W);
            _E = new Spell(SpellSlot.E,590f);
            _E.SetTargetted(0.15f, 1500f);
            _R = new Spell(SpellSlot.R);
        }
        #endregion

        #region
      


        #endregion
    }
}
