using System.Collections;
using System.Collections.Generic;

namespace ResourceCopier.ResourceProccess
{
    interface ResourceWorker
    {
        List<DictionaryEntry> ReadKeyValuePairs(string path);

        void WriteKeyValuePairs(string path, List<DictionaryEntry> exitedEntries);

        KvpModel GetKvpByKey(IEnumerable<string> paths, string key);

        KvpModel GetKvpByValue(IEnumerable<string> paths, string value);

        void FindDiffAndCopuFromAnotherResources(Language lang);
    }
}
