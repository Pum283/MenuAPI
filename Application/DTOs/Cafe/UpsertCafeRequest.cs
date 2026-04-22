using Application.DTOs.Common;

namespace Application.DTOs.Cafe
{
    public class UpsertCafeRequest
    {
        public string Code { get; set; } = string.Empty;
        public CafeTranslationsRequest Translations { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }
}

