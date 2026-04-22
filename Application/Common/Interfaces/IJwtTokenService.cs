using Domain.Entity;

namespace Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(User user, IReadOnlyCollection<string> roleCodes);
    }
}

