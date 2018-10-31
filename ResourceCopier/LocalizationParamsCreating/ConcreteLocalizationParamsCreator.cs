using ResourceCopier.LocalizationParamsObjects;

namespace ResourceCopier.LocalizationParamsCreating
{
    public class ConcreteLocalizationParamsCreator:LocalizationParamsCreator
    {
        public override LocalizationParamsObjects.LocalizationParams Create(Language language)
        {
            LocalizationParamsObjects.LocalizationParams localizationParams = null;
            switch (language)
            {
                case Language.Eng:
                    localizationParams = new EngLocalizationParams();
                    break;
                case Language.Ger:
                    localizationParams =  new GerLocalizationParams();
                    break;
                case Language.Fra:
                    localizationParams = new FraLocalizationParams();
                    break;
                case Language.Que:
                    localizationParams = new QueLocalizationParams();
                    break;
            }

            return localizationParams;
        }
    }
}
