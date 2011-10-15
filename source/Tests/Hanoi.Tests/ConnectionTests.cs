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
            Assert.NotNull(connection.FeatureDetection);
        }
    }
}
