using Hanoi.Authentication;
using NSubstitute;
using Xunit;

namespace Hanoi.Tests
{
    public class AuthenticatorFactoryTests
    {
        [Fact]
        public void Create_WithInvalidFeature_ReturnsNull()
        {
            var connection = Substitute.For<Connection>();
            var factory = new AuthenticatorFactory();

            var auth = factory.Create(StreamFeatures.Sessions, connection);

            Assert.Null(auth);
        }

        [Fact]
        public void Create_WithValidFeature_ReturnsNotNull()
        {
            var connection = Substitute.For<Connection>();
            var factory = new AuthenticatorFactory();

            var auth = factory.Create(StreamFeatures.SaslPlain, connection);

            Assert.NotNull(auth);
        }

        [Fact]
        public void Create_WithValidFeature_ReturnsCorrectType()
        {
            var connection = Substitute.For<Connection>();
            var factory = new AuthenticatorFactory();

            var auth = factory.Create(StreamFeatures.SaslPlain, connection);

            Assert.IsType<SaslPlainAuthenticator>(auth);
        }

        [Fact]
        public void Create_WithXGoogleToken_ReturnsCorrectAuthenticator()
        {
            var connection = Substitute.For<Connection>();
            var factory = new AuthenticatorFactory();

            var auth = factory.Create(StreamFeatures.XGoogleToken, connection);

            Assert.IsType<SaslXGoogleTokenAuthenticator>(auth);
        }

        [Fact]
        public void Create_WithSaslDigestMD5Token_ReturnsCorrectAuthenticator()
        {
            var connection = Substitute.For<Connection>();
            var factory = new AuthenticatorFactory();

            var auth = factory.Create(StreamFeatures.SaslDigestMD5, connection);

            Assert.IsType<SaslDigestAuthenticator>(auth);
        }

        [Fact]
        public void Register_WithCustomToken_ReturnsCustomAuthenticator()
        {
            var feature = StreamFeatures.InBandRegistration;
            var connection = Substitute.For<Connection>();
            var fakeAuthenticator = Substitute.For<Authenticator>(connection);
            var factory = new AuthenticatorFactory();
            factory.Register(feature, c => fakeAuthenticator);
            
            var auth = factory.Create(feature, connection);

            Assert.Same(fakeAuthenticator, auth);
        }


        [Fact]
        public void Default_ForAllScenarios_IsNotNull()
        {
            Assert.NotNull(AuthenticatorFactory.Default);
            Assert.IsType<AuthenticatorFactory>(AuthenticatorFactory.Default);
        }
    }
}
