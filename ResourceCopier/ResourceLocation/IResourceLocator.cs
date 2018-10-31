using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceCopier.LocalizationParamsObjects;

namespace ResourceCopier.ResourceLocation
{
    public interface IResourceLocator
    {
        List<string> GetAll(string folderPath, LocalizationParams localizationParams);
    }
}
