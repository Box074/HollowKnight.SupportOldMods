using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HKLab
{
    [Serializable]
    public class Config
    {
        public static readonly float CurrentVersion = 4.31f;
        [JsonIgnore]
        internal float OldVersion = -1;
        public float Version { get; set; } = -1;
        public bool AutoIgnoreBrokenMods { get; set; } = true;
		public bool TestMode { get; set; } = false;
        public Dictionary<string, float> IgnoreOldMods { get; set; } = new();
    }
}
