using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session4
{
    class Asset_EM
    {
        private EmergencyMaintenance item;
        public string Name { get; set; }
        public Asset_EM(EmergencyMaintenance item)
        {
            this.item = item;
            Name = item.Asset.AssetName + " " + item.ID;
        }
    }
}
