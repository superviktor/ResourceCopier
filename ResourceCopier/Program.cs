using System;
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
            var resourceLocator = new ResXResourceLocator();
            var localizationParamsCreator = new ConcreteLocalizationParamsCreator();
            var worker = new ResXResourceWorker(resourceLocator, _fuelWebUIPath, localizationParamsCreator);
            foreach (var lang in Enum.GetValues(typeof(Language)).Cast<Language>())
            {
                worker.FindDiffAndCopuFromAnotherResources(lang);
            }    
        }

    }
}
