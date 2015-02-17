using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Trundle
{
    class OutgoingDamage
    {
        public static bool IsMovingToMe(Obj_AI_Hero target)
        {
            if (target.Path.Count() > 0)
            {
                var targetPath = target.Path[0].To2D();
                if (ObjectManager.Player.Distance(target) > ObjectManager.Player.Distance(targetPath))
                {
                    return true;
                }
            }
            else if (!target.IsMoving)
            {
                return true;
            }
            return false;
        }

    }
}
