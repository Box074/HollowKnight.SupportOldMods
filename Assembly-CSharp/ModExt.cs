using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKLab;

namespace Modding
{
    public abstract class Mod<TSaveSettings> : HKLab.Mod where TSaveSettings : ModSettings
    {
        public virtual TSaveSettings Settings
        {
            get
            {
                TSaveSettings result;
                if ((result = _settings) == null)
                {
                    if ((result = _settings = base.SaveSettings as TSaveSettings) == null)
                    {
                        result = _settings = Activator.CreateInstance<TSaveSettings>();
                        base.SaveSettings = result;
                    }
                }
                return result;
            }
            set
            {
                _settings = value;
                base.SaveSettings = value;
            }
        }
        internal override Type LocalSettingsType => typeof(TSaveSettings);
        private TSaveSettings _settings = null;

        public override ModSettings SaveSettings { get => Settings; set => Settings = (TSaveSettings)value; }

        public Mod(): base()
        {

        }
        public Mod(string name): base(name)
        {

        }
    }
    public abstract class Mod<TSaveSettings, TGlobalSettings> : Mod<TSaveSettings> where TSaveSettings : ModSettings
        where TGlobalSettings : ModSettings
    {
        public new virtual TGlobalSettings GlobalSettings
        {
            get
            {
                TGlobalSettings result;
                if ((result = _globalSettings) == null)
                {
                    if ((result = _globalSettings = base.GlobalSettings as TGlobalSettings) == null)
                    {
                        result = _globalSettings = Activator.CreateInstance<TGlobalSettings>();
                        base.GlobalSettings = result;
                    }
                }
                return result;
            }
            set
            {
                _globalSettings = value;
                base.GlobalSettings = value;
            }
        }

        public Mod() : base()
        {

        }
        public Mod(string name) : base(name)
        {

        }
        internal override Type GlobalSettingsType => typeof(TGlobalSettings);

        private TGlobalSettings _globalSettings = null;
    }
    
}
