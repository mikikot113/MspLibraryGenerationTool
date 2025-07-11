using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NistMspToMsDialMsp
{
    public class RtCcsMatching
    {
        public static void mergeRTandCCSintoMsp(string inputmsp, string rtLibrary, string posLibrary, string negLibrary, string output)
        {
            var rtDic = new Dictionary<string, string>();
            var ccsDic = CcsValueFromLibrary(posLibrary, negLibrary);
            var storages = NistSdfToMspConvert.NistSdfToMspConvert.GetMspStorages(inputmsp);
            var blankRtList = new List<string>();
            var matchedCcsList = new List<string>();

            using (var sr = new StreamReader(rtLibrary, true))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    var linearray = line.Split('\t');
                    if (!rtDic.ContainsKey(linearray[2]))
                    {
                        rtDic.Add(linearray[2], linearray[4]);
                    }
                }
            }
            using (var sw = new StreamWriter(inputmsp + "_mergedRtCcs.msp", false, Encoding.ASCII))
            {
                foreach (var storage in storages)
                {
                    //var inchikeyArray = storage.InchiKey.Split('-');
                    if (rtDic.ContainsKey(storage.InchiKey))
                    {
                        storage.Retentiontime = rtDic[storage.InchiKey];
                    }
                    if (storage.Retentiontime == "" || storage.Retentiontime == null)
                    {
                        blankRtList.Add(storage.Name + "\t" + storage.InchiKey);
                    }
                    if (ccsDic.ContainsKey(storage.InchiKey))
                    {
                        var ccsDicSub = ccsDic[storage.InchiKey];
                        if (ccsDicSub.ContainsKey(storage.PrecursorType))
                        {
                            storage.CollisionCrossSection = ccsDicSub[storage.PrecursorType];
                            matchedCcsList.Add(storage.Name + "\t" + storage.InchiKey + "\t" + storage.CollisionCrossSection);
                        }
                    }
                    NistSdfToMspConvert.NistSdfToMspConvert.writeMspFields(storage, sw);
                }
            }

            using (var sw = new StreamWriter(inputmsp + "_noRtList.txt", false, Encoding.ASCII))
            {
                foreach (var item in blankRtList)
                {
                    sw.WriteLine(item);
                }
            }

            using (var sw = new StreamWriter(inputmsp + "_matchCcsList.txt", false, Encoding.ASCII))
            {
                foreach (var item in matchedCcsList)
                {
                    sw.WriteLine(item);
                }
            }


        }


        private static Dictionary<string, Dictionary<string, string>> CcsValueFromLibrary(string posLibrary, string negLibrary)
        {
            var ccsLibrary = new Dictionary<string, Dictionary<string, string>>();

            using (var sr = new StreamReader(posLibrary, true))
            {
                var headerLine = sr.ReadLine();

                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    var linearray = line.Split('\t');
                    //var inchikeyArray = linearray[4].Split('-');
                    if (ccsLibrary.ContainsKey(linearray[4]))
                    {
                        var ccsLibrarySub = ccsLibrary[linearray[4]];
                        if (!ccsLibrarySub.ContainsKey(linearray[3]))
                        {
                            ccsLibrarySub.Add(linearray[3], linearray[8]);
                            ccsLibrary.Remove(linearray[4]);

                            ccsLibrary.Add(linearray[4], ccsLibrarySub);
                        }
                    }
                    else
                    {
                        var ccsLibrarySub = new Dictionary<string, string>();
                        ccsLibrarySub.Add(linearray[3], linearray[8]);
                        ccsLibrary.Add(linearray[4], ccsLibrarySub);
                    }
                }
            }

            using (var sr = new StreamReader(negLibrary, true))
            {
                var headerLine = sr.ReadLine();

                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    var linearray = line.Split('\t');
                    //var inchikeyArray = linearray[4].Split('-');
                    if (ccsLibrary.ContainsKey(linearray[4]))
                    {
                        var ccsLibrarySub = ccsLibrary[linearray[4]];
                        if (!ccsLibrarySub.ContainsKey(linearray[3]))
                        {
                            ccsLibrarySub.Add(linearray[3], linearray[8]);
                            ccsLibrary.Remove(linearray[4]);

                            ccsLibrary.Add(linearray[4], ccsLibrarySub);
                        }
                    }
                    else
                    {
                        var ccsLibrarySub = new Dictionary<string, string>();
                        ccsLibrarySub.Add(linearray[3], linearray[8]);
                        ccsLibrary.Add(linearray[4], ccsLibrarySub);
                    }
                }
            }

            return ccsLibrary;
        }

    }
}
