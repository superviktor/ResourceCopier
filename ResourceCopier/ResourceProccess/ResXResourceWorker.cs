using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using ResourceCopier.LocalizationParamsCreating;
using ResourceCopier.LocalizationParamsObjects;
using ResourceCopier.ResourceLocation;

namespace ResourceCopier.ResourceProccess
{
    public class ResXResourceWorker : ResourceWorker
    {
        private IResourceLocator _resourceLocator;
        private string _fuelWebUIPath;
        private LocalizationParamsCreator _localizationParamsCreator;

        public ResXResourceWorker(IResourceLocator resourceLocator, string fuelWebUIPath, LocalizationParamsCreator localizationParamsCreator)
        {
            _resourceLocator = resourceLocator;
            _fuelWebUIPath = fuelWebUIPath;
            _localizationParamsCreator = localizationParamsCreator;
        }

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

        public KvpModel GetKvpByKey(IEnumerable<string> paths, string key)
        {
            KvpModel kvpModel = null;
            foreach (var path in paths)
            {
                var kvps = ReadKeyValuePairs(path);
                var kvp = kvps.FirstOrDefault(x => x.Key.ToString() == key);
                if (kvp.Key != null && kvp.Value != null)
                {
                    kvpModel = new KvpModel()
                    {
                        DictionaryEntry = kvp,
                        FileName = path
                    };
                    break;
                }
            }

            return kvpModel;
        }

        public KvpModel GetKvpByValue(IEnumerable<string> paths, string value)
        {
            KvpModel kvpModel = null;
            foreach (var path in paths)
            {
                var kvps = ReadKeyValuePairs(path);
                var kvp = kvps.FirstOrDefault(x => x.Value.ToString() == value);
                if (kvp.Key != null && kvp.Value != null)
                {
                    kvpModel = new KvpModel()
                    {
                        DictionaryEntry = kvp,
                        FileName = path
                    };
                    break;
                }
            }

            return kvpModel;
        }

        public void FindDiffAndCopuFromAnotherResources(Language lang)
        {
            var engLocalizationParams = _localizationParamsCreator.Create(Language.Eng);
            var anotherLangLocalizationParams = _localizationParamsCreator.Create(lang);
            var engResXFiles = _resourceLocator.GetAll(_fuelWebUIPath, engLocalizationParams);
            var anotherLangResXFiles = _resourceLocator.GetAll(_fuelWebUIPath, anotherLangLocalizationParams);

            foreach (var engResXFile in engResXFiles)
            {
                var anotherLangResXFile = GetAnotherLangFileName(engResXFile, anotherLangLocalizationParams);

                if (File.Exists(anotherLangResXFile))
                {
                    var engResXFileKvps = ReadKeyValuePairs(engResXFile);
                    var anotherLangResXFileKvps = ReadKeyValuePairs(anotherLangResXFile);

                    var engResXFileKvpsCount = engResXFileKvps.Count();
                    var anotherLangResXFileKvpsCount = anotherLangResXFileKvps.Count();

                    if (engResXFileKvpsCount != anotherLangResXFileKvpsCount)
                    {
                        var diffs = engResXFileKvps.Select(x => x.Key.ToString())
                            .Except(anotherLangResXFileKvps.Select(y => y.Key.ToString()));

                        var diffKvps = diffs.Select(diff => new DictionaryEntry()
                        {
                            Key = diff,
                            Value = engResXFileKvps.FirstOrDefault(x => x.Key.ToString() == diff).Value
                        }).ToList();

                        int count = 0;
                        foreach (var diffKvp in diffKvps)
                        {
                            var foundKvp = GetKvpByValue(engResXFiles.Except(new List<string>{engResXFile}), diffKvp.Value.ToString());
                            if (foundKvp?.DictionaryEntry.Key != null && foundKvp?.DictionaryEntry.Value != null)
                            {
                                count++;
                                var foundKvpDictionaryEntry = foundKvp.DictionaryEntry;
                                foundKvpDictionaryEntry.Key = diffKvp.Key;
                                var anotherLangConcreteResXFile = GetAnotherLangFileName(foundKvp.FileName, anotherLangLocalizationParams);
                                var entryByKey = GetKvpByKey(new List<string>() { anotherLangConcreteResXFile },
                                    foundKvp.DictionaryEntry.Key.ToString());
                                if (entryByKey?.DictionaryEntry.Key!=null && entryByKey?.DictionaryEntry.Value != null)
                                {
                                    foundKvpDictionaryEntry.Value = entryByKey.DictionaryEntry.Value;
                                }
                                anotherLangResXFileKvps.Add(foundKvpDictionaryEntry);
                            }
                            else
                            {
                                Console.WriteLine($"Key {diffKvp.Value} exist in file {engResXFile} but does not exist in file {anotherLangResXFile}. Also we couldn't found such key in another files");
                            }
                        }

                        if (count > 0)
                        {
                            WriteKeyValuePairs(anotherLangResXFile, anotherLangResXFileKvps);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"File does not exist {anotherLangResXFile}");
                }

            }
        }

        public string GetAnotherLangFileName(string engFileName, LocalizationParams localizationParams)
        {
            var index = engFileName.IndexOf(".resx");
            var anotherLangResXFile =
                engFileName.Substring(0, index) + localizationParams.FileExtension;
            return anotherLangResXFile;
        }
    }
}
