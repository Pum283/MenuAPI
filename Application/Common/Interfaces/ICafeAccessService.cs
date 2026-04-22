namespace Application.Common.Interfaces
{
    public interface ICafeAccessService
    {
        Task<bool> CanManageCafeAsync(Guid userId, Guid cafeId, CancellationToken cancellationToken = default);
        Task<bool> IsAdminAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}

