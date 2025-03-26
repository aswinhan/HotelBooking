namespace UserManagement.Domain.Entities;

public class HostVerification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string GovernmentId { get; set; } = string.Empty;
    public string BusinessLicense { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public DateTime VerifiedAt { get; set; }
    public User User { get; set; } = null!;
}
