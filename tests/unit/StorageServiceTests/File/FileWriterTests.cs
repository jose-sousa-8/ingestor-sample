namespace StorageServiceTests.File
{
    using AutoFixture;

    using FluentAssertions;

    using StorageService.File;

    using Xunit;

    using File = System.IO.File;
    
    public class FileWriterTests
    {
        private readonly FileWriter fileWriter = new FileWriter();
        private readonly Fixture fixture = new Fixture();
        
        [Fact]
        public void EnsureFileExists_FileDoesNotExist_ShouldBeCreated()
        {
            // Arrange
            var randomFilePath = this.fixture.Create<string>();
            File.Exists(randomFilePath).Should().BeFalse();
            
            // Act
            this.fileWriter.EnsureFileExists(randomFilePath);
            
            // Assert
            File.Exists(randomFilePath).Should().BeTrue();
        }
        
        [Fact]
        public void EnsureFileExists_FileAlreadyExists_FileCreationIsSkipped()
        {
            // Arrange
            var randomFilePath = this.fixture.Create<string>();
            using var fs = File.Create(randomFilePath);
            fs.Dispose();
            
            File.Exists(randomFilePath).Should().BeTrue();
            
            // Act
            this.fileWriter.EnsureFileExists(randomFilePath);
            
            // Assert
            // Since we're using System.IO.File static methods, calls cannot be asserted 
            File.Exists(randomFilePath).Should().BeTrue();
        }
    }
}