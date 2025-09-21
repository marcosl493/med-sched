namespace Domain.Entities;

public class User
{
    public User(string name, string email, UserRole role, string hashedPassword, string salt)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        Role = role;
        HashedPassword = hashedPassword;
        Salt = salt;
    }
    public User()
    {

    }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public UserRole Role { get; private set; }

}
public enum UserRole
{
    ADMIN = 1,
    PHYSICIAN,
    PATIENT
}