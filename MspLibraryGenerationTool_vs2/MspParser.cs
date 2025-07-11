using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MspLibraryGenerationTool
{
    public static class MspParser
    {
         public static List<MspStorage> MspFileParser(string mspFile)
        {
            var mspList = new List<MspStorage>();
            using (StreamReader sr = new StreamReader(mspFile, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var wkstr = sr.ReadLine();
                    var isRecordStarted = wkstr.StartsWith("NAME:");
                    if (isRecordStarted)
                    {
                        var mspField = new MspStorage();

                        ReadMspField(wkstr, out string titleName, out string titleValue); // read Name field
                        SetMspField(mspField, titleName, titleValue, sr);

                        while (sr.Peek() > -1)
                        {
                            wkstr = sr.ReadLine();
                            ReadMspField(wkstr, out string fieldName, out string fieldValue);
                            var isSpecRetrieved = SetMspField(mspField, fieldName, fieldValue, sr);
                            if (isSpecRetrieved) break;
                        }
                        mspList.Add(mspField);
                    }
                }
            }
            return mspList;
        }

        public static void ReadMspField(string wkstr, out string fieldName, out string fieldValue)
        {
            if(wkstr.Contains(':'))
            {
                fieldName = wkstr.Split(':')[0];
                fieldValue = wkstr.Substring(fieldName.Length + 1).Trim();
            }
            else
            {
                fieldName = "Comment";
                fieldValue = wkstr;
            }
        }

        public static bool SetMspField(MspStorage mspObj, string fieldName, string fieldValue, StreamReader sr)
        {
            // if (fieldValue.IsEmptyOrNull()) return false; // this code has some bug ?
            switch (fieldName.ToUpper())
            {
                // string
                case "NAME": mspObj.Name = fieldValue; return false;
                case "PRECURSORMZ": mspObj.PrecursorMz = fieldValue; return false;
                case "ADDUCTIONNAME":
                case "PRECURSORTYPE":
                    mspObj.PrecursorType = fieldValue;
                    return false;
                case "INSTRUMENTTYPE": mspObj.InstrumentType = fieldValue; return false;
                case "INSTRUMENT": mspObj.Instrument = fieldValue; return false;
                case "SMILES": mspObj.Smiles = fieldValue; return false;
                case "INCHIKEY":
                    mspObj.InchiKey = fieldValue;
                    return false;
                case "INCHI": mspObj.Inchi = fieldValue; return false;
                case "FORMULA":
                    mspObj.Formula = fieldValue;
                    return false;
                case "RETENTIONTIME":
                    mspObj.Retentiontime = fieldValue;
                    return false;
                case "CCS":
                    mspObj.CollisionCrossSection = fieldValue;
                    return false;
                case "IONMODE": mspObj.Ionmode = fieldValue; return false;
                case "LINKS": mspObj.Links = fieldValue; return false;
                case "COMMENT": mspObj.Comment = fieldValue; return false;
                case "ONTOLOGY": mspObj.Ontology = fieldValue; return false;
                case "SPLASH": mspObj.Splash = fieldValue; return false;
                case "COLLISIONENERGY": mspObj.CollisionEnergy = fieldValue; return false;

                case "NUM PEAKS":
                case "NUMPEAKS":
                case "NUM_PEAKS":
                    mspObj.Peaks = ReadSpectrum(sr, fieldValue);
                    mspObj.Peaknum = mspObj.Peaks.Count.ToString();
                    return true;

            }
            return false;
        }


        public static List<MspPeak> ReadSpectrum(StreamReader sr, string numPeakField)
        {
            var mspPeaks = new List<MspPeak>();

            if (int.TryParse(numPeakField.Trim(), out int peaknum))
            {
                if (peaknum == 0) { return mspPeaks; }

                var pairCount = 0;
                var mspPeak = new MspPeak();

                while (pairCount < peaknum)
                {
                    var wkstr = sr.ReadLine();
                    if (wkstr == null) break;
                    if (wkstr == string.Empty) break;
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
                                        mspPeak.Mz = double.Parse(numChar);
                                        mzFill = true;
                                        numChar = string.Empty;
                                    }
                                }
                                else if (mzFill == true)
                                {
                                    if (numChar != string.Empty)
                                    {
                                        mspPeak.Intensity = double.Parse(numChar);
                                        mzFill = false;
                                        numChar = string.Empty;

                                        if (mspPeak.Comment == null)
                                        {

                                        }
                                        mspPeaks.Add(mspPeak);
                                        mspPeak = new MspPeak();
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
                                mspPeaks[mspPeaks.Count - 1].Comment = letterChar;
                            else
                            {
                                mspPeaks[mspPeaks.Count - 1].Comment = letterChar.Split(new string[] { "_f_" }, StringSplitOptions.None)[0];
                                //mspPeaks[mspPeaks.Count - 1].Frag = letterChar.Split(new string[] { "_f_" }, StringSplitOptions.None)[1];
                            }

                        }
                        else
                        {
                            if (mzFill == false)
                            {
                                if (numChar != string.Empty)
                                {
                                    mspPeak.Mz = double.Parse(numChar);
                                    mzFill = true;
                                    numChar = string.Empty;
                                }
                            }
                            else if (mzFill == true)
                            {
                                if (numChar != string.Empty)
                                {
                                    mspPeak.Intensity = double.Parse(numChar);
                                    mzFill = false;
                                    numChar = string.Empty;

                                    if (mspPeak.Comment == null)
                                    {

                                    }

                                    mspPeaks.Add(mspPeak);
                                    mspPeak = new MspPeak();
                                    pairCount++;
                                }
                            }
                        }
                    }
                }

                mspPeaks = mspPeaks.OrderBy(n => n.Mz).ToList();
            }
            return setPeaks(mspPeaks);
        }

        public static List<MspPeak> setPeaks(List<MspPeak> input)
        {
            var peaks = new List<MspPeak>();
            var peakItem = input.Select(n=>n.Intensity);
            var maxIntensityValue = input.Select(n => n.Intensity).Max();
            var normalizationFactor = 1.0;
            if (maxIntensityValue != 100.0)
            {
                normalizationFactor = 100.0 / maxIntensityValue;
            }
            foreach (var item in input)
            {
                var mz = Math.Round(item.Mz, 3);
                var intensity = Math.Round(item.Intensity * normalizationFactor * 10);
                if (intensity < 10)
                {
                    continue;
                }
                if (intensity == 999)
                {
                    intensity = 1000;
                }
                var peak = new MspPeak()
                {
                    Mz = mz,
                    Intensity = intensity,
                };
                peaks.Add(peak);
            }
            peaks = peaks.OrderBy(n => n.Mz).ToList();
            return peaks;
        }

        public static void writeMspFields(MspStorage mspStorage, StreamWriter sw)
        {
            sw.WriteLine("NAME: " + mspStorage.Name);
            sw.WriteLine("PRECURSORMZ: " + mspStorage.PrecursorMz);
            sw.WriteLine("PRECURSORTYPE: " + mspStorage.PrecursorType);
            sw.WriteLine("IONMODE: " + mspStorage.Ionmode);
            if (!String.IsNullOrEmpty(mspStorage.Formula))
            {
                sw.WriteLine("FORMULA: " + mspStorage.Formula);
            }
            if (!String.IsNullOrEmpty(mspStorage.Smiles))
            {
                sw.WriteLine("SMILES: " + mspStorage.Smiles);
            }
            //sw.WriteLine("INCHI: " + mspStorage.Inchi);
            if (!String.IsNullOrEmpty(mspStorage.InchiKey))
            {
                sw.WriteLine("INCHIKEY: " + mspStorage.InchiKey);
            }
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
            if (!String.IsNullOrEmpty(mspStorage.Ontology))
            {
                sw.WriteLine("ONTOLOGY: " + mspStorage.Ontology);
            }
            if (!String.IsNullOrEmpty(mspStorage.Splash))
            {
                sw.WriteLine("SPLASH: " + mspStorage.Splash);
            }
            sw.WriteLine("COMMENT: " + mspStorage.Comment);

            sw.WriteLine("Num Peaks: " + mspStorage.Peaknum);
            foreach (var peak in mspStorage.Peaks)
            {
                sw.WriteLine(peak.Mz + "\t" + peak.Intensity);
            }
            sw.WriteLine();
        }

    }
    public class MspStorage
    {
        private string name;
        private string precursorMz;
        private string precursorType;
        private string instrumentType;
        private string instrument;
        private string authors;
        private string license;
        private string smiles;
        private string tmsSmiles;

        private string inchi;
        private string inchiKey;
        private string basepeakIntensity;
        private string comment;
        private string collisionEnergy;
        private string formula;
        private string retentiontime;
        private string retentionIndex;
        private string compoundClass;
        private string id;
        private string challengename;
        private string xlogP;
        private string sssCH2;
        private string meanI;
        private string mslevel;
        private string exactmass;
        private string binID;
        private string quantMass;
        private string ontology;
        private string ontologyID;
        private string ionmode;
        private string ionization;
        private string charge;
        private string links;
        private string massBankAccession;
        private string peaknum;
        private string cas;

        private string collisionCrossSection;

        private string splash;

        private List<MspPeak> peaks;

        public MspStorage()
        {
            peaks = new List<MspPeak>();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string PrecursorMz
        {
            get { return precursorMz; }
            set { precursorMz = value; }
        }

        public string PrecursorType
        {
            get { return precursorType; }
            set { precursorType = value; }
        }

        public string Exactmass
        {
            get { return exactmass; }
            set { exactmass = value; }
        }

        public string Mslevel
        {
            get { return mslevel; }
            set { mslevel = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string BinID
        {
            get { return binID; }
            set { binID = value; }
        }

        public string Challengename
        {
            get { return challengename; }
            set { challengename = value; }
        }

        public string InstrumentType
        {
            get { return instrumentType; }
            set { instrumentType = value; }
        }

        public string Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        public string Authors
        {
            get { return authors; }
            set { authors = value; }
        }

        public string QuantMass
        {
            get { return quantMass; }
            set { quantMass = value; }
        }

        public string License
        {
            get { return license; }
            set { license = value; }
        }

        public string Smiles
        {
            get { return smiles; }
            set { smiles = value; }
        }

        public string TmsSmiles
        {
            get { return tmsSmiles; }
            set { tmsSmiles = value; }
        }

        public string Inchi
        {
            get { return inchi; }
            set { inchi = value; }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public string RetentionIndex
        {
            get { return retentionIndex; }
            set { retentionIndex = value; }
        }

        public string CollisionEnergy
        {
            get { return collisionEnergy; }
            set { collisionEnergy = value; }
        }

        public string Formula
        {
            get { return formula; }
            set { formula = value; }
        }

        public string Retentiontime
        {
            get { return retentiontime; }
            set { retentiontime = value; }
        }

        public string MassBankAccession
        {
            get { return massBankAccession; }
            set { massBankAccession = value; }
        }

        public string Charge
        {
            get { return charge; }
            set { charge = value; }
        }

        public string Ionmode
        {
            get { return ionmode; }
            set { ionmode = value; }
        }

        public string Ionization
        {
            get { return ionization; }
            set { ionization = value; }
        }

        public string Links
        {
            get { return links; }
            set { links = value; }
        }

        public string Peaknum
        {
            get { return peaknum; }
            set { peaknum = value; }
        }

        public List<MspPeak> Peaks
        {
            get { return peaks; }
            set { peaks = value; }
        }

        public string InchiKey
        {
            get { return inchiKey; }
            set { inchiKey = value; }
        }

        public string CompoundClass
        {
            get { return compoundClass; }
            set { compoundClass = value; }
        }

        public string XlogP
        {
            get { return xlogP; }
            set { xlogP = value; }
        }

        public string SssCH2
        {
            get { return sssCH2; }
            set { sssCH2 = value; }
        }

        public string MeanI
        {
            get { return meanI; }
            set { meanI = value; }
        }

        public string Cas
        {
            get { return cas; }
            set { cas = value; }
        }

        public string Ontology
        {
            get
            {
                return ontology;
            }

            set
            {
                ontology = value;
            }
        }

        public string OntologyID
        {
            get
            {
                return ontologyID;
            }

            set
            {
                ontologyID = value;
            }
        }

        public string BasepeakIntensity
        {
            get
            {
                return basepeakIntensity;
            }

            set
            {
                basepeakIntensity = value;
            }
        }

        public string CollisionCrossSection
        {
            get
            {
                return collisionCrossSection;
            }

            set
            {
                collisionCrossSection = value;
            }
        }
        public string Splash
        {
            get
            {
                return splash;
            }

            set
            {
                splash = value;
            }
        }

    }
    public class MspPeak
    {
        double mz;
        double intensity;
        string comment;
        string frag;

        public double Mz
        {
            get { return mz; }
            set { mz = value; }
        }

        public double Intensity
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
