public class AccountUpdateDTO
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string SocialFile { get; set; }
    public string MedicalFile { get; set; }
    public string CareerFile { get; set; }

    public string? Photo { get; set; }
    // Other updatable fields...

    public Account ToEntity()
    {
        return new Account
        {
            Username = Username,
            Name = Name,
            Role = Role,
            SocialFile = SocialFile,
            MedicalFile = MedicalFile,
            CareerFile = CareerFile,
            Photo = Photo
        };
    }
}
