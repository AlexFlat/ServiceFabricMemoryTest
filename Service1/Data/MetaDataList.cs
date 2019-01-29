using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Service1.Data
{
    [CollectionDataContract]
    public class MetaDataList : List<MetaData>
    {
        /// <summary>
        /// Adds a new MetaData if not there, or updates if it exists
        /// </summary>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public MetaData AddOrUpdate(MetaData metaData)
        {
            if(metaData == null)
            {
                throw new ArgumentNullException("metaData");
            }
            if (string.IsNullOrEmpty(metaData.Name))
            {
                throw new ArgumentNullException("metaData.Name");
            }
            var existing = GetExisting(metaData);
            if (existing != null)
            {
                existing.Type = metaData.Type;
                existing.Value = metaData.Value;
                return existing;
            }
            else
            {
                Add(metaData);
                return metaData;
            }
        }

        public MetaData AddOrUpdate(string name, string value, string type = nameof(String))
        {
            return AddOrUpdate(new MetaData {Name = name, Type = type, Value = value});
        }

        public string GetValueOrDefault(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            var existing = GetExisting(new MetaData() { Name = name, Type = nameof(String) });
            if(existing == null)
            {
                return "";
            }
            return existing.Value;
        }

        public string GetValueOrNull(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            var existing = GetExisting(new MetaData() { Name = name, Type = nameof(String) });
            if (existing == null || string.IsNullOrEmpty(existing.Value))
            {
                return null;
            }
            return existing.Value;
        }

        private MetaData GetExisting(MetaData metaData)
        {
            return this.FirstOrDefault(item => string.IsNullOrEmpty(item.Name) ? false : item.Name.Equals(metaData.Name, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public string GetValueOrDefaultContains(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            var existing = GetExistingContains(new MetaData() { Name = name, Type = nameof(String) });
            if(existing == null)
            {
                return "";
            }
            return existing.Value;
        }

        private MetaData GetExistingContains(MetaData metaData)
        {
            return this.FirstOrDefault(item => string.IsNullOrEmpty(item.Name) ? false : item.Name.Contains(metaData.Name));
        }
    }
}
