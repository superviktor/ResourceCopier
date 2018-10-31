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

        public void FindDiffAndCopuFromAnotherResources(Language lang)
        {
            var engLocalizationParams = _localizationParamsCreator.Create(Language.Eng);
            var anotherLangLocalizationParams = _localizationParamsCreator.Create(Language.Ger);
            var engResXFiles = _resourceLocator.GetAll(_fuelWebUIPath, engLocalizationParams);
            var anotherLangResXFiles = _resourceLocator.GetAll(_fuelWebUIPath, anotherLangLocalizationParams);

            foreach (var engResXFile in engResXFiles)
            {
                var index = engResXFile.IndexOf(".resx");
                var anotherLangResXFile =
                    engResXFile.Substring(0, index) + anotherLangLocalizationParams.FileExtension;

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
                        int count = 0;
                        foreach (var diff in diffs)
                        {
                            var foundKvp = GetKvp(anotherLangResXFiles, diff);
                            if (foundKvp.Key != null && foundKvp.Value != null)
                            {
                                count++;
                                anotherLangResXFileKvps.Add(foundKvp);
                            }
                            else
                            {
                                Console.WriteLine($"Key {diff} exist in file {engResXFile} but does not exist in file {anotherLangResXFile}. Also we couldn't found such key in another files");
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
    }
}
