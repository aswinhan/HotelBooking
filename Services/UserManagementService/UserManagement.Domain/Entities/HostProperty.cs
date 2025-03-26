namespace UserManagement.Domain.Entities;

public class HostProperty
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string PropertyLicense { get; set; } = string.Empty;
    public bool IsListed { get; set; } = true;
    public User Host { get; set; } = null!;
}
