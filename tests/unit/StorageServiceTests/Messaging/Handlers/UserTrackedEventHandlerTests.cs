namespace StorageServiceTests.Messaging.Handlers
{
    using System.Reflection;

    using AutoFixture;

    using KafkaFlow;

    using Microsoft.Extensions.Options;

    using NSubstitute;

    using StorageService.File;
    using StorageService.Messaging.Handlers;
    using StorageService.Settings;

    using TrackingContracts;

    using Xunit;
    
    public class UserTrackedEventHandlerTests
    {
        private readonly UserTrackedEventHandler userTrackedEventHandler;
        private IFileWriter fileWriter;
        private StorageSettings storageSettings = new StorageSettings { VisitsLogFilePath = "/tmp/visits.log" };
        private IOptions<StorageSettings> storageSettingsOptions;
        private readonly Fixture fixture = new Fixture();

        public UserTrackedEventHandlerTests()
        {
            this.fileWriter = Substitute.For<IFileWriter>();
            this.storageSettingsOptions = Substitute.For<IOptions<StorageSettings>>();
            this.storageSettingsOptions.Value.Returns(this.storageSettings);
            this.userTrackedEventHandler = new UserTrackedEventHandler(this.storageSettingsOptions, this.fileWriter);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Handle_IpAddressIsNullOrEmpty_MessageIsDiscarded(string? ipAddress)
        {
            // Arrange
            // Since the property is read only , will have to use reflection
            // var userTrackedEvent = this.fixture.Build<UserTrackedEvent>()
            //    .Without(x => x.IpAddress).Create();
            var userTrackedEvent = new UserTrackedEvent("192.168.1.1");
            SetPropertyValue(userTrackedEvent, "IpAddress", ipAddress);
            
            // Act
            await this.userTrackedEventHandler.Handle(Substitute.For<IMessageContext>(), userTrackedEvent);

            // Assert
            this.fileWriter.DidNotReceive().AppendLine(Arg.Any<string>(), Arg.Any<string>());
        }
        
        [Fact]
        public async Task Handle_ValidUserTrackedEvent_VisitLogEntryIsCreatedCorrectly()
        {
            // Arrange
            var userTrackedEvent = this.fixture.Create<UserTrackedEvent>();
            
            // Act
            await this.userTrackedEventHandler.Handle(Substitute.For<IMessageContext>(), userTrackedEvent);

            // Assert
            var expectedFileLine = BuildExpectedLogEntry(userTrackedEvent);
            this.fileWriter.Received().AppendLine(this.storageSettings.VisitsLogFilePath, expectedFileLine);
        }
        
        [Fact]
        public async Task Handle_ValidUserTrackedEventNullReferer_VisitLogEntryIsCreatedCorrectly()
        {
            // Arrange
            var userTrackedEvent = this.fixture.Build<UserTrackedEvent>()
                .With(x => x.Referer, value:null).Create();
            
            // Act
            await this.userTrackedEventHandler.Handle(Substitute.For<IMessageContext>(), userTrackedEvent);

            // Assert
            var expectedFileLine = BuildExpectedLogEntry(userTrackedEvent);
            this.fileWriter.Received().AppendLine(this.storageSettings.VisitsLogFilePath, expectedFileLine);
        }
        
        [Fact]
        public async Task Handle_ValidUserTrackedEventNullUserAgent_VisitLogEntryIsCreatedCorrectly()
        {
            // Arrange
            var userTrackedEvent = this.fixture.Build<UserTrackedEvent>()
                .With(x => x.UserAgent, value:null).Create();
            
            // Act
            await this.userTrackedEventHandler.Handle(Substitute.For<IMessageContext>(), userTrackedEvent);

            // Assert
            var expectedFileLine = BuildExpectedLogEntry(userTrackedEvent);
            this.fileWriter.Received().AppendLine(this.storageSettings.VisitsLogFilePath, expectedFileLine);
        }
        
        [Fact]
        public async Task Handle_ValidUserTrackedEventBothUserAgentAndRefererAreNull_VisitLogEntryIsCreatedCorrectly()
        {
            // Arrange
            var userTrackedEvent = this.fixture.Build<UserTrackedEvent>()
                .With(x => x.UserAgent, value:null)
                .With(x => x.Referer, value:null).Create();
            
            // Act
            await this.userTrackedEventHandler.Handle(Substitute.For<IMessageContext>(), userTrackedEvent);

            // Assert
            var expectedFileLine = BuildExpectedLogEntry(userTrackedEvent);
            this.fileWriter.Received().AppendLine(this.storageSettings.VisitsLogFilePath, expectedFileLine);
        }
        
        private static string BuildExpectedLogEntry(UserTrackedEvent userTrackedEvent)
        {
            var referer = userTrackedEvent.Referer ?? "null";
            var userAgent = userTrackedEvent.UserAgent ?? "null";
            return $@"{userTrackedEvent.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}|{referer}|{userAgent}|{userTrackedEvent.IpAddress}";
        }

        static void SetPropertyValue(object obj, string propertyName, object value)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(obj, value);
            }
            else
            {
                Console.WriteLine($"Property '{propertyName}' not found or not writable.");
            }
        }
    }
}