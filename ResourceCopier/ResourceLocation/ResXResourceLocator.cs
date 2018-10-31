using System.Collections.Generic;
using System.IO;
using System.Linq;
using ResourceCopier.LocalizationParamsObjects;

namespace ResourceCopier.ResourceLocation
{
    public class ResXResourceLocator: IResourceLocator
    {
        public List<string> GetAll(string folderPath, LocalizationParams localizationParams)
        {
            string searchPattern = "*";
            List<string> resFiles = new List<string>();
            if (localizationParams.Language == Language.Eng)
            {
                resFiles = Directory.GetFiles(folderPath, "*" + localizationParams.FileExtension, SearchOption.AllDirectories)
                    .Where(x => !x.Contains(".uk.resx")
                                && !x.Contains(".de.resx") &&
                                !x.Contains(".fr-FR.resx") &&
                                !x.Contains(".fr-CA.resx"))
                    .ToList();
            }
            else
            {
                resFiles = Directory.GetFiles(folderPath, "*" + localizationParams.FileExtension, SearchOption.AllDirectories).ToList();
            }
            return resFiles;
        }
    }
}
