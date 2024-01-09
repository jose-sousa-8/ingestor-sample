namespace PixelServiceTests.Images
{
    using FluentAssertions;

    using PixelService.Images;

    using Xunit;

    public class GifImageGeneratorTests
    {
        private readonly GifImageGenerator gifImageGenerator = new GifImageGenerator();
        
        [Fact]
        public void Generate_ShouldReturnAFilledByteArray()
        {
            // Act
            var result = this.gifImageGenerator.Generate();
            
            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(31);
        }
    }
}