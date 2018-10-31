using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCopier.ResourceProccess
{
    public class ResXResourceWorker : ResourceWorker
    {
        public List<DictionaryEntry> ReadKeyValuePairs(string path)
        {
            var existingResourceDictionaryEntries = new List<DictionaryEntry>();
            using (var resourceReader = new ResXResourceReader(path))
            {
                existingResourceDictionaryEntries = resourceReader.Cast<DictionaryEntry>().ToList();
            }

            return existingResourceDictionaryEntries;
        }

        public void WriteKeyValuePairs(string path, List<DictionaryEntry> existedEntries)
        {
            using (var writer = new ResXResourceWriter(path))
            {
                existedEntries.ForEach(r =>
                {
                    writer.AddResource(r.Key.ToString(), r.Value.ToString());
                });
                writer.Generate();
            }
        }

        public DictionaryEntry GetKvp(IEnumerable<string> paths, string key)
        {
            DictionaryEntry foundKey;
            foreach (var path in paths)
            {
                var kvps = ReadKeyValuePairs(path);
                var kvp = kvps.FirstOrDefault(x => x.Key.ToString() == key);
                if (kvp.Key !=null && kvp.Value!=null)
                {
                    foundKey = new DictionaryEntry(kvp.Key, kvp.Value);
                }
            }

            return foundKey;
        }
    }
}
