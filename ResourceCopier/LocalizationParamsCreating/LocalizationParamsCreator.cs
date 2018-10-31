using ResourceCopier.LocalizationParamsObjects;

namespace ResourceCopier.LocalizationParamsCreating
{
    public abstract class LocalizationParamsCreator
    {
        public abstract LocalizationParams Create(Language language);
    }
}
