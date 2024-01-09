namespace StorageServiceTests.File
{
    using AutoFixture;

    using FluentAssertions;

    using StorageService.File;

    using Xunit;

    using File = System.IO.File;
    
    public class FileReaderTests
    {
        private readonly FileReader fileReader = new FileReader();
        private readonly Fixture fixture = new Fixture();
        
        [Fact]
        public void ReadFile_FileDoesNotExist_ReturnsNull()
        {
            // Arrange
            var randomFilePath = this.fixture.Create<string>();
            
            // Act
            var result = this.fileReader.ReadFile(randomFilePath);
            
            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public void ReadFile_FileIsEmpty_ReturnsEmpty()
        {
            // Arrange
            var randomFilePath = this.fixture.Create<string>();
            using var fs = File.Create(randomFilePath);
            fs.Dispose();
            
            // Act
            var result = this.fileReader.ReadFile(randomFilePath);
            
            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void ReadFile_FileIsNotEmpty_ReturnsFileContents()
        {
            // Arrange
            var randomFilePath = this.fixture.Create<string>();
            using var fs = File.Create(randomFilePath);
            using var writer = new StreamWriter(fs);
            var randomFileContent = this.fixture.Create<string>();
            writer.WriteLine(randomFileContent);
            writer.Dispose();
            fs.Dispose();
            
            // Act
            var result = this.fileReader.ReadFile(randomFilePath);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().Contain(randomFileContent);
        }
    }
}