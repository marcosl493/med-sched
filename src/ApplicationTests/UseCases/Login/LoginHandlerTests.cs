using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.UseCases.Login;
using Domain.Entities;
using Moq;

namespace ApplicationTests.UseCases.Login;

[TestClass()]
public class LoginHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPatientRepository> _patientRepository = new();
    private readonly Mock<IPhysicianRepository> _physicianRepository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private const string _validPassword = "senha123";
    private LoginHandler CreateHandler() =>
        new(_userRepository.Object, _patientRepository.Object, _physicianRepository.Object, _tokenService.Object);

    [TestMethod()]
    public async Task Handle_ShouldReturnToken_WhenPatientLoginIsSuccessful()
    {
        var user = new User("Patient", "patient@email.com", UserRole.PATIENT, _validPassword);
        _userRepository.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _patientRepository.Setup(r => r.GetPatientIdByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid());
        _tokenService.Setup(t => t.CreateToken(It.IsAny<Guid>(), user.Email, user.Role)).Returns(("token", "Bearer", 15));

        var handler = CreateHandler();
        var command = new LoginCommand(user.Email, _validPassword);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("token", result.Value.AccessToken);
    }

    [TestMethod()]
    public async Task Handle_ShouldReturnToken_WhenPhysicianLoginIsSuccessful()
    {
        var user = new User("Physician", "physician@email.com", UserRole.PHYSICIAN, _validPassword);
        _userRepository.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _physicianRepository.Setup(r => r.GetPhysicianIdByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid());
        _tokenService.Setup(t => t.CreateToken(It.IsAny<Guid>(), user.Email, user.Role)).Returns(("token", "Bearer", 15));

        var handler = CreateHandler();
        var command = new LoginCommand(user.Email, _validPassword);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("token", result.Value.AccessToken);
    }

    [TestMethod()]
    public async Task Handle_ShouldFail_WhenUserDoesNotExist()
    {
        _userRepository.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

        var handler = CreateHandler();
        var command = new LoginCommand("notfound@email.com", "senha123");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(401, result.Errors[0].Metadata["StatusCode"]);
    }

    [TestMethod()]
    public async Task Handle_ShouldFail_WhenPasswordIsIncorrect()
    {
        var user = new User("Patient", "patient@email.com", UserRole.PATIENT, _validPassword);
        _userRepository.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _patientRepository.Setup(r => r.GetPatientIdByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid());

        var handler = CreateHandler();
        var command = new LoginCommand(user.Email, "wrongpassword");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(401, result.Errors[0].Metadata["StatusCode"]);
    }

    [TestMethod()]
    public async Task Handle_ShouldFail_WhenUserRoleIsUnknown()
    {
        var user = new User("Patient", "patient@email.com", (UserRole)999, _validPassword);
        _userRepository.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var handler = CreateHandler();
        var command = new LoginCommand(user.Email, _validPassword);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(401, result.Errors[0].Metadata["StatusCode"]);
    }
}
