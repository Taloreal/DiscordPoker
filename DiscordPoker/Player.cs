using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordPoker {

    public class Player {

        public string Name { get; private set; }
        public ulong ID { get; private set; }
        public double Cash { get; private set; }
        public DateTime LastActive { get; private set; }
        public List<Card> Hand = new List<Card>();

        public Player(string playerName, ulong playerID) {
            Name = playerName;
            ID = playerID;
        }

        public void Give(double money) {
            Cash += money;
            Save();
        }

        public void Take(double money) {
            Cash -= money;
            Save();
        }

        public void Save() {
            try {
                Directory.CreateDirectory("Players\\");
                StreamWriter sw = new StreamWriter(@"Players\Player" + ID.ToString() + ".plr");
                sw.WriteLine(Name);
                sw.WriteLine(ID.ToString());
                sw.WriteLine(Cash.ToString());
                sw.WriteLine(LastActive.ToString());
                sw.Close();
            } catch (Exception E) { }
        }

        public static Player Load(ulong id) {
            Player p = null;
            try {
                StreamReader sr = new StreamReader(@"Players\Player" + id.ToString() + ".plr");
                string name = sr.ReadLine();
                if (!ulong.TryParse(sr.ReadLine(), out ulong tag) || id != tag) {
                    sr.Close();
                    throw new Exception("ERROR: ID is invalid.");
                }
                if (!double.TryParse(sr.ReadLine(), out double cash)) {
                    sr.Close();
                    throw new Exception("ERROR: Cash is invalid.");
                }
                if (!DateTime.TryParse(sr.ReadLine(), out DateTime lactive)) {
                    sr.Close();
                    throw new Exception("ERROR: Last active date is invalid.");
                }
                p = new Player(name, id);
                p.LastActive = lactive;
                p.Cash = cash;
                sr.Close();
            }
            catch (Exception e) {  }
            return p;
        }

        public static List<Player> Remember_Players() {
            List<Player> players = new List<Player>();
            if (!Directory.Exists("Players\\")) { return players; }
            string[] files = Directory.GetFiles("Players\\");
            for (int i = 0; i < files.Length; i++) {
                if (!files[i].StartsWith("Players\\Player")) { continue; }
                if (!files[i].EndsWith(".plr")) { continue; }
                string id = files[i].Replace("Players\\Player", "").Replace(".plr", "");
                if (!ulong.TryParse(id, out ulong playerID)) { continue; }
                Player p = Load(playerID);
                if (p == null) { continue; }
                players.Add(p);
            }
            return players;
        }
    }
}
