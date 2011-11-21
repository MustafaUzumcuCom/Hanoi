using System.Collections.Generic;

namespace Hanoi
{
    public class Features
    {
        public StreamFeatures StreamFeatures { get; set; }
        public IList<string> SaslFeatures { get; set; }

        public Features()
        {
            StreamFeatures = default(StreamFeatures);
            SaslFeatures = new List<string>();
        }
    }
}