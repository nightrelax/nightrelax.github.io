using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using ConsoleApplication12.Properties;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;


namespace ConsoleApplication12
{
    internal class Program
    {
        public static Boolean doublepress;
        public static float lastsay, lastclick;
        public static Vector2 lastpositon;
        public static Menu SidebarMenu, Leftbar, SummonerW;
        private static readonly IList<enemies> enemyList = new List<enemies>();
        private static string _version;
        private static Sprite Sprite;
        private static Obj_AI_Hero hero { get { return ObjectManager.Player; } }
        private static float x, y;
        public static SpellSlot[] SummonerSpellSlots = { ((SpellSlot)4), ((SpellSlot)5) };
        private static float[] respawntime = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static string[] SummonersNames =
{
"summonerbarrier", "summonernoost", "summonerclairvoyance",
"summonerdot", "summonerexhaust", "summonerflash", "summonerhaste", "summonerheal", "summonermana",
"summonerodingarrison", "summonerrevive", "summonersmite", "summonerteleport"
};
        public static string[] NoEnergie =
{
"Aatrox", "DrMundo", "Vladimir",
"Zac","Katarina","Garen","Riven" 
};

        public static string[] Energie =
        {
            "Akali", "Kennen", "LeeSin", "Shen", "Zed","Gnar","Katarina","RekSai","Renekton","Rengar","Rumble",
        };

        private static int hpwidth = 0;
        private static float Height = Drawing.Height;
        private static float Width = Drawing.Width;
        private static int scale = 1;
        static SharpDX.Direct3D9.Font small, respawnfont, medium;
        private static Texture HUD, HUDult, hpTexture, manaTexture, blackTexture, energieTexture;
        private static Texture temp, summonerheal, summonerbarrier, summonerboost, summonerclairvoyance, summonerdot, summonerexhaust, summonerflash, summonerhaste, summonermana, summonerodingarrison, summonerrevive, summonersmite, summonerteleport;
        private static void Main(string[] args)
        {
            #region load images/fonts
            Sprite = new Sprite(Render.Device);
            HUDult = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.HUDult, typeof(byte[])), 16, 16, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            blackTexture = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.schwarz, typeof(byte[])), 62 + 24 + 10, 90, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            HUD = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.HUDtest, typeof(byte[])), 62 + 24 + 10, 90, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            hpTexture = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.HPbar, typeof(byte[])), 58, 10, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            manaTexture = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.MANAbar, typeof(byte[])), 58, 10, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            energieTexture = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.Energiebar, typeof(byte[])), 58, 10, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            #region summeners
            summonerheal = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerHeal, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerbarrier = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerBarrier, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerboost = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerBoost, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerclairvoyance = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerClairvoyance, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerdot = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerDot, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerflash = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerFlash, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerhaste = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerHaste, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerexhaust = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerExhaust, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonermana = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerMana, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerodingarrison = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerOdinGarrison, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerrevive = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerRevive, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonersmite = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerSmite, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            summonerteleport = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Resources.SummonerTeleport, typeof(byte[])), 24, 480, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

            #endregion
            small = new SharpDX.Direct3D9.Font(Drawing.Direct3DDevice, new FontDescription()
            {
                FaceName = "Verdana",
                Height = 10,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });
            medium = new SharpDX.Direct3D9.Font(Drawing.Direct3DDevice, new FontDescription()
            {
                FaceName = "Verdana",
                Height = 16,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });
            respawnfont = new SharpDX.Direct3D9.Font(Drawing.Direct3DDevice, new FontDescription()
            {
                FaceName = "Verdana",
                Height = 40,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });
            #endregion
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Print(String text)
        {
            Game.PrintChat("<font color='#ff3232'>Sidebar: </font> <font color='#FFFFFF'>" + text + "</font>");
        }

        private static void Game_OnGameLoad(EventArgs args)
        {

            SidebarMenu = new Menu("sidebar", "sidebar", true);
            Leftbar = new Menu("Allie Sidebar", "Left sidebar", false);
            SummonerW = new Menu("write Summeners", "summener");
            Leftbar.AddItem(new MenuItem("Activate2", "Activate")).SetValue(true);
            Leftbar.AddItem(new MenuItem("offX5", "Offset for width").SetValue(new Slider(0, -80, 80)));
            Leftbar.AddItem(new MenuItem("offY5", "Offset for height").SetValue(new Slider(0, -200, 200)));
            Leftbar.AddItem(new MenuItem("Distance", "Distance").SetValue(new Slider(0, -50, 50)));
            SidebarMenu.AddSubMenu(Leftbar);
            SidebarMenu.AddItem(new MenuItem("Activate", "Activate")).SetValue(true);
            SidebarMenu.AddItem(new MenuItem("Activate4", "Only draw visible enemies")).SetValue(true);
            SummonerW.AddItem(new MenuItem("hootkey", "combo hotkey").SetValue(new KeyBind(20, KeyBindType.Press)));
            SummonerW.AddItem(new MenuItem("challenger", "challenger mod").SetValue(false));
            SummonerW.AddItem(new MenuItem("Activate3", "Activate").SetValue(true));
            SidebarMenu.AddSubMenu(SummonerW);
            SidebarMenu.AddToMainMenu();

            var attempt = 0;
            _version = GameVersion();
            while (string.IsNullOrEmpty(_version) && attempt < 5)
            {
                _version = GameVersion();
                Print("attempt: " + attempt);
                attempt++;
            }//funtzt
            if (!string.IsNullOrEmpty(_version))
            {
                Thread newThread = new Thread(LoadImages);
                newThread.IsBackground = true;
                newThread.Start();
                Print("Loaded! ");
                Drawing.OnPreReset += DrawingOnPreReset;
                Drawing.OnPostReset += DrawingOnPostReset;
                Drawing.OnEndScene += Drawing_EndScene;
                AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
                Game.OnWndProc += Game_OnWndProc;
            }
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs e)
        {
            Sprite.Dispose();
            small.Dispose();
            medium.Dispose();
            respawnfont.Dispose();
        }

        static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }
            if (Game.Time-lastclick <0.5&&lastpositon== Game.CursorPos.To2D())
            {
                doublepress = true;
            }
            else
            {
                doublepress = false;
            }
            lastclick = Game.Time;
             lastpositon= Game.CursorPos.To2D();
        }



        private static void DrawingOnPostReset(EventArgs args)
        {
           
            small.OnResetDevice();
            medium.OnResetDevice();
            respawnfont.OnResetDevice();
            Sprite.OnResetDevice();
        }
        private static void DrawingOnPreReset(EventArgs args)
        {
            Sprite.OnLostDevice();
            small.OnLostDevice();
            medium.OnLostDevice();
            respawnfont.OnResetDevice();
        }

        static Texture summonerTexture(string spellname)
        {
            switch (spellname.ToLower())
            {//shitty code but im to tired
                case "summonerodingarrison":
                    return summonerodingarrison;
                case "summonerrevive":
                    return summonerrevive;
                case "summonerclairvoyance":
                    return summonerclairvoyance;
                case "summonerboost":
                    return summonerboost;
                case "summonermana":
                    return summonermana;
                case "summonerteleport":
                    return summonerteleport;
                case "summonerheal":
                    return summonerheal;
                case "summonerexhaust":
                    return summonerexhaust;
                case "summonerdot":
                    return summonerdot;
                case "summonerhaste":
                    return summonerhaste;
                case "summonerflash":
                    return summonerflash;
                case "summonerbarrier":
                    return summonerbarrier;
                default:
                    return summonersmite;
            }

        }
        static void Drawing_EndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                return;
            try
            {
                #region right
                if (SidebarMenu.Item("Activate").GetValue<bool>()) //drawHUD
                {

                    x = -Width + ((62 + 24 + 10) * scale);
                    y = Height * -.10f;
                    int zahler = 0;
                    foreach (var enemie in enemyList)
                    {
                        x = x - 10;
                        int z = 0;
                        foreach (var sSlot in SummonerSpellSlots)    
                        {
                            var spell = enemie.Hero.Spellbook.GetSpell(sSlot);

                            var t = spell.CooldownExpires - Game.Time;
                            var percent = (Math.Abs(spell.Cooldown) > float.Epsilon) ? t / spell.Cooldown : 1f;
                            var n = (t > 0) ? (int)(19 * (1f - percent)) : 19;
                            var ts = TimeSpan.FromSeconds((int)t);
                            var s = t > 60 ? string.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds) : String.Format("{0:0}", t);
                            if (t > 0)
                            {

                                medium.DrawText(
                                    null, s, Convert.ToInt32(-x - 4 - s.Length*7), Convert.ToInt32(-y + 7 + z),
                                    new ColorBGRA(255, 255, 255, 255));


                           
                            if (SummonerW.Item("Activate3").GetValue<bool>())
                            {
                                var t2 = spell.CooldownExpires;
                                var ts2 = TimeSpan.FromSeconds((int)t2);

                                var s2 = t > 60
                                    ? string.Format("{0}{1:D2}", ts2.Minutes, ts2.Seconds)
                                    : String.Format("{0:0}", t);
                                
                                SharpDX.RectangleF summoner = new SharpDX.RectangleF(-(x - 2), -(y - 7 - z), 24, 24);
                        //        SharpDX.RectangleF icon = new SharpDX.RectangleF(-(x - 23 - 5), -(y - 8), 55, 55);
                                if (lastsay + 1 < Game.Time) //say it to the chat
                                {
                                   
                                    if (doublepress)
                                    {
                                        
              
                                        if (summoner.Contains(Utils.GetCursorPos()))
                                        {
                                            lastsay = Game.Time;
                                            doublepress = false;
                                            if (SummonerW.Item("challenger").GetValue<bool>())
                                            {

                                                Game.Say(nickname(enemie.Hero.BaseSkinName) + " " +
                                                         realSummoner(spell.Name) + " " + s2);
                                            }
                                            else
                                            {
                                                Game.Say(nickname(enemie.Hero.BaseSkinName) + "has no" +
                                                         realSummoner(spell.Name));
                                            }

                                        }
                                 //  if (icon.Contains(Utils.GetCursorPos()))
                                  //      {
                                    //     lastsay = Game.Time;
                                      //    doublepress = false;
                                   
                           // Game.Say(nickname(enemie.Hero.BaseSkinName)+ " no summoner");
                              //          }
                                    }
                                 

                                }
                            }
                            }
                            Sprite.Begin();
                            Sprite.Draw(summonerTexture(spell.Name), new ColorBGRA(255, 255, 255, 255), new SharpDX.Rectangle(0, 24 * n, 24, 24), new Vector3(x - 2, y - 7 - z, 0));
                            Sprite.End();

                            z = 24;

                #endregion

                        }
                        doublepress = false;
                        x = x - 23;//fix wege i ha falsch agfange


                        Sprite.Begin(); //DRAW icon 255, 255, 255, 255
                        Sprite.Draw(enemie.Icon, new ColorBGRA(255, 255, 255, 255), null, new Vector3(x - 5, y - 8, 0), null);
                        Sprite.End();

                        if (enemie.Hero.IsDead && respawntime[zahler] < Game.ClockTime)
                        {
                            respawntime[zahler] = Game.ClockTime + enemie.Hero.DeathDuration;
                            //todo get respawn timer
                        }
                        else if (enemie.Hero.IsDead && (respawntime[zahler] > Game.ClockTime))
                        {
                            String timetorespawn = (Math.Round(respawntime[zahler] - Game.ClockTime)).ToString();
                            if (timetorespawn.Length == 1)
                            {
                                respawnfont.DrawText(null, timetorespawn, (int)x * -1 + 21, (int)y * -1 + 13, new ColorBGRA(248, 248, 255, 255));
                            }
                            else
                            {
                                respawnfont.DrawText(null, timetorespawn, (int)x * -1 + 10, (int)y * -1 + 13, new ColorBGRA(248, 248, 255, 255));
                            }
                        }


                        String HP = Math.Round(enemie.Hero.Health) + "/" + Math.Round(enemie.Hero.MaxHealth);

                        int hplength = ((58 - (HP.Length * 5)) / 2); //to center text
                        hpwidth = Convert.ToInt32(((58f / 100f) * (enemie.Hero.HealthPercentage())));
                        Sprite.Begin(); //DRAW HUD
                        // //ziel:-1617 / -124 //bild 1 -4/-26 55x55
                        x = x + 23 + 10;//fix wege i ha falsch agfange
                        Sprite.Draw(HUD, new ColorBGRA(255, 255, 255, 255), new SharpDX.Rectangle(1, 0, 62 + 23 + 10, 90), new Vector3(x, y, 0), null); //todo add % value for heigh 
                        x = x - 23 - 10;//fix wege i ha falsch agfange
                        Sprite.End();
                        // //draw level  weiss =    248-248-255
                        small.DrawText(null, enemie.Hero.Level.ToString(), (int)x * -1 + 48, (int)y * -1 + 52, new ColorBGRA(248, 248, 255, 255));


                        if (enemie.Hero.Spellbook.GetSpell(SpellSlot.R).CooldownExpires < Game.Time && enemie.Hero.Spellbook.GetSpell(SpellSlot.R).Level > 0)
                        {
                            Sprite.Begin();
                            Sprite.Draw(HUDult, new ColorBGRA(255, 255, 255, 255), null, new Vector3(x + -46, y + -2, 0), null);
                            Sprite.End();

                        }
                        if (!NoEnergie.Contains(enemie.Hero.ChampionName))
                        {
                            String Mana = Math.Round(enemie.Hero.Mana) + "/" + Math.Round(enemie.Hero.MaxMana);
                            int manawidth = Convert.ToInt32(((58f / 100f) * (enemie.Hero.ManaPercentage())));
                            int Manalength = ((58 - (Mana.Length * 5)) / 2); //to center t
                            //draw MANA /Manabar
                            Sprite.Begin();
                            if (!Energie.Contains(enemie.Hero.ChampionName))
                            {
                                Sprite.Draw(manaTexture, new ColorBGRA(255, 255, 255, 255), new SharpDX.Rectangle(0, 0, manawidth, 10), new Vector3(x - 2, y - 57 - 7 - 14, 0), null);
                            }
                            else
                            {
                                Sprite.Draw(energieTexture, new ColorBGRA(255, 255, 255, 255), new SharpDX.Rectangle(0, 0, manawidth, 10), new Vector3(x - 2, y - 57 - 7 - 14, 0),
                                    null);
                            }
                            Sprite.End();
                            small.DrawText(null, Mana, (int)x * -1 + 2 + Manalength, (int)y * -1 + 65 + 14,
                                new ColorBGRA(248, 248, 255, 255));
                        }

                        int minionlength = 0;

                        switch (enemie.Hero.MinionsKilled.ToString().Length)// zentriere
                        {
                            case 1:
                                minionlength = 15;
                                break;

                            case 2:
                                minionlength = 12;
                                break;

                            case 3:
                                minionlength = 10;
                                break;
                        }
                        x = x + 10 + 24;
                        small.DrawText(null, enemie.Hero.MinionsKilled.ToString(), (int)x * -1 + minionlength, (int)y * -1 + 62, new ColorBGRA(248, 248, 255, 255));
                        x = x - 10 - 24;
                        //draw HP/MAXHP 

                        Sprite.Begin();
                        Sprite.Draw(hpTexture, new ColorBGRA(255, 255, 255, 255), new SharpDX.Rectangle(0, 0, hpwidth, 10), new Vector3(x - 2, y - 57 - 7, 0), null);
                        Sprite.End();
                        small.DrawText(null, HP, (int)x * -1 + 2 + hplength, (int)y * -1 + 65, new ColorBGRA(248, 248, 255, 255));
                        if (!enemie.Hero.IsVisible || enemie.Hero.IsDead)//make it black :)
                        {
                            Sprite.Begin(); //DRAW icon 255, 255, 255, 255
                            Sprite.Draw(blackTexture, new ColorBGRA(255, 255, 255, 110), null, new Vector3(x + 24 + 10, y, 0), null);
                            Sprite.End();
                        }

                        if (enemie.Hero.Health < 350 && !enemie.Hero.IsDead)//eventuell && (int)hero.Spellbook.GetSpell(SpellSlot.R).SData.CastRange.GetValue(0) < 5000 && enemie.Hero.ServerPosition.Distance(hero.ServerPosition) < (int)hero.Spellbook.GetSpell(SpellSlot.R).SData.CastRange.GetValue(0) && hero.Spellbook.GetSpell(SpellSlot.R).Cooldown.Equals(0) && hero.Level >= 6)
                        {
                            //todo ping on  enemie.Hero.ServerPosition
                            {

                                //    LeagueSharp.Network..S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(enemie.Hero.Position.X, enemie.Hero.Position.Y,enemie.Hero.NetworkId,ObjectManager.Player.NetworkId, Packet.PingType.Danger)).Process();
                            }

                        }
                        x = x + 23 + 10;
                        y = y - 94;
                        zahler++;

                    }
                }

                #region leftsidebar
                if (Leftbar.Item("Activate2").GetValue<bool>())
                {
                    x = (Leftbar.Item("offX5").GetValue<Slider>().Value * -1) - 50;
                    y = Leftbar.Item("offY5").GetValue<Slider>().Value - 50;
                    foreach (Obj_AI_Hero herosHero in ObjectManager.Get<Obj_AI_Hero>().Where(herosHero => herosHero != null && herosHero.Team == ObjectManager.Player.Team && hero.IsValid && herosHero.Name != hero.Name))
                    {
                        int z = 0;
                        foreach (var sSlot in SummonerSpellSlots)    //Imeh again
                        {
                            var spell = herosHero.Spellbook.GetSpell(sSlot);

                            var t = spell.CooldownExpires - Game.Time;
                            var percent = (Math.Abs(spell.Cooldown) > float.Epsilon) ? t / spell.Cooldown : 1f;
                            var n = (t > 0) ? (int)(19 * (1f - percent)) : 19;
                            var ts = TimeSpan.FromSeconds((int)t);
                            var s = t > 60 ? string.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds) : String.Format("{0:0}", t);
                            if (t > 0)
                            {

                                medium.DrawText(
                                    null, s, Convert.ToInt32(-x + 30), Convert.ToInt32(-y + 10 + z),
                                    new ColorBGRA(255, 255, 255, 255));
                            }



                            Sprite.Begin();
                            Sprite.Draw(summonerTexture(spell.Name), new ColorBGRA(255, 255, 255, 255), new SharpDX.Rectangle(0, 24 * n, 24, 24), new Vector3(x - 2, y - 7 - z, 0));
                            Sprite.End();
                            z = 24;

                        }
                        y = y - 48 - Leftbar.Item("Distance").GetValue<Slider>().Value;
                    }
                }
                #endregion



            }
            catch
            {
                Console.Write("Sidebar crashed at drawing?");
            }
        }



        private static string nickname(string p)
        {
            switch (p)
            {
                case "Aatrox":
                    return "aatrox";
                   
                case "Akali":
                    return p.ToLower();

                case "Alistar":
                    return "al";
                case "Amumu":
                    return "mumu";
                case "Anivia":
                    return p.ToLower();
                case "Annie":
                    return p.ToLower();
                case "Brand":
                    return p.ToLower();
                case "Braum":
                    return p.ToLower();
                case "Cassiopeia":
                    return "cassio";
                case "Chogath":
                    return "cho";
                case "Corki":
                    return p.ToLower();
                case "Darius":
                    return "darius";
                case "Diana":
                    return p.ToLower();
                case "DrMundo":
                    return "mundo";
                case "Draven":
                    return p.ToLower();
                case "Elise":
                    return p.ToLower();
                case "Evelynn":
                    return "eve";
                case "Ezreal":
                    return "ez";
                case "FiddleSticks":
                    return "fiddle";
                case "Fiora":
                    return p.ToLower();

                case "Galio":
                    return p.ToLower();
                case "Gangplank":
                    return "gp";
                case "Garen":
                    return p.ToLower();
                case "Gragas":
                    return p.ToLower();
                case "Graves":
                    return p.ToLower();
                case "Heimerdinger":
                    return "heimer";
                case "Irelia":
                    return p.ToLower();
                case "Janna":
                    return p.ToLower();
                case "JarvanIV":
                    return "j4";
                case "Jayce":
                    return p.ToLower();
                case "Karma":
                    return p.ToLower();
                case "Karthus":
                    return p.ToLower();
                case "Kassadin":
                    return "kassa";
                case "Kayle":
                    return p.ToLower();
                case "Kennen":
                    return p.ToLower();
                case "Khazix":
                    return "kha";
                case "KogMaw":
                    return "kog";
                case "Leblanc":
                    return p.ToLower();
                case "LeeSin":
                    return "lee";
                case "Leona":
                    return p.ToLower();

                case "Lucian":
                    return p.ToLower();

                case "Maokai":
                    return p.ToLower();
                case "MasterYi":
                    return "yi";
                case "MissFortune":
                    return "mf";
                case "MonkeyKing":
                    return "wukong";
                case "Mordekaiser":
                    return "morde";
                case "Morgana":
                    return p.ToLower();
                case "Nasus":
                    return p.ToLower();

                case "Nocturne":
                    return "noc";

                case "Orianna":
                    return "ori";
                case "Pantheon":
                    return "panth";
                case "Poppy":
                    return p.ToLower();
                case "Quinn":
                    return p.ToLower();
                case "Rammus":
                    return p.ToLower();
                case "Renekton":
                    return "renek";
                case "Rengar":
                    return p.ToLower();
                case "Riven":
                    return p.ToLower();
                case "Rumble":
                    return p.ToLower();

                case "Shaco":
                    return p.ToLower();

                case "Singed":
                    return p.ToLower();

                case "Sivir":
                    return p.ToLower();
                case "Skarner":
                    return p.ToLower();
                case "Soraka":
                    return p.ToLower();
                case "Swain":
                    return p.ToLower();
                case "Syndra":
                    return p.ToLower();
                case "Talon":
                    return p.ToLower();
                case "Taric":
                    return p.ToLower();
                case "Teemo":
                    return p.ToLower();
                case "Thresh":
                    return p.ToLower();
                case "Tristana":
                    return "tris";

                case "Trundle":
                    return p.ToLower();
                case "Tryndamere":
                    return "trynda";
                case "TwistedFate":
                    return "tf";
                case "Twitch":
                    return "twitch";
                case "RekSai":
                    return "rek";
                case "Kalista":
                    return p.ToLower();
                case "Udyr":
                    return p.ToLower();
                case "Urgot":
                    return p.ToLower();
                case "Varus":
                    return p.ToLower();
                case "Vayne":
                    return p.ToLower();
                case "Veigar":
                    return p.ToLower();

                case "Viktor":
                    return "vik";
                case "Vladimir":
                    return "vlad";
                case "Volibear":
                    return "voli";
                case "Warwick":
                    return "ww"; break;
                case "Xerath":
                    return "xerath";
                case "XinZhao":
                    return "xin";

                case "Yorick":
                    return p.ToLower();

                case "Ziggs":
                    return "ziggs";
                case "Zilean":
                    return p.ToLower();
                case "Zyra":
                    return "zyra";
                default:
                    if (p.Length > 4)
                    {
                        return p.ToLower().Substring(0, 4);
                    }
                    return p.ToLower();

            }


        }

        private static string realSummoner(string p)
        {

            switch (p.ToLower())
            {//shitty code but im to tired
                case "summonerodingarrison":
                    return "garrison";
                case "summonerrevive":
                    return "revive";
                case "summonerclairvoyance":
                    return "clairvoyance";
                case "summonerboost":
                    return "cleanse";
                case "summonermana":
                    return "clarity";
                case "summonerteleport":
                    return "tp";
                case "summonerheal":
                    return "heal";
                case "summonerexhaust":
                    return "exhaust";
                case "summonersmite":
                    return "smite";
                case "summonerdot":
                    return "ignit";
                case "summonerhaste":
                    return "ghost";
                case "summonerbarrier":
                    return "barrier";
                case "summonerflash":
                    return "flash";
                default:
                    return "smite";

            }


        }

        //TheSaltyWaffle Universal Minimaphack (fetching Champion Icons)

        #region fetch images

        private static void LoadImages()
        {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero != null && hero.Team != ObjectManager.Player.Team && hero.IsValid))
            {
                LoadImage(hero);
            }
        }

        private static void LoadImage(Obj_AI_Hero hero)
        {
            Bitmap bmp = null;
            if (File.Exists(GetImageCached(hero.ChampionName)))
            {
                bmp = new Bitmap(GetImageCached(hero.ChampionName));//works like a charm
            }
            else
            {
                int attempt = 0;
                bmp = DownloadImage(hero.ChampionName);
                while (bmp == null && attempt < 5)
                {
                    bmp = DownloadImage(hero.ChampionName);

                    attempt++;

                }
                if (bmp == null)
                {

                    Game.PrintChat("Failed to load " + hero.ChampionName + " after " + attempt + 1 + " attempts!");
                }
                else
                {

                    bmp.Save(GetImageCached(hero.ChampionName));
                }
            }
            if (bmp != null)
            {
                var enemie = new enemies(hero, bmp);
                enemyList.Add(enemie);
            }
        }



        private static Bitmap DownloadImage(string champName)
        {
            WebRequest request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + _version + "/img/champion/" + champName +
                                  ".png");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                Stream responseStream;
                using (responseStream = response.GetResponseStream())
                {
                    return responseStream != null && responseStream != Stream.Null ? new Bitmap(responseStream) : null;
                }

            }
        }




        public static string GameVersion()
        {
            String json = new WebClient().DownloadString("http://ddragon.leagueoflegends.com/realms/euw.json");
            return (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(json)["v"];
        }

        public static string GetImageCached(string champName)
        {
            string path = Path.GetTempPath() + "Sidebar";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "\\" + _version;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path + "\\" + champName + ".png";
        }

        #endregion

        //end copy past

        private class enemies
        {
            public Obj_AI_Hero Hero { get; set; }
            public Texture Icon { get; set; }

            public enemies(Obj_AI_Hero hero, Bitmap bmp)
            {
                // TODO: Complete member initialization
                this.Hero = hero;

                this.Icon = Texture.FromMemory(Drawing.Direct3DDevice,
                    (byte[])new ImageConverter().ConvertTo(bmp, typeof(byte[])), 55, 55, 0, Usage.None, Format.A1,
                    Pool.Managed, Filter.Default, Filter.Default, 0);

            }


        }

        
    }
}


