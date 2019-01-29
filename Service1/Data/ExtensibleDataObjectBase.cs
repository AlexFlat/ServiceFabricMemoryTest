using System.Runtime.Serialization;

namespace Service1.Data
{
    /// <summary>
    /// 
    /// </summary>    
    [DataContract]
    public class ExtensibleDataObjectBase : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        public ExtensionDataObject ExtensionData
        {
            get;
            set;
        }
    }
}
