using System;
using System.IO;
using System.Linq;
using ResourceCopier.LocalizationParamsCreating;
using ResourceCopier.ResourceLocation;
using ResourceCopier.ResourceProccess;

namespace ResourceCopier
{
    class Program
    {
        private static readonly string _fuelWebUIPath = @"E:\onyx\Source\FUEL.Web.UI";
        static void Main(string[] args)
        {
            var engLocalizationParams = new ConcreteLocalizationParamsCreator().Create(Language.Eng);
            var anotherLanguageLocalizationParams = new ConcreteLocalizationParamsCreator().Create(Language.Que);
            var resourceLocator = new ResXResourceLocator();
            var resourceWorker = new ResXResourceWorker();
            var engResXFiles = resourceLocator.GetAll(_fuelWebUIPath, engLocalizationParams);
            var anotherLangResXFiles = resourceLocator.GetAll(_fuelWebUIPath, anotherLanguageLocalizationParams);

            foreach (var engResXFile in engResXFiles)
            {
                var index = engResXFile.IndexOf(".resx");
                var anotherLangResXFile =
                    engResXFile.Substring(0, index) + anotherLanguageLocalizationParams.FileExtension;

                if (File.Exists(anotherLangResXFile))
                {
                    var engResXFileKvps = resourceWorker.ReadKeyValuePairs(engResXFile);
                    var anotherLangResXFileKvps = resourceWorker.ReadKeyValuePairs(anotherLangResXFile);

                    var engResXFileKvpsCount = engResXFileKvps.Count();
                    var anotherLangResXFileKvpsCount = anotherLangResXFileKvps.Count();

                    if (engResXFileKvpsCount != anotherLangResXFileKvpsCount)
                    {
                        var diffs = engResXFileKvps.Select(x => x.Key.ToString())
                            .Except(anotherLangResXFileKvps.Select(y => y.Key.ToString()));

                        foreach (var diff in diffs)
                        {
                            var foundKvp = resourceWorker.GetKvp(anotherLangResXFiles, diff);
                            if (foundKvp != null)
                            {
                                anotherLangResXFileKvps.Add(foundKvp.Value);
                            }
                        }

                        resourceWorker.WriteKeyValuePairs(anotherLangResXFile, anotherLangResXFileKvps);
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
