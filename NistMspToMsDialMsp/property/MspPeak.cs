using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NistSdfToMspConvert
{
    public class MspPeak
    {
        float mz;
        float intensity;
        string comment;
        string frag;

        public float Mz
        {
            get { return mz; }
            set { mz = value; }
        }

        public float Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public string Frag
        {
            get { return frag; }
            set { frag = value; }
        }
    }
}
