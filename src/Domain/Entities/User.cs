namespace Domain.Entities;

public class User
{
    public User(string name, string email, UserRole role, string passwordInput)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        Role = role;
        HashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordInput);
    }
    public User()
    {

    }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public UserRole Role { get; private set; }

    public bool CheckPassword(string passwordInput)
    {
        return BCrypt.Net.BCrypt.Verify(passwordInput, HashedPassword);
    }
}
public enum UserRole
{
    ADMIN = 1,
    PHYSICIAN,
    PATIENT
}