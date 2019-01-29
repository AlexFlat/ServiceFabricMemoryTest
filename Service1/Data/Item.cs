using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Service1.Data
{
    [DataContract]
    public class Item
    {
        [DataMember]
        public MetaDataList MetaDatas { get; set; }
    }
}
