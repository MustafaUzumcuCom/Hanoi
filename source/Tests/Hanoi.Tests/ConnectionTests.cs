using NSubstitute;
using Xunit;

namespace Hanoi.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public void Constructor_ForAllScenarios_SetsDependencies()
        {
            var detection = Substitute.For<IFeatureDetection>();
            var factory = Substitute.For<IAuthenticatorFactory>();

            var connection = new Connection(factory, detection);

            Assert.NotNull(connection.Authenticator);
            Assert.NotNull(connection.Features);
        }

        [Fact]
        public void Constructor_WhenNotSpecified_SetsDependencies()
        {
            var connection = new Connection();

            Assert.NotNull(connection.Authenticator);
            Assert.NotNull(connection.Features);
        }

        [Fact]
        public void Constructor_WhenNotSpecified_SetsDependenciesToDefault()
        {
            var connection = new Connection();

            Assert.Same(AuthenticatorFactory.Default, connection.Authenticator);
            Assert.Same(FeatureDetection.Default, connection.Features);
        }
    }
}
