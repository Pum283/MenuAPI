namespace Application.DTOs.Auth
{
    public class BlockUserRequest
    {
        public bool IsBlocked { get; set; }
        public string? Reason { get; set; }
    }
}

