using System.Collections.Generic;
using NSSplash;
using NSSplash.impl;
using System.IO;
using System.Text;
using NistSdfToMspConvert;
using System;

namespace NistMspToMsDialMsp
{

    public class MspCheck
    {
        public static void GetSpectrumSplashFromMsp(string input, string output)
        {
            var storage = new MspStorage();
            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {

                using (var sr = new StreamReader(input, Encoding.ASCII))
                {
                    while (sr.Peek() > -1)
                    {
                        var line = sr.ReadLine();
                        if ((line.Contains("NAME:") || line.Contains("Name:")) && line[0] == 'N')
                        {
                            storage = NistSdfToMspConvert.NistSdfToMspConvert.tryGetField(line, sr);
                        }
                        var splash = CalculateSplash(storage.Peaks);
                        sw.WriteLine(storage.Name + "\t" + storage.PrecursorType + "\t" + splash);
                    }
                }
            }
        }


        private static string CalculateSplash(List<MspPeak> peaks)
        {
            var ions = new List<Ion>();
            peaks.ForEach(it => ions.Add(new Ion(it.Mz, it.Intensity)));
            string splash = new Splash().splashIt(new MSSpectrum(ions));
            return splash;
        }

        public static void compairSplash(string file1, string file2, string resultOutput)
        {
            var file2Dic = new Dictionary<string, string>();

            using (var sr = new StreamReader(file2, true))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    var lineArray = line.Split('\t');
                    if (!file2Dic.ContainsKey(lineArray[2]))
                    {
                        file2Dic.Add(lineArray[2], lineArray[0] + "\t" + lineArray[1]);

                    }
                }
            }
            var matchList = new List<string>();

            var notMatchList = new List<string>();
            using (var sr = new StreamReader(file1, true))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    var lineArray = line.Split('\t');
                    var matchInFile2 = "";
                    if (!file2Dic.ContainsKey(lineArray[2]))
                    {
                        notMatchList.Add(line);
                    }
                    else
                    {
                        matchList.Add(line);
                    }
                }
            }
            using (var sw = new StreamWriter(resultOutput + ".noMatch", false, Encoding.ASCII))
            {
                foreach (var item in notMatchList)
                {
                    sw.WriteLine(item);
                }
            }
            using (var sw = new StreamWriter(resultOutput + ".matched", false, Encoding.ASCII))
            {
                foreach (var item in matchList)
                {
                    sw.WriteLine(item);
                }
            }

        }

        public static void peakIntensityFroatToInt(string input, string output)
        {
            var storage = new MspStorage();
            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {

                using (var sr = new StreamReader(input, Encoding.ASCII))
                {
                    while (sr.Peek() > -1)
                    {
                        var line = sr.ReadLine();
                        if ((line.Contains("NAME:") || line.Contains("Name:")) && line[0] == 'N')
                        {
                            storage = NistSdfToMspConvert.NistSdfToMspConvert.tryGetField(line, sr);
                        }

                        NistSdfToMspConvert.NistSdfToMspConvert.writeMspFields(storage, sw);
                    }
                }
            }
        }

        public static void IonModeChecker(string input)  //20231219
        {
            Console.WriteLine(input);
            var storage = new List<MspStorage>();
            var TrueMsp = new List<MspStorage>();
            var FalseMsp = new List<MspStorage>();
            using (var sr = new StreamReader(input, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    if ((line.Contains("NAME:") || line.Contains("Name:")) && line[0] == 'N')
                    {
                        storage.Add(NistSdfToMspConvert.NistSdfToMspConvert.tryGetField(line, sr));
                    }
                }
            }
            foreach (var item in storage)
            {
                if (item.Ionmode.ToUpper().StartsWith("P") && item.PrecursorType.EndsWith("+"))
                {
                    TrueMsp.Add(item);
                }
                else if (item.Ionmode.ToUpper().StartsWith("N") && item.PrecursorType.EndsWith("-"))
                {
                    TrueMsp.Add(item);
                }
                else
                {
                    FalseMsp.Add(item);
                }
            }
            if (FalseMsp.Count > 0)
            {
                using (var sw = new StreamWriter(input.Replace(".msp","_exclude.msp.txt"), false, Encoding.ASCII))
                {
                    foreach (var item in FalseMsp)
                    {
                        NistSdfToMspConvert.NistSdfToMspConvert.writeMspFields(item, sw);
                    }
                }
                using (var sw = new StreamWriter(input.Replace(".msp", "_Cleaned.msp"), false, Encoding.ASCII))
                {
                    foreach (var item in TrueMsp)
                    {
                        NistSdfToMspConvert.NistSdfToMspConvert.writeMspFields(item, sw);
                    }
                }

            }
        }
    }
}
