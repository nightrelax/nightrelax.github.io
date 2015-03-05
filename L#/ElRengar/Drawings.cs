using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;


namespace ElRengar
{
    public class Drawings
    {
        public static void Drawing_OnDraw(EventArgs args)
        {
            if(ElRengarMenu._menu.Item("ElRengar.Draw.off").GetValue<bool>())
                return;

            if(ElRengarMenu._menu.Item("ElRengar.Draw.Q").GetValue<bool>())
                if (Rengar.spells[Spells.Q].Level > 0)
                    Utility.DrawCircle(Rengar.Player.Position, Rengar.spells[Spells.Q].Range, Rengar.spells[Spells.Q].IsReady() ? Color.Green : Color.Red);

            if(ElRengarMenu._menu.Item("ElRengar.Draw.W").GetValue<bool>())
                if (Rengar.spells[Spells.W].Level > 0)
                    Utility.DrawCircle(Rengar.Player.Position, Rengar.spells[Spells.W].Range, Rengar.spells[Spells.W].IsReady() ? Color.Green : Color.Red);

            if(ElRengarMenu._menu.Item("ElRengar.Draw.E").GetValue<bool>())
                if (Rengar.spells[Spells.E].Level > 0)
                    Utility.DrawCircle(Rengar.Player.Position, Rengar.spells[Spells.E].Range, Rengar.spells[Spells.E].IsReady() ? Color.Green : Color.Red);

            if(ElRengarMenu._menu.Item("ElRengar.Draw.R").GetValue<bool>())
                if (Rengar.spells[Spells.R].Level > 0)
                    Utility.DrawCircle(Rengar.Player.Position, Rengar.spells[Spells.R].Range, Rengar.spells[Spells.R].IsReady() ? Color.Green : Color.Red);

        }
    }
}