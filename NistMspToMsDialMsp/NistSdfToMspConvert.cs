using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using NCDK.Graphs.InChI;
using NCDK.QSAR.Descriptors.Moleculars;
using NCDK.IO.Iterator;
using MathNet.Numerics.Statistics.Mcmc;
using System.Collections.Specialized;

namespace NistSdfToMspConvert
{
    public class NistSdfToMspConvert
    {
        private NistSdfToMspConvert() { }

        public static void Convert(string sdfFile, string exportFile)
        {
            var counter = 0;
            var inchikeySmilesDic = readSdfToInchikeyAndSmiles(sdfFile, exportFile);

            var noInchikeyList = new List<string>();

            using (var sw = new StreamWriter(exportFile, false, Encoding.ASCII))
            {
                using (var sr = new StreamReader(sdfFile, Encoding.ASCII))
                {
                    var endOfCompoundFlag = 0;
                    var firstLine = sr.ReadLine().Trim();
                    var title = firstLine;

                    while (sr.Peek() > -1)
                    {
                        var mspStorage = new MspStorage();
                        var line = sr.ReadLine().Trim();
                        if (endOfCompoundFlag == 1)
                        {
                            title = line;
                            endOfCompoundFlag = 0;
                        }
                        if (line == "$$$$")
                        {
                            endOfCompoundFlag = 1;
                        }

                        if (Regex.IsMatch(line, "<NAME>", RegexOptions.IgnoreCase))
                        {
                            mspStorage.Name = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();

                            while (line != "$$$$")
                            {
                                readSdfField(line, mspStorage, sr);
                                line = sr.ReadLine();
                            }
                            if (line == "$$$$")
                            {
                                endOfCompoundFlag = 1;
                            }
                            if (inchikeySmilesDic.ContainsKey(title))
                            {
                                mspStorage.InchiKey = inchikeySmilesDic[title].inChIKey;
                                mspStorage.Smiles = inchikeySmilesDic[title].Smiles;
                            }
                            else
                            {
                                noInchikeyList.Add(mspStorage.Name + "\t" + title);
                            }
                            mspStorage.Comment = "registered in NIST 20";
                            writeMspFields(mspStorage, sw);
                            Console.WriteLine(counter); counter++;
                        }

                    }
                }
            }

            using (var sw = new StreamWriter(exportFile + "_noInchikey.txt", false, Encoding.ASCII))
            {
                //noInchikeyList = noInchikeyList.Distinct().ToList();
                foreach (var item in noInchikeyList)
                {
                    sw.WriteLine(item);
                }

            }
        }

        public static Dictionary<string, MetaProperty> readSdfToInchikeyAndSmiles(string input, string output)
        {
            var SmilesParser = new SmilesParser();
            var SmilesGenerator = new SmilesGenerator(SmiFlavors.Canonical);
            var inchikeySmilesDic = new Dictionary<string, MetaProperty>();
            var inchikeySmilesList = new List<string>();

            using (var sr = new StreamReader(input, true))
            {
                using (var reader = new EnumerableSDFReader(sr))
                {

                    foreach (var iAtom in reader)
                    {
                        var name = iAtom.Title;

                        var smiles = SmilesGenerator.Create(iAtom);
                        if (smiles == "")
                        {
                            continue;
                        }
                        var InChIGeneratorFactory = new InChIGeneratorFactory();
                        var InChIKey = InChIGeneratorFactory.GetInChIGenerator(iAtom).GetInChIKey();
                        if (!inchikeySmilesDic.ContainsKey(name))
                        {
                            var meta = new MetaProperty()
                            {
                                Smiles = smiles,
                                inChIKey = InChIKey,
                            };

                            inchikeySmilesDic.Add(name, meta);
                            inchikeySmilesList.Add(name + "\t" + InChIKey + "\t" + smiles);
                        }
                    }
                }
            }
            using (var sw = new StreamWriter(output + "_InChIKey-SMILES.txt", false, Encoding.ASCII))
            {
                sw.WriteLine("name" + "\t" + "InChIKey" + "\t" + "SMILES");
                inchikeySmilesList = inchikeySmilesList.Distinct().ToList();
                foreach (var item in inchikeySmilesList)
                {
                    sw.WriteLine(item);
                }
            }
            return inchikeySmilesDic;
        }


        public static void readSdfField(string line, MspStorage mspStorage, StreamReader sr)
        {
            foreach (var field in SdfField.Fields)
            {
                if (Regex.IsMatch(line, Regex.Escape(field), RegexOptions.IgnoreCase))
                {
                    switch (field)
                    {
                        case "<NAME>":
                            mspStorage.Name = sr.ReadLine().Trim();
                            break;
                        case "<PRECURSOR M/Z>":
                            mspStorage.PrecursorMz = sr.ReadLine().Trim();
                            break;
                        case "<SYNONYMS>":

                            while (sr.Peek() > -1)
                            {
                                var lineSyno = sr.ReadLine();
                                if (lineSyno == string.Empty) break;

                                if (Regex.IsMatch(lineSyno, Regex.Escape("$:03"), RegexOptions.IgnoreCase))
                                {
                                    mspStorage.PrecursorType = lineSyno.Substring(4).Trim();
                                }
                                else if (Regex.IsMatch(lineSyno, Regex.Escape("$:07"), RegexOptions.IgnoreCase))
                                {
                                    mspStorage.Instrument = lineSyno.Substring(4).Trim();
                                }
                                else if (Regex.IsMatch(lineSyno, Regex.Escape("$:06"), RegexOptions.IgnoreCase))
                                {
                                    mspStorage.InstrumentType = lineSyno.Substring(4).Trim();
                                }
                                else if (Regex.IsMatch(lineSyno, Regex.Escape("$:10"), RegexOptions.IgnoreCase))
                                {
                                    mspStorage.Ionization = lineSyno.Substring(4).Trim();
                                }
                                else if (Regex.IsMatch(lineSyno, Regex.Escape("$:11"), RegexOptions.IgnoreCase))
                                {
                                    var ionMode = lineSyno.Substring(4).Trim();
                                    if (ionMode.Contains("N")) mspStorage.Ionmode = "Negative";
                                    else if (ionMode.Contains("P")) mspStorage.Ionmode = "Positive";
                                }
                                else if (Regex.IsMatch(lineSyno, Regex.Escape("$:05"), RegexOptions.IgnoreCase))
                                {
                                    var ce = lineSyno.Substring(4).Trim();
                                    mspStorage.CollisionEnergy = getCollisionEnergy(ce);
                                }
                            }

                            break;
                        case "<INSTRUMENT TYPE>":
                            mspStorage.InstrumentType = sr.ReadLine().Trim();
                            break;
                        case "<SPECTRUM TYPE>":
                            break;
                        case "<COMPOUND TYPE>":
                            break;
                        case "<PRECURSOR TYPE>":
                            mspStorage.PrecursorType = sr.ReadLine().Trim();
                            break;
                        case "<COLLISION ENERGY>":
                            mspStorage.CollisionEnergy = getCollisionEnergy(sr.ReadLine().Trim());
                            break;
                        case "<INSTRUMENT>":
                            mspStorage.Instrument = sr.ReadLine().Trim();
                            break;
                        case "<SAMPLE INLET>":
                            break;
                        case "<IONIZATION>":
                            mspStorage.Ionization = sr.ReadLine().Trim();
                            break;
                        case "<ION MODE>":
                            var ionmode = sr.ReadLine().Trim();
                            if (ionmode.Contains("N")) mspStorage.Ionmode = "Negative";
                            else if (ionmode.Contains("P")) mspStorage.Ionmode = "Positive";
                            break;
                        case "<FORMULA>":
                            mspStorage.Formula = sr.ReadLine().Trim();
                            break;
                        case "<EXACT MASS>":
                            break;
                        case "<CHARGE>":
                            mspStorage.Charge = sr.ReadLine().Trim();
                            break;
                        case "<NUM PEAKS>":
                            mspStorage.Peaknum = sr.ReadLine().Trim();
                            break;
                        case "<MASS SPECTRAL PEAKS>":
                            mspStorage.Peaks = getMspPeakField(mspStorage.Peaknum, sr);
                            break;
                        case "<COMMENT>":
                            mspStorage.Comment = sr.ReadLine().Trim();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void writeMspFields(MspStorage mspStorage, StreamWriter sw)
        {
            sw.WriteLine("NAME: " + mspStorage.Name);
            sw.WriteLine("PRECURSORMZ: " + mspStorage.PrecursorMz);
            sw.WriteLine("PRECURSORTYPE: " + mspStorage.PrecursorType);
            sw.WriteLine("IONMODE: " + mspStorage.Ionmode);
            sw.WriteLine("FORMULA: " + mspStorage.Formula);
            sw.WriteLine("SMILES: " + mspStorage.Smiles);
            //sw.WriteLine("INCHI: " + mspStorage.Inchi);
            sw.WriteLine("INCHIKEY: " + mspStorage.InchiKey);
            if (!String.IsNullOrEmpty(mspStorage.Ionization))
            {
                sw.WriteLine("IONIZATION: " + mspStorage.Ionization);
            }
            if (!String.IsNullOrEmpty(mspStorage.InstrumentType))
            {
                sw.WriteLine("INSTRUMENTTYPE: " + mspStorage.InstrumentType);
            }
            if (!String.IsNullOrEmpty(mspStorage.Instrument))
            {
                sw.WriteLine("INSTRUMENT: " + mspStorage.Instrument);
            }
            if (!String.IsNullOrEmpty(mspStorage.CollisionEnergy))
            {
                sw.WriteLine("COLLISIONENERGY: " + mspStorage.CollisionEnergy);
            }
            if (!String.IsNullOrEmpty(mspStorage.Authors))
            {
                sw.WriteLine("Authors: " + mspStorage.Authors);
            }
            if (!String.IsNullOrEmpty(mspStorage.CompoundClass))
            {
                sw.WriteLine("COMPOUNDCLASS: " + mspStorage.CompoundClass);
            }

            //sw.WriteLine("License: " + mspStorage.License);
            if (!String.IsNullOrEmpty(mspStorage.Retentiontime))
            {
                sw.WriteLine("RETENTIONTIME: " + mspStorage.Retentiontime);
            }
            if (!String.IsNullOrEmpty(mspStorage.CollisionCrossSection))
            {
                sw.WriteLine("CCS: " + mspStorage.CollisionCrossSection);
            }
            //sw.WriteLine("CHARGE: " + mspStorage.Charge);
            //sw.WriteLine("MASSBANKACCESSION: " + mspStorage.MassBankAccession);
            //sw.WriteLine("Links: " + mspStorage.Links);
            //sw.WriteLine("Comment: " + mspStorage.Comment);
            if (!String.IsNullOrEmpty(mspStorage.Ontology))
            {
                sw.WriteLine("ONTOLOGY: " + mspStorage.Ontology);
            }
            sw.WriteLine("COMMENT: " + mspStorage.Comment);

            var peakList = new List<string>();
            foreach (var peak in mspStorage.Peaks)
            {
                //sw.WriteLine(peak.Mz + "\t" + peak.Intensity);
                var intensityRound = Math.Round(peak.Intensity);
                if (intensityRound < 5)
                {
                    continue;
                }

                if (intensityRound == 999)
                {
                    intensityRound = 1000;
                }


                peakList.Add(Math.Round(peak.Mz, 3) + "\t" + intensityRound);
            }
            mspStorage.Peaknum = peakList.Count().ToString();

            sw.WriteLine("Num Peaks: " + mspStorage.Peaknum);


            foreach (var peak in peakList)
            {
                sw.WriteLine(peak);
            }

            sw.WriteLine();
        }

        private static void skipStream(StreamReader sr, string line)
        {
            while (line != "M  END")
            {
                if (line.Contains("No Structure")) { sr.ReadLine(); break; }
                line = sr.ReadLine();
                if (line == null) break;
                if (line != string.Empty) line = line.Trim();
            }
        }

        public static string getCollisionEnergy(string ce)
        {
            string figure = string.Empty;
            for (int i = 0; i < ce.Length; i++)
            {
                if (Char.IsNumber(ce[i]) || ce[i] == '.')
                {
                    figure += ce[i];
                }
                else
                {
                    return figure;
                }
            }
            return figure;
        }

        public static List<MspPeak> getMspPeakField(string num, StreamReader sr)
        {
            int peakNum; int.TryParse(num, out peakNum);

            var peaks = new List<MspPeak>();

            if (peakNum == 0) { peaks.Add(new MspPeak()); }

            var pairCount = 0;
            var peak = new MspPeak();

            while (pairCount < peakNum)
            {
                var wkstr = sr.ReadLine();
                var numChar = string.Empty;
                var mzFill = false;
                for (int i = 0; i < wkstr.Length; i++)
                {
                    if (char.IsNumber(wkstr[i]) || wkstr[i] == '.')
                    {
                        numChar += wkstr[i];

                        if (i == wkstr.Length - 1 && wkstr[i] != '"')
                        {
                            if (mzFill == false)
                            {
                                if (numChar != string.Empty)
                                {
                                    peak.Mz = float.Parse(numChar);
                                    mzFill = true;
                                    numChar = string.Empty;
                                }
                            }
                            else if (mzFill == true)
                            {
                                if (numChar != string.Empty)
                                {
                                    peak.Intensity = float.Parse(numChar);
                                    mzFill = false;
                                    numChar = string.Empty;

                                    if (peak.Comment == null)
                                        peak.Comment = peak.Mz.ToString();
                                    peaks.Add(peak);
                                    peak = new MspPeak();
                                    pairCount++;
                                }
                            }
                        }
                    }
                    else if (wkstr[i] == '"')
                    {
                        i++;
                        var letterChar = string.Empty;

                        while (wkstr[i] != '"')
                        {
                            letterChar += wkstr[i];
                            i++;
                        }
                        if (!letterChar.Contains("_f_"))
                            peaks[peaks.Count - 1].Comment = letterChar;
                        else
                        {
                            peaks[peaks.Count - 1].Comment = letterChar.Split(new string[] { "_f_" }, StringSplitOptions.None)[0];
                            peaks[peaks.Count - 1].Frag = letterChar.Split(new string[] { "_f_" }, StringSplitOptions.None)[1];
                        }

                    }
                    else
                    {
                        if (mzFill == false)
                        {
                            if (numChar != string.Empty)
                            {
                                peak.Mz = float.Parse(numChar);
                                mzFill = true;
                                numChar = string.Empty;
                            }
                        }
                        else if (mzFill == true)
                        {
                            if (numChar != string.Empty)
                            {
                                peak.Intensity = float.Parse(numChar);
                                mzFill = false;
                                numChar = string.Empty;

                                if (peak.Comment == null)
                                    peak.Comment = peak.Mz.ToString();

                                peaks.Add(peak);
                                peak = new MspPeak();
                                pairCount++;
                            }
                        }
                    }
                }
            }

            peaks = peaks.OrderBy(n => n.Mz).ToList();

            return peaks;
        }

        public static void SeparatePositiveNegative(string inputfile, string outputPositive, string outputNegative)
        {
            var storages = GetMspStorages(inputfile);

            using (var sw = new StreamWriter(outputPositive, false, Encoding.ASCII))
            {
                foreach (var storage in storages)
                {
                    if (storage.Ionmode == "Positive")
                        writeMspFields(storage, sw);
                }
            }

            using (var sw = new StreamWriter(outputNegative, false, Encoding.ASCII))
            {
                foreach (var storage in storages)
                {
                    if (storage.Ionmode == "Negative")
                        writeMspFields(storage, sw);
                }
            }
        }

        public static List<MspStorage> GetMspStorages(string inputfile)
        {
            var storages = new List<MspStorage>();

            using (var sr = new StreamReader(inputfile, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    if ((line.Contains("NAME:") || line.Contains("Name:")) && line[0] == 'N')
                    {
                        var storage = tryGetField(line, sr);
                        storages.Add(storage);
                        //if (storages.Count % 1000 == 0)
                        //    Console.WriteLine(storages.Count);
                    }
                }
            }

            return storages;
        }

        public static MspStorage tryGetField(string wkstr, StreamReader sr)
        {
            var storage = new MspStorage();
            storage.Name = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();

            while (sr.Peek() > -1)
            {
                wkstr = sr.ReadLine();
                if (wkstr == string.Empty) break;

                if (Regex.IsMatch(wkstr, "PRECURSORMZ:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "precursor m/z:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "pepmass:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "PreIon:", RegexOptions.IgnoreCase))
                {
                    storage.PrecursorMz = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "EXACTMASS:", RegexOptions.IgnoreCase))
                {
                    storage.Exactmass = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "ms level:", RegexOptions.IgnoreCase))
                {
                    storage.Mslevel = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "PRECURSORTYPE:", RegexOptions.IgnoreCase) || Regex.IsMatch(wkstr, "adduct:", RegexOptions.IgnoreCase) || Regex.IsMatch(wkstr, "ion type:", RegexOptions.IgnoreCase) || Regex.IsMatch(wkstr, "precursor type:", RegexOptions.IgnoreCase))
                {
                    storage.PrecursorType = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "INSTRUMENTTYPE:", RegexOptions.IgnoreCase))
                {
                    storage.InstrumentType = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "INSTRUMENT:", RegexOptions.IgnoreCase))
                {
                    storage.Instrument = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "Authors:", RegexOptions.IgnoreCase))
                {
                    storage.Authors = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "License:", RegexOptions.IgnoreCase))
                {
                    storage.License = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "BinId:", RegexOptions.IgnoreCase))
                {
                    storage.BinID = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "^SMILES:", RegexOptions.IgnoreCase))
                {
                    storage.Smiles = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "INCHI:", RegexOptions.IgnoreCase) || Regex.IsMatch(wkstr, "INCHICODE:", RegexOptions.IgnoreCase))
                {
                    storage.Inchi = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "INCHIKEY:", RegexOptions.IgnoreCase))
                {
                    storage.InchiKey = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "Comment:", RegexOptions.IgnoreCase))
                {
                    storage.Comment = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "COLLISIONENERGY:", RegexOptions.IgnoreCase) || Regex.IsMatch(wkstr, "collision energy:", RegexOptions.IgnoreCase))
                {
                    storage.CollisionEnergy = getCollisionEnergy(wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim());
                }
                else if (Regex.IsMatch(wkstr, "FORMULA:", RegexOptions.IgnoreCase))
                {
                    storage.Formula = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "RETENTIONTIME:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "retention time:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "RT:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "RetTime:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "RT\\[min]:", RegexOptions.IgnoreCase))
                {
                    storage.Retentiontime = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }

                else if (Regex.IsMatch(wkstr, "RETENTIONINDEX:", RegexOptions.IgnoreCase) || Regex.IsMatch(wkstr, "RI:", RegexOptions.IgnoreCase))
                {
                    storage.RetentionIndex = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }

                else if (Regex.IsMatch(wkstr, "CCS:", RegexOptions.IgnoreCase))
                {
                    storage.CollisionCrossSection = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }

                else if (Regex.IsMatch(wkstr, "IONMODE:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "ion mode:", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(wkstr, "IonPolarity:", RegexOptions.IgnoreCase))
                {
                    storage.Ionmode = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "IONIZATION:", RegexOptions.IgnoreCase))
                {
                    storage.Ionization = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }

                else if (Regex.IsMatch(wkstr, "RT_AritaM:", RegexOptions.IgnoreCase))
                {
                    storage.XlogP = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "RT_FiehnO:", RegexOptions.IgnoreCase))
                {
                    storage.SssCH2 = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "RT_SaitoK:", RegexOptions.IgnoreCase))
                {
                    storage.MeanI = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "origin:", RegexOptions.IgnoreCase))
                {
                    storage.Comment += wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "MASSBANKACCESSION:", RegexOptions.IgnoreCase) || Regex.IsMatch(wkstr, "accession:", RegexOptions.IgnoreCase))
                {
                    storage.MassBankAccession = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "Links:", RegexOptions.IgnoreCase))
                {
                    storage.Links = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "Ontology:", RegexOptions.IgnoreCase))
                {
                    storage.Ontology = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "COMPOUNDCLASS:", RegexOptions.IgnoreCase))
                {
                    storage.CompoundClass = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "CAS:", RegexOptions.IgnoreCase) || Regex.IsMatch(wkstr, "CASNO:", RegexOptions.IgnoreCase))
                {
                    storage.Cas = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "BasePeakIntensity:", RegexOptions.IgnoreCase))
                {
                    storage.BasepeakIntensity = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                }
                else if (Regex.IsMatch(wkstr, "Num Peaks:", RegexOptions.IgnoreCase))
                {
                    storage.Peaknum = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                    if (storage.Peaknum == "0")
                    {
                        storage.Peaks = new List<MspPeak>();
                        storage.Peaknum = "0";
                    }
                    else
                    {
                        storage.Peaks = getMspPeakField(storage.Peaknum, sr);
                        storage.Peaknum = storage.Peaks.Count.ToString();
                    }
                }
            }

            return storage;
        }


        public static void pickupNoInchikeyCompoundFromSdf(string input, string sdfFile, string output)
        {
            var pickupItemDic = new Dictionary<string, string>();
            var sdfInchikey = new List<String>();
            using (var sr = new StreamReader(input, true))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    var linearray = line.Split('\t');
                    if (!pickupItemDic.ContainsKey(linearray[1]))
                        pickupItemDic.Add(linearray[1], linearray[0]);
                }
            }
            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {
                using (var sr = new StreamReader(sdfFile, true))
                {
                    //var endOfCompoundFlag = 0;
                    while (sr.Peek() > -1)
                    {

                        var line = sr.ReadLine().Trim();

                        if (pickupItemDic.ContainsKey(line))
                        {
                            var title = pickupItemDic[line];
                            while (line != "$$$$")
                            {
                                sw.WriteLine(line);
                                if (Regex.IsMatch(line, Regex.Escape("$:28"), RegexOptions.IgnoreCase))
                                {
                                    sdfInchikey.Add(title + "\t" + line.Substring(4).Trim());
                                }

                                line = sr.ReadLine();
                            }
                            sw.WriteLine("$$$$");
                        }
                    }
                }
            }
            using (var sw = new StreamWriter(output + "_inchikey.txt", false, Encoding.ASCII))
            {
                foreach (var item in sdfInchikey)
                {
                    sw.WriteLine(item);
                }
            }
        }

        public static void mergeInchikeyAndSmilesToMsp(string inputmsp, string inchikeySmilesFile)
        {
            var inchikeySmilesDic = new Dictionary<string, MetaProperty>();
            var storages = GetMspStorages(inputmsp);
            using (var sr = new StreamReader(inchikeySmilesFile, true))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    var linearray = line.Split('\t');
                    if (!inchikeySmilesDic.ContainsKey(linearray[0]))
                    {
                        var meta = new MetaProperty()
                        {
                            Smiles = linearray[2],
                            inChIKey = linearray[1],
                        };

                        inchikeySmilesDic.Add(linearray[0], meta);
                    }
                }
            }

            using (var sw = new StreamWriter(inputmsp + "_mergedInChiKey.msp", false, Encoding.ASCII))
            {
                foreach (var storage in storages)
                {
                    if (storage.InchiKey == "")
                    {
                        if (inchikeySmilesDic.ContainsKey(storage.Name))
                        {
                            storage.InchiKey = inchikeySmilesDic[storage.Name].inChIKey;
                            storage.Smiles = inchikeySmilesDic[storage.Name].Smiles;
                        }
                    }
                    writeMspFields(storage, sw);
                }
            }
        }


        public static MetaProperty getMetaProperty(string rawSmiles)
        {
            var SmilesParser = new SmilesParser();
            var SmilesGenerator = new SmilesGenerator(SmiFlavors.Canonical);
            var iAtomContainer = SmilesParser.ParseSmiles(rawSmiles);
            var smiles = SmilesGenerator.Create(iAtomContainer);

            var InChIGeneratorFactory = new InChIGeneratorFactory();
            var InChIKey = InChIGeneratorFactory.GetInChIGenerator(iAtomContainer).GetInChIKey();

            var iMolecularFormula = MolecularFormulaManipulator.GetMolecularFormula(iAtomContainer);
            var formula = MolecularFormulaManipulator.GetString(iMolecularFormula);
            var exactMass = MolecularFormulaManipulator.GetMass(iMolecularFormula, MolecularWeightTypes.MonoIsotopic);

            var JPlogPDescriptor = new JPlogPDescriptor();
            var logP = JPlogPDescriptor.Calculate(iAtomContainer).JLogP;

            var meta = new MetaProperty()
            {
                Smiles = smiles,
                Formula = formula,
                inChIKey = InChIKey,
                ExactMass = exactMass,
                LogP = logP
            };

            return meta;
        }


        public static void extractOntologies(string inputInChIKey, string dictionaryfile, string output)
        {
            var inchikeys = new List<string>();
            using (var sr = new StreamReader(inputInChIKey, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    if (line == string.Empty)
                        continue;

                    //var shortInChIKey = line.Split('=', '-')[1];
                    var shortInChIKey = line.Split('=', '-')[0];

                    inchikeys.Add(shortInChIKey);
                }
            }

            var pairs = new Dictionary<string, string>(); //[0]inchikey [1]ontology
            using (var sr = new StreamReader(dictionaryfile, Encoding.ASCII))
            {
                sr.ReadLine();
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    if (line == string.Empty)
                        continue;

                    var lineArray = line.Split('\t');
                    var shortInChIKey = lineArray[0].Split('-')[0];
                    if (!pairs.ContainsKey(shortInChIKey))
                        pairs[shortInChIKey] = lineArray[1];
                }
            }

            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {
                foreach (var key in inchikeys)
                {

                    if (pairs.ContainsKey(key))
                        sw.WriteLine(key + "\t" + pairs[key]);
                    else
                        sw.WriteLine(key + "\t" + "NA");
                }
            }
        }

        public static void getMetadataFromSDF(string sdfFile, string output)
        {
            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {
                using (var sr = new StreamReader(sdfFile, Encoding.ASCII))
                {
                    while (sr.Peek() > -1)
                    {
                        var line = sr.ReadLine();
                        if (line == string.Empty)
                        {
                            continue;
                        }

                        if (line[0] != ' ')
                        {
                            sw.WriteLine(line);
                        }

                    }

                };
            }
        }

        public static void getClassyOntologydataFromSDF(string sdfFile, string output)
        {
            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {
                using (var sr = new StreamReader(sdfFile, Encoding.ASCII))
                {
                    bool writeLine = false;
                    var line = sr.ReadLine();
                    sw.Write(line + "\t");

                    while (sr.Peek() > -1)
                    {
                        line = sr.ReadLine();
                        if (line == string.Empty)
                        {
                            continue;
                        }
                        //if (line[0] == 'Q') {
                        //    sw.Write(line + "\t");
                        //}
                        else if (line.Contains("<InChIKey>") || line.Contains("<Direct Parent>"))
                        {
                            writeLine = true;
                            //sw.Write(line + "\t");
                        }
                        else if (writeLine)
                        {
                            sw.Write(line.Replace("InChIKey=", "") + "\t");
                            writeLine = false;
                        }
                        else if (line.Contains("$$$"))
                        {
                            sw.WriteLine("");
                            writeLine = true;
                        }
                        else
                        {
                            continue;
                        }

                    }

                }
            }
        }

        public static void mergeOntologyToMsp(string inputmsp, string ontologyFile)
        {
            var ontologyDic = new Dictionary<string, string>();
            var storages = GetMspStorages(inputmsp);
            var blankOntologyList = new List<string>();
            using (var sr = new StreamReader(ontologyFile, true))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    var linearray = line.Split('\t');
                    var inchikeyArray = linearray[0].Split('-');
                    if (!ontologyDic.ContainsKey(linearray[0]))
                    {
                        ontologyDic.Add(inchikeyArray[0], linearray[1]);
                    }
                }
            }

            using (var sw = new StreamWriter(inputmsp + "_mergedOntology.msp", false, Encoding.ASCII))
            {
                foreach (var storage in storages)
                {
                    var inchikeyArray = storage.InchiKey.Split('-');
                    if (ontologyDic.ContainsKey(inchikeyArray[0]))
                    {
                        storage.Ontology = ontologyDic[inchikeyArray[0]];
                    }
                    if (storage.Ontology == "" || storage.Ontology == null)
                    {
                        blankOntologyList.Add(storage.Name + "\t" + storage.InchiKey);
                    }
                    writeMspFields(storage, sw);
                }
            }

            using (var sw = new StreamWriter(inputmsp + "_noOntology.txt", false, Encoding.ASCII))
            {
                foreach (var item in blankOntologyList)
                {
                    sw.WriteLine(item);
                }
            }
        }
    }
}
