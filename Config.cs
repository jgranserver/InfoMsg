using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FeatureInfo
{
    public class Infos
    {
        public string Command = "getinfo";
        public string Reload = "inforeload";
        
        public class InfoMsg
        {
            public string info { get; set; }
            public string msg { get; set; }
        }

        public List<InfoMsg> GetInfoMsgs = new List<InfoMsg>();

        public static Infos Read(string configpath)
        {
            if (!File.Exists(configpath))
            {
                Infos infos = new Infos();

                infos.GetInfoMsgs.Add(new InfoMsg()
                {
                    info = "keyword",
                    msg = "information send for this keyword"
                });
                
                File.WriteAllText(configpath, JsonConvert.SerializeObject(infos, Formatting.Indented));
                return infos;
            }
            return JsonConvert.DeserializeObject<Infos>(File.ReadAllText(configpath));
        }
    }
}
