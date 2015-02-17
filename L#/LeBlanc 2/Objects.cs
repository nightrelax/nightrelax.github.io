using System;
using LeagueSharp;
using SharpDX;

namespace LeBlanc_2
{
    internal class Objects
    {
        public static class SecondW
        {
            public static GameObject Object { get; set; }
            public static Vector3 Pos { get; set; }
            public static double ExpireTime { get; set; }
        }
        public static class Clone
        {
            public static GameObject Object { get; set; }
            public static Vector3 Pos { get; set; }
            public static double ExpireTime { get; set; }
        }

        public static void Init()
        {
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("LeBlanc_Base_W_return_indicator.troy")))
            {
                return;
            }
            SecondW.Pos = new Vector3(0, 0, 0);
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("LeBlanc_Base_W_return_indicator.troy") && !sender.IsMe)
            {
                SecondW.Object = sender;
                SecondW.Pos = sender.Position;
                SecondW.ExpireTime = Game.Time + 4;
            }
            if (sender.Name.Contains("LeBlanc_MirrorImagePoff.troy") && sender.IsMe)
            {
                Clone.Object = sender;
                Clone.Pos = sender.Position;
                Clone.ExpireTime = Game.Time + 8;
            }
        }
    }
}
