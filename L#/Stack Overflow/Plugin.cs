#region

using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Stack_Overflow.Utilitarios;

#endregion

namespace Stack_Overflow
{
    public abstract class Plugin
    {


        protected Plugin()
        {
            CriarMenu();

            Utility.HpBarDamageIndicator.DamageToUnit = DamageToUnit;
            Utility.HpBarDamageIndicator.Enabled = true;

            PrintChat("Carregando, Criado por ptr0x e Mr Articuno");
            PrintChat("Made by ptr0x and Mr Articuno");

        }

        public Menu Menu { get; internal set; }
        public Orbwalking.Orbwalker Orbwalker { get; internal set; }

        public Orbwalking.OrbwalkingMode OrbwalkerMode
        {
            get { return Orbwalker.ActiveMode; }
        }

        public bool Packets
        {
            get { return false; }
        }

        public Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        private float DamageToUnit(Obj_AI_Hero hero)
        {
            return GetComboDamage(hero);
        }

        private void CriarMenu()
        {
            Menu = new Menu("Stack OverFlow", "stackoverflow", true);

            var tsMenu = new Menu("Target Selector", "stackoverflowTS");
            TargetSelector.AddToMenu(tsMenu);
            Menu.AddSubMenu(tsMenu);

            var orbwalkMenu = new Menu("Orbwalker", "stackoverflowOrbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            Menu.AddSubMenu(orbwalkMenu);

            var comboMenu = new Menu("Combo", "stackoverflowCombo");
            Combo(comboMenu);
            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass", "stackoverflowHarass");
            Harass(harassMenu);
            Menu.AddSubMenu(harassMenu);

            var itemsMenu = new Menu("Items", "stackoverflowItems");
            ItemMenu(itemsMenu);
            Menu.AddSubMenu(itemsMenu);

            var miscMenu = new Menu("Misc", "stackoverflowMisc");
            miscMenu.AddItem(new MenuItem("packets", "Use packets / Pacotes").SetValue(true));
            Misc(miscMenu);
            Menu.AddSubMenu(miscMenu);

            var itemMenu = new Menu("Items and Summoners", "Items");
            ItemManager.AddToMenu(itemMenu);
            Menu.AddSubMenu(itemMenu);


            if (Player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown)
            {
                var igniteMenu = new Menu("Ignite/Incendiar", "stackoverflowIgnite");
                new AutoIgnite().Load(igniteMenu);
                Menu.AddSubMenu(igniteMenu);
            }

            var pmUtilitario = new Menu("Potion Control/Controle", "stackoverflowPM");
            new PotionUtilitario().Load(pmUtilitario);
            Menu.AddSubMenu(pmUtilitario);

            var drawingMenu = new Menu("Drawings / Desenhos", "stackoverflowDrawing");
            Drawings(drawingMenu);
            Menu.AddSubMenu(drawingMenu);

            Menu.AddToMainMenu();
        }

        public static void PrintChat(string msg)
        {
            Game.PrintChat("<font color='#07c7dd'>Stack Overflow:</font> <font color='#FFFFFF'>" + msg + "</font>");
        }

        public T GetValue<T>(string name)
        {
            return Menu.Item(name).GetValue<T>();
        }

        public bool GetBool(string name)
        {
            return GetValue<bool>(name);
        }

        public virtual float GetComboDamage(Obj_AI_Hero target)
        {
            return 0;
        }

        public Spell GetSpell(List<Spell> spellList, SpellSlot slot)
        {
            return spellList.First(x => x.Slot == slot);
        }

        #region Virtuals

        public virtual void Combo(Menu config)
        {
        }

        public virtual void Harass(Menu config)
        {
        }

        public virtual void ItemMenu(Menu config)
        {
        }

        public virtual void Misc(Menu config)
        {
        }

        public virtual void Drawings(Menu config)
        {
        }

        #endregion Virtuals
    }
}