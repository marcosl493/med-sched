namespace Domain.Entities;

public class User
{
    public User(string name, string email, UserRole role)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        Role = role;
    }
    public User()
    {
        
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

}
public enum UserRole
{
    ADMIN = 1 ,
    PHYSICIAN,
    PATIENT
}