namespace Application.DTOs.Common
{
    public class CafeTranslationsRequest
    {
        public TranslationText? Vi { get; set; }
        public TranslationText? En { get; set; }
    }

    public class CategoryTranslationsRequest
    {
        public TranslationTextWithDescription? Vi { get; set; }
        public TranslationTextWithDescription? En { get; set; }
    }

    public class ItemTranslationsRequest
    {
        public TranslationTextWithDescription? Vi { get; set; }
        public TranslationTextWithDescription? En { get; set; }
    }

    public class TranslationText
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TranslationTextWithDescription
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

