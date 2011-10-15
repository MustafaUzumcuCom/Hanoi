using Hanoi.Authentication;
using NSubstitute;
using Xunit;

namespace Hanoi.Tests
{
    public class ConnectionFactoryTests
    {
        [Fact]
        public void Create_WithInvalidFeature_ReturnsNull()
        {
            var connection = Substitute.For<Connection>();
            var factory = new ConnectionFactory();

            var auth = factory.Create(StreamFeatures.Sessions, connection);

            Assert.Null(auth);
        }

        [Fact]
        public void Create_WithValidFeature_ReturnsNotNull()
        {
            var connection = Substitute.For<Connection>();
            var factory = new ConnectionFactory();

            var auth = factory.Create(StreamFeatures.SaslPlain, connection);

            Assert.NotNull(auth);
        }

        [Fact]
        public void Create_WithValidFeature_ReturnsCorrectType()
        {
            var connection = Substitute.For<Connection>();
            var factory = new ConnectionFactory();

            var auth = factory.Create(StreamFeatures.SaslPlain, connection);

            Assert.IsType<SaslPlainAuthenticator>(auth);
        }
    }
}
