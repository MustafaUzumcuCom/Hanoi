using Xunit;

namespace Hanoi.Tests
{
    public class FeatureDetectionTests
    {
        [Fact]
        public void Process_ForEmptyStreamCapabilities_ReturnsDefault() {
            var detection = new FeatureDetection();
            var features = new Serialization.Core.Streams.StreamFeatures();

            var output = detection.Process(features);

            Assert.Equal(default(StreamFeatures), output);
        }

        [Fact]
        public void Process_WithMechanismSet_ReturnsFeature()
        {
            var detection = new FeatureDetection();
            var features = new Serialization.Core.Streams.StreamFeatures();
            features.Mechanisms.SaslMechanisms.Add(XmppCodes.SaslDigestMD5Mechanism);

            var output = detection.Process(features);

            Assert.True(output.HasFlag(StreamFeatures.SaslDigestMD5));
        }
    }
}
