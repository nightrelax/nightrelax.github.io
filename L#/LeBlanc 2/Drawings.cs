using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace LeBlanc_2
{
    internal class Drawings
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static void Init()
        {
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || args == null)
                return;

            var q = Configs.LeBlancConfig.Item("drawQ").GetValue<Circle>();
            var w = Configs.LeBlancConfig.Item("drawW").GetValue<Circle>();
            var e = Configs.LeBlancConfig.Item("drawE").GetValue<Circle>();
            var wts = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var timer = (Objects.SecondW.ExpireTime - Game.Time > 0) ? (Objects.SecondW.ExpireTime - Game.Time) : 0;

            if (Objects.SecondW.Pos != new Vector3(0, 0, 0))
            {
                Drawing.DrawText(wts.X - 35, wts.Y + 10, Color.White, "Second W: " + timer.ToString("0.0"));
            }

            if (Objects.SecondW.Pos != new Vector3(0, 0, 0))
            {
                Render.Circle.DrawCircle(Objects.SecondW.Pos, 100, Color.Red, 50);
            }

            if (q.Active)
                Render.Circle.DrawCircle(Player.Position, Spells.Q.Range, q.Color);
            if (w.Active)
                Render.Circle.DrawCircle(Player.Position, Spells.W.Range, w.Color);
            if (e.Active)
                Render.Circle.DrawCircle(Player.Position, Spells.E.Range, e.Color);

            if (Configs.LeBlancConfig.Item("haraKey").GetValue<KeyBind>().Active)
                Drawing.DrawText(Player.HPBarPosition.X + 40, Player.HPBarPosition.Y - 15, Color.Red, "Harass On");
        }
    }
}
