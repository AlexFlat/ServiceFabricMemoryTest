using System.Runtime.Serialization;
using System.Diagnostics;

namespace Service1.Data
{
    [DataContract]
    [DebuggerDisplay("{ToString()}")]
    public class MetaData : ExtensibleDataObjectBase
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Readable representation of this instance. Useful during debugging inside watches.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}:{Value} {Type}";
        }
    }
}
