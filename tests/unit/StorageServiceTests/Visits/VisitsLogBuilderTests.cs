namespace StorageServiceTests.Visits
{

    using AutoFixture;

    using FluentAssertions;

    using StorageService.File;
    using StorageService.Visits;

    using Xunit;

    using File = System.IO.File;
    
    public class VisitsLogBuilderTests
    {
        private readonly VisitsLogBuilder visitsLogBuilder = new VisitsLogBuilder();
        private readonly Fixture fixture = new Fixture();
        
        [Fact]
        public void Build_WithAllPropertiesFilled_ReturnsCorrectVisitLog()
        {
            // Arrange
            var referer = this.fixture.Create<string>();
            var userAgent = this.fixture.Create<string>();
            var ipAddress = this.fixture.Create<string>();
            var timestamp = this.fixture.Create<DateTimeOffset>();

            this.visitsLogBuilder.WithReferer(referer)
                .WithUserAgent(userAgent)
                .WithTimestamp(timestamp)
                .WithIpAddress(ipAddress);
            
            // Act
            var result = this.visitsLogBuilder.Build();
            
            // Assert
            result.Should().NotBeNull();
            result.IpAddress.Should().Be(ipAddress);
            result.UserAgent.Should().Be(userAgent);
            result.Referer.Should().Be(referer);
            result.Timestamp.Should().Be(timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Build_IpAddressNullOrEmpty_ThrowsArgumentException(string? ipAddress)
        {
            // Arrange
            var referer = this.fixture.Create<string>();
            var userAgent = this.fixture.Create<string>();
            var timestamp = this.fixture.Create<DateTimeOffset>();

            this.visitsLogBuilder.WithReferer(referer)
                .WithUserAgent(userAgent)
                .WithTimestamp(timestamp)
                .WithIpAddress(ipAddress);
            
            // Act
            Action build = () => this.visitsLogBuilder.Build();
            
            // Assert
            build.Should().ThrowExactly<ArgumentException>().WithMessage("Visit log must have an ip address");
        }
        
        // Could add more unit tests for the other properties being filled or not
    }
}