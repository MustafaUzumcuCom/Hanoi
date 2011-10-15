using Hanoi.Serialization.InstantMessaging.Register;

namespace Hanoi
{
    public interface IFeatureDetection {
        StreamFeatures Process(Serialization.Core.Streams.StreamFeatures features);
    }

    public class FeatureDetection : IFeatureDetection
    {
        private static IFeatureDetection _default;
        public static IFeatureDetection Default
        {
            get { return _default ?? (_default = new FeatureDetection()); }
        }

        public StreamFeatures Process(Serialization.Core.Streams.StreamFeatures features)
        {

            StreamFeatures streamFeatures = default(StreamFeatures);
            if (features.Mechanisms != null && features.Mechanisms.SaslMechanisms.Count > 0)
            {
                foreach (string mechanism in features.Mechanisms.SaslMechanisms)
                {
                    switch (mechanism)
                    {
                        case XmppCodes.SaslDigestMD5Mechanism:
                            streamFeatures |= StreamFeatures.SaslDigestMD5;
                            break;

                        case XmppCodes.SaslPlainMechanism:
                            streamFeatures |= StreamFeatures.SaslPlain;
                            break;

                        case XmppCodes.SaslXGoogleTokenMechanism:
                            streamFeatures |= StreamFeatures.XGoogleToken;
                            break;
                    }
                }
            }

            if (features.Bind != null)
            {
                streamFeatures |= StreamFeatures.ResourceBinding;
            }

            if (features.SessionSpecified)
            {
                streamFeatures |= StreamFeatures.Sessions;
            }

            if (features.Items.Count > 0)
            {
                foreach (object item in features.Items)
                {
                    if (item is RegisterIQ)
                    {
                        streamFeatures |= StreamFeatures.InBandRegistration;
                    }
                }
            }

            return streamFeatures;
        }
    }
}
