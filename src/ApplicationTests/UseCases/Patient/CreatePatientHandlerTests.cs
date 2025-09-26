using Application.Interfaces.Repositories;
using Application.UseCases.Patient;
using Moq;

namespace ApplicationTests.UseCases.Patient;

[TestClass()]
public class CreatePatientHandlerTests
{
    [TestMethod()]
    public async Task Handle_ShouldCreatePatientAndReturnSuccessResult()
    {
        // Arrange
        var patientRepositoryMock = new Mock<IPatientRepository>();
        patientRepositoryMock
            .Setup(repo => repo.CreatePatientAsync(It.IsAny<Domain.Entities.Patient>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreatePatientHandler(patientRepositoryMock.Object);

        var command = new CreatePatientCommand(
            "Nome Teste",
            "email@teste.com",
            "senha123",
            new DateTime(1990, 1, 1)
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreNotEqual(Guid.Empty, result.Value.Id);
        Assert.AreEqual("Created", result.Successes[0].Message);

        patientRepositoryMock.Verify(repo => repo.CreatePatientAsync(It.IsAny<Domain.Entities.Patient>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}