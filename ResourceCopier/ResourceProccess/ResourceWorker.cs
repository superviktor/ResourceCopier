using System.Collections;
using System.Collections.Generic;

namespace ResourceCopier.ResourceProccess
{
    interface ResourceWorker
    {
        List<DictionaryEntry> ReadKeyValuePairs(string path);

        void WriteKeyValuePairs(string path, List<DictionaryEntry> exitedEntries);

        DictionaryEntry GetKvp(IEnumerable<string> paths, string key);
    }
}
