using Hanoi.Serialization.Core.ResourceBinding;
using Hanoi.Serialization.Core.Sasl;
using Xunit;

namespace Hanoi.Tests
{
    public class FeatureDetectionTests
    {
        [Fact]
        public void Process_ForEmptyStreamCapabilities_ReturnsDefault()
        {
            var detection = new FeatureDetection();
            var features = new Serialization.Core.Streams.StreamFeatures();

            var output = detection.Process(features);

            Assert.Equal(default(StreamFeatures), output);
        }

        [Fact]
        public void Process_WithMechanismSet_ReturnsFeature()
        {
            var detection = new FeatureDetection();
            var features = new Serialization.Core.Streams.StreamFeatures { Mechanisms = new Mechanisms() };
            features.Mechanisms.SaslMechanisms.Add(XmppCodes.SaslDigestMD5Mechanism);

            var output = detection.Process(features);

            Assert.True(output.HasFlag(StreamFeatures.SaslDigestMD5));
        }

        [Fact]
        public void Process_WithMultipleMechanismsSet_ReturnsFeatures()
        {
            var detection = new FeatureDetection();
            var features = new Serialization.Core.Streams.StreamFeatures { Mechanisms = new Mechanisms() };
            features.Mechanisms.SaslMechanisms.Add(XmppCodes.SaslDigestMD5Mechanism);
            features.Mechanisms.SaslMechanisms.Add(XmppCodes.SaslXGoogleTokenMechanism);

            var output = detection.Process(features);

            Assert.True(output.HasFlag(StreamFeatures.SaslDigestMD5));
            Assert.True(output.HasFlag(StreamFeatures.XGoogleToken));
        }

        [Fact]
        public void Process_WithBindSet_ReturnsBinding()
        {
            var detection = new FeatureDetection();
            var features = new Serialization.Core.Streams.StreamFeatures
                               {
                                   Mechanisms = new Mechanisms(),
                                   Bind = new Bind()
                               };

            var output = detection.Process(features);

            Assert.True(output.HasFlag(StreamFeatures.ResourceBinding));
        }

        [Fact]
        public void Default_ForAllScenarios_IsNotNull()
        {
            Assert.NotNull(FeatureDetection.Default);
            Assert.IsType<FeatureDetection>(FeatureDetection.Default);
        }
    }
}
