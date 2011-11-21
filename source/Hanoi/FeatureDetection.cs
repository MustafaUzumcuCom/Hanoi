using Hanoi.Serialization.InstantMessaging.Register;

namespace Hanoi
{
    public class FeatureDetection : IFeatureDetection
    {
        private static IFeatureDetection _default;
        public static IFeatureDetection Default
        {
            get { return _default ?? (_default = new FeatureDetection()); }
        }

        public Features Process(Serialization.Core.Streams.StreamFeatures features)
        {
            var streamFeatures = new Features();
            if (features.Mechanisms != null && features.Mechanisms.SaslMechanisms.Count > 0)
            {
                foreach (string mechanism in features.Mechanisms.SaslMechanisms)
                {
                    streamFeatures.SaslFeatures.Add(mechanism);
                    switch (mechanism)
                    {
                        case XmppCodes.SaslDigestMD5Mechanism:
                            streamFeatures.StreamFeatures |= StreamFeatures.SaslDigestMD5;
                            break;

                        case XmppCodes.SaslPlainMechanism:
                            streamFeatures.StreamFeatures |= StreamFeatures.SaslPlain;
                            break;

                        case XmppCodes.SaslXGoogleTokenMechanism:
                            streamFeatures.StreamFeatures |= StreamFeatures.XGoogleToken;
                            break;
                    }
                }
            }

            if (features.Bind != null)
            {
                streamFeatures.StreamFeatures |= StreamFeatures.ResourceBinding;
            }

            if (features.SessionSpecified)
            {
                streamFeatures.StreamFeatures |= StreamFeatures.Sessions;
            }

            if (features.Items.Count > 0)
            {
                foreach (object item in features.Items)
                {
                    if (item is RegisterIQ)
                    {
                        streamFeatures.StreamFeatures |= StreamFeatures.InBandRegistration;
                    }
                }
            }

            return streamFeatures;
        }
    }
}
