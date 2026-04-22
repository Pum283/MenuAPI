namespace Application.Common.Interfaces
{
    public interface ICafeAccessService
    {
        Task<bool> CanManageCafeAsync(int userId, int cafeId, CancellationToken cancellationToken = default);
        Task<bool> IsAdminAsync(int userId, CancellationToken cancellationToken = default);
    }
}

