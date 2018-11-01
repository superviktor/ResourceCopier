using System;
using System.Linq;
using ResourceCopier.LocalizationParamsCreating;
using ResourceCopier.ResourceLocation;
using ResourceCopier.ResourceProccess;

namespace ResourceCopier
{
    class Program
    {
        private static readonly string _path = @"";
        static void Main(string[] args)
        {           
            var resourceLocator = new ResXResourceLocator();
            var localizationParamsCreator = new ConcreteLocalizationParamsCreator();
            var worker = new ResXResourceWorker(resourceLocator, _path, localizationParamsCreator);
            foreach (var lang in Enum.GetValues(typeof(Language)).Cast<Language>())
            {
                worker.FindDiffAndCopuFromAnotherResources(lang);
                Console.WriteLine("\n");
                Console.WriteLine("************************");
                Console.WriteLine("\n");
            }    
        }

    }
}
