using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace UnderratedAIO
{
    class Program
    {
        public static Obj_AI_Hero player = ObjectManager.Player;
        public static float version = 1.1f;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad(EventArgs args)
        {
            try
            {
                if (Activator.CreateInstance(null, "UnderratedAIO.Champions." + player.ChampionName) != null)
                {
                }
            }
            catch
            {
            }
        }
    }
}
