using NSSplash;
using NSSplash.impl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MspLibraryGenerationTool
{
    public class SplashUtil
    {
        public static List<MspStorage> SetSplashToMsp(List<MspStorage> mspStorage)
        {
            var NewMspStorage = new List<MspStorage>();
            foreach (var msp in mspStorage)
            {
                if (msp != null && msp.Peaks != null && msp.Peaks.Count > 0)
                {
                    if (msp.Splash == null )
                    {
                        var splash = CalculateSplash(msp.Peaks);
                        if (splash != null || splash != "")
                        {
                            msp.Splash = splash;
                        }
                    }
                }
                NewMspStorage.Add(msp);
            }
            return NewMspStorage;
        }


        private static string CalculateSplash(List<MspPeak> peaks)
        {
            var ions = new List<Ion>();
            peaks.ForEach(it => ions.Add(new Ion(it.Mz, it.Intensity)));
            string splash = new Splash().splashIt(new MSSpectrum(ions));
            return splash;
        }
    }
}
