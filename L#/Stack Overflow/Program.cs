#region

using System;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Stack_Overflow
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            var plugin = Type.GetType("Stack_Overflow.Champions." + ObjectManager.Player.ChampionName);

            if (plugin == null)
            {
                Plugin.PrintChat(ObjectManager.Player.ChampionName + " not supported / não suportado");
                return;
            }

            Activator.CreateInstance(plugin);
        }

        private static void CurrentDomainOnUnhandledException(object sender,
            UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Console.WriteLine(((Exception) unhandledExceptionEventArgs.ExceptionObject).Message);
            Plugin.PrintChat("Fatal Error please report on forum / Erro critico por favor avise no fórum");
        }
    }
}