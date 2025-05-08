using KYCVerificationAPI.Core;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Data.Repositories;
using Moq;

namespace KYCVerificationAPI.Tests.Repositories
{
    public class VerificationRepositoryTests
    {
        private readonly Mock<IVerificationRepository> _mockRepository;

        public VerificationRepositoryTests()
        {
            _mockRepository = new Mock<IVerificationRepository>();
        }

        [Fact]
        public async Task GetById_ShouldReturnVerification_WhenVerificationExists()
        {
            // Arrange
            var expectedVerification = TestHelpers.GetVerifications(1).First();

            _mockRepository.Setup(repo => repo.GetByIdAsync(expectedVerification.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedVerification);

            // Act
            var result = await _mockRepository.Object.GetByIdAsync(expectedVerification.Id,
                It.IsAny<CancellationToken>());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVerification, result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllVerifications()
        {
            // Arrange
            var verifications = TestHelpers.GetVerifications();

            _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(verifications);

            // Act
            var result = await _mockRepository.Object.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(verifications.Count, result.Count());
        }

        [Fact]
        public async Task Add_ShouldAddNewVerification()
        {
            // Arrange
            var newVerification = TestHelpers.GetVerifications(1).First();

            _mockRepository.Setup(repo => repo.AddAsync(newVerification, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newVerification);

            // Act
            var result = await _mockRepository.Object.AddAsync(newVerification, 
                It.IsAny<CancellationToken>());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newVerification, result);
        }

        [Fact]
        public async Task Update_ShouldUpdateExistingVerification()
        {
            // Arrange
            var verification = TestHelpers.GetVerifications(1).First();
            verification.Status = VerificationStatus.Failed;
            
            _mockRepository.Setup(repo => repo.UpdateAsync(verification, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(verification);

            // Act
            var result = await _mockRepository.Object.UpdateAsync(verification);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(VerificationStatus.Failed, result.Status);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenVerificationDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(nonExistentId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Verification);

            // Act
            var result = await _mockRepository.Object.GetByIdAsync(nonExistentId, 
                It.IsAny<CancellationToken>());

            // Assert
            Assert.Null(result);
        }
    }
}
