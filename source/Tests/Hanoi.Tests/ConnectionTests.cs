using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Testing;
using System.Reactive.Testing.Mocks;
using Hanoi.Transports;
using NSubstitute;
using Xunit;
using System.Concurrency;

namespace Hanoi.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public void Constructor_ForAllScenarios_SetsDependencies()
        {
            var detection = Substitute.For<IFeatureDetection>();
            var factory = Substitute.For<IAuthenticatorFactory>();
            var connectionFactory = Substitute.For<IConnectionFactory>();

            var connection = new Connection(factory, detection, connectionFactory);

            Assert.NotNull(connection.Authenticator);
            Assert.NotNull(connection.Features);
            Assert.NotNull(connection.Factory);
        }

        [Fact]
        public void Constructor_WhenNotSpecified_SetsDependencies()
        {
            var connection = new Connection();

            Assert.NotNull(connection.Authenticator);
            Assert.NotNull(connection.Features);
            Assert.NotNull(connection.Factory);
        }

        [Fact]
        public void Constructor_WhenNotSpecified_SetsDependenciesToDefault()
        {
            var connection = new Connection();

            Assert.Same(AuthenticatorFactory.Default, connection.Authenticator);
            Assert.Same(FeatureDetection.Default, connection.Features);
            Assert.Same(ConnectionFactory.Default, connection.Factory);
        }

        [Fact]
        public void Open_WithMockFactory_CallsCreate()
        {
            // arrange
            var init = new List<string> { "something" };
            var messages = new List<object> { new Serialization.Core.Streams.StreamFeatures() };

            var mockTransport = Substitute.For<ITransport>();
            mockTransport.OnXmppStreamInitialized.Returns(init.ToObservable()
                                                              .Delay(TimeSpan.FromSeconds(1)));
            mockTransport.OnMessageReceived.Returns(messages.ToObservable()
                                                            .Delay(TimeSpan.FromSeconds(3)));

            var mockFactory = Substitute.For<IConnectionFactory>();
            mockFactory.Create(Arg.Any<ConnectionString>()).Returns(mockTransport);
            ConnectionFactory.SetDefault(mockFactory);

            // act
            var connection = new Connection();
            connection.Open("server=localhost;user id=123;user password=456");

            mockFactory.Received().Create(Arg.Any<ConnectionString>());
        }
    }
}
