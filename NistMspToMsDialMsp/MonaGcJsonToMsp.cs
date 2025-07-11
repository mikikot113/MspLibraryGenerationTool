using NistSdfToMspConvert;
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
using System.Runtime.Serialization.Json;
using NistMspToMsDialMsp.property;

namespace NistMspToMsDialMsp
{
    public class MonaGcJsonToMsp
    {
        public static void extractMonaJsonToTsv(string readFile, string tsvOutfile) // jsonからtsv出力
        {
            var file = new StreamReader(readFile);
            string line = "";
            var smilesList = new List<string>();
            using (StreamWriter sw = new StreamWriter(tsvOutfile, false, Encoding.ASCII))
            {
                sw.WriteLine(String.Join("\t", new string[] { "Name", "PrecursorMZ", "PrecursorType", "Formula", "ExactMass", "ExactMass2", "Ontology", "InchiKey", "Smiles",
                        "KovatsRetentionIndex", "FameRetentionIndex", "Ionmode", "InstrumentType", "Instrument", "Spectrum", "Comment01", "Comment02" }));

                while ((line = file.ReadLine()) != null)
                {
                    var json = line; // 行の内容をjson変数に格納
                    if (json.Length < 2) continue;//はじめと最後の行をskip

                    var serializer = new DataContractJsonSerializer(typeof(RootObject));

                    var ms = new MemoryStream(Encoding.UTF8.GetBytes((json)));
                    ms.Seek(0, SeekOrigin.Begin);
                    var data = serializer.ReadObject(ms) as RootObject;

                    // デシリアライズ
                    var jsonName = data.compound[0].names[0].name;

                    var jsonPrecursorMZ = "";
                    var jsonPrecursorType = "";
                    var jsonFormula = "";
                    var jsonOntology = "";
                    var jsonInchiKey = "";
                    var jsonSmiles = "";
                    //                    var jsonRetentiontime = "";
                    //                    var jsonCCS = "";
                    var jsonIonmode = ""; // 
                    var jsonInstrumentType = "";
                    var jsonInstrument = "";
                    //                    var jsonCollisionEnergy = "";
                    var jsonKovatsRetentionIndex = "";
                    var jsonFameRetentionIndex = "";


                    // comment用データ
                    var jsonComment01 = data.id; //MoNA ID
                    var jsonComment02 = data.library.description; //library

                    // exactMass <- MoNA キュレートのexactMass(Compound-metadata)　exactMass2 <- 元データのexactMass(metaData)
                    var jsonExactMass = "";
                    var jsonExactMass2 = "";

                    //metaDataから取得
                    var metaDataJson = data.metaData;

                    for (int i = 0; i < metaDataJson.Count; i++)
                    {
                        var meta = metaDataJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "instrument type":
                                jsonInstrumentType = metaValue;
                                break;
                            case "instrument":
                                jsonInstrument = metaValue;
                                break;
                            //case "collision energy":
                            //    jsonCollisionEnergy = metaValue;
                            //    break;
                            case "precursor m/z":
                                jsonPrecursorMZ = metaValue;
                                break;
                            case "precursor type":
                                jsonPrecursorType = metaValue;
                                break;
                            case "ionization mode":
                                jsonIonmode = metaValue;
                                break;
                            case "Kovats retention index":
                                jsonKovatsRetentionIndex = metaValue;
                                break;
                            case "FAME retention index":
                                jsonFameRetentionIndex = metaValue;
                                break;
                            case "exact mass":
                                jsonExactMass2 = metaValue;
                                break;
                        }
                    }

                    //Compound-metadataから取得
                    var compoundJson = data.compound[0].metaData;
                    for (int i = 0; i < compoundJson.Count; i++)
                    {
                        var meta = compoundJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "InChIKey":
                                jsonInchiKey = metaValue;
                                break;
                            case "molecular formula":
                                jsonFormula = metaValue;
                                break;
                            case "SMILES":
                                jsonSmiles = metaValue;
                                break;
                            case "total exact mass":
                                jsonExactMass = metaValue;
                                break;
                        }
                    }
                    // mustな情報が無ければスキップ
                    //if (jsonPrecursorMZ == "" || jsonPrecursorType == "" || jsonSmiles == "") { continue; }
                    //if (jsonPrecursorMZ == "n/a" || jsonPrecursorType == "n/a" || jsonSmiles == "n/a") { continue; }


                    // Ontologyを取得 (Compound-classification)
                    //Compound-metadataから取得
                    var compoundCassyJson = data.compound[0].classification;
                    for (int i = 0; i < compoundCassyJson.Count; i++)
                    {
                        var meta = compoundCassyJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "direct parent":
                                jsonOntology = metaValue;
                                break;
                        }
                    }

                    // spectrum list
                    var jsonSpectrum = data.spectrum;

                    //Writing
                    sw.WriteLine(String.Join("\t", new string[] { jsonName, jsonPrecursorMZ, jsonPrecursorType, jsonFormula, jsonExactMass, jsonExactMass2, jsonOntology, jsonInchiKey, jsonSmiles,
                        jsonKovatsRetentionIndex, jsonFameRetentionIndex, jsonIonmode, jsonInstrumentType, jsonInstrument, jsonSpectrum, jsonComment01, jsonComment02
                    }));

                    // Smiles のリストを
                    smilesList.Add(jsonSmiles);

                }
            }
            File.WriteAllLines(tsvOutfile + ".smiles", smilesList.ToArray());
        }




        public static void mergeCalcedData(string CalcedInchkey, string OntologyList, string CalcedSmiles, string CalcedFormula, string output)
        {
            var calcedInchkey = new List<string>();
            var calcedSmiles = new List<string>();
            var calcedFormula = new List<string[]>();
            var classyOntology = new List<string>();

            using (var sr = new StreamReader(CalcedInchkey, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine().Trim();
                    var lineArray = line.Split('=');
                    var inchikey = lineArray[1];
                    calcedInchkey.Add(inchikey);
                }
            }
            using (var sr = new StreamReader(OntologyList, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine().Trim();
                    classyOntology.Add(line);
                }
            }
            using (var sr = new StreamReader(CalcedFormula, Encoding.ASCII))
            {
                sr.ReadLine();
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine().Trim();
                    var lineArray = line.Split('\t');
                    if (lineArray.Length == 3)
                    {
                        calcedFormula.Add(new string[] { lineArray[1], lineArray[2] });
                    }
                    else if (lineArray.Length == 2)
                    {
                        calcedFormula.Add(new string[] { lineArray[1], "" });
                    }
                }
            }
            using (var sr = new StreamReader(CalcedSmiles, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine().Trim();
                    calcedSmiles.Add(line);
                }
            }
            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {
                if (calcedInchkey.Count != classyOntology.Count || calcedInchkey.Count != calcedFormula.Count ||
                     calcedInchkey.Count != calcedSmiles.Count)
                {
                    sw.WriteLine("Query count is not equal in {0}", output);
                    return;
                }
                for (int i = 0; i < calcedFormula.Count; i++)
                {
                    //for (int j = 0; j < jsonData[0].Length; j++)
                    //{
                    //    sw.Write(jsonData[i][j] + "\t");

                    //}
                    var formula = calcedFormula[i][1];
                    var exactmass = calcedFormula[i][0];
                    var inchikey = calcedInchkey[i];
                    var ontology = classyOntology[i];
                    var smiles = calcedSmiles[i];
                    sw.WriteLine(formula + "\t" + exactmass + "\t" + inchikey + "\t" + ontology + "\t" + smiles);
                }
            }

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

                    var shortInChIKey = line.Split('=', '-')[1];
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
                        sw.WriteLine(pairs[key]);
                    else
                        sw.WriteLine("NA");
                }
            }
        }

        public static void exportMspFile(string readFile, string mspOutfile) // jsonからtsv出力
        {
            var file = new StreamReader(readFile);
            string line = "";
            var smilesList = new List<string>();
            var mspFileName = Path.GetDirectoryName(mspOutfile) + "\\" + Path.GetFileNameWithoutExtension(mspOutfile);

            // case KovatsRI
            using (StreamWriter sw = new StreamWriter(mspFileName + "-KovatsRI.msp", false, Encoding.ASCII))
            {
                while ((line = file.ReadLine()) != null)
                {
                    var mspStorage = new MspStorage();

                    var json = line; // 行の内容をjson変数に格納
                    if (json.Length < 2) continue;//はじめと最後の行をskip

                    var serializer = new DataContractJsonSerializer(typeof(RootObject));

                    var ms = new MemoryStream(Encoding.UTF8.GetBytes((json)));
                    ms.Seek(0, SeekOrigin.Begin);
                    var data = serializer.ReadObject(ms) as RootObject;

                    // デシリアライズ
                    var jsonName = data.compound[0].names[0].name;
                    mspStorage.Name = jsonName;

                    // comment用データ
                    var jsonComment01 = data.id; //MoNA ID
                    var jsonComment02 = data.library.description; //library
                    mspStorage.Comment = "DB#=" + jsonComment01 + "; origin=" + jsonComment02;


                    //metaDataから取得
                    var metaDataJson = data.metaData;

                    for (int i = 0; i < metaDataJson.Count; i++)
                    {
                        var meta = metaDataJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "instrument type":
                                mspStorage.InstrumentType = metaValue;
                                break;
                            //case "instrument":
                            //    jsonInstrument = metaValue;
                            //    break;
                            //case "collision energy":
                            //    jsonCollisionEnergy = metaValue;
                            //    break;
                            //case "precursor m/z":
                            //    jsonPrecursorMZ = metaValue;
                            //    break;
                            //case "precursor type":
                            //    jsonPrecursorType = metaValue;
                            //    break;
                            case "ionization mode":
                                var ionMode = metaValue;

                                if (ionMode.ToUpper().Contains("N")) mspStorage.Ionmode = "Negative";
                                else if (ionMode.ToUpper().Contains("P")) mspStorage.Ionmode = "Positive";

                                break;
                            case "Kovats retention index":
                                mspStorage.RetentionIndex = metaValue;
                                break;
                            //case "FAME retention index":
                            //    jsonFameRetentionIndex = metaValue;
                            //    break;
                            case "exact mass":
                                mspStorage.Exactmass = metaValue;
                                break;
                        }
                    }

                    //Compound-metadataから取得
                    var compoundJson = data.compound[0].metaData;
                    for (int i = 0; i < compoundJson.Count; i++)
                    {
                        var meta = compoundJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "InChIKey":
                                mspStorage.InchiKey = metaValue;
                                break;
                            case "molecular formula":
                                mspStorage.Formula = metaValue;
                                break;
                            case "SMILES":
                                mspStorage.Smiles = metaValue;
                                break;
                                //case "total exact mass":
                                //    jsonExactMass = metaValue;
                                //    break;
                        }
                    }
                    // Ontologyを取得 (Compound-classification)
                    //Compound-metadataから取得
                    var compoundCassyJson = data.compound[0].classification;
                    for (int i = 0; i < compoundCassyJson.Count; i++)
                    {
                        var meta = compoundCassyJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "direct parent":
                                mspStorage.Ontology = metaValue;
                                break;
                        }
                    }

                    // spectrum list
                    var jsonSpectrum = data.spectrum;
                    // spectrum list
                    string[] spectrumArr = jsonSpectrum.Split(' ');

                    var peaks = new List<NistSdfToMspConvert.Peak>();

                    foreach (var spec in spectrumArr)
                    {
                        var mz = Double.Parse(spec.Split(':')[0]);
                        var intensity = Double.Parse(spec.Split(':')[1]);

                        peaks.Add(new NistSdfToMspConvert.Peak() { Mz = mz, Intensity = intensity });
                    }
                    peaks = peaks.OrderBy(n => n.Mz).ToList();

                    var spectrumList = new List<string>(spectrumArr);

                    //Meta data section 
                    sw.WriteLine(String.Join(": ", new string[] { "NAME", mspStorage.Name }));
                    sw.WriteLine(String.Join(": ", new string[] { "EXACTMASS", mspStorage.Exactmass }));
                    //sw.WriteLine(String.Join(": ", new string[] { "PRECURSORTYPE", mspPrecursorType }));
                    sw.WriteLine(String.Join(": ", new string[] { "FORMULA", mspStorage.Formula }));
                    sw.WriteLine(String.Join(": ", new string[] { "ONTOLOGY", mspStorage.Ontology }));
                    sw.WriteLine(String.Join(": ", new string[] { "INCHIKEY", mspStorage.InchiKey }));
                    sw.WriteLine(String.Join(": ", new string[] { "SMILES", mspStorage.Smiles }));
                    sw.WriteLine(String.Join(": ", new string[] { "RETENTIONINDEX", mspStorage.RetentionIndex }));
                    //sw.WriteLine(String.Join(": ", new string[] { "CCS", mspCCS }));
                    sw.WriteLine(String.Join(": ", new string[] { "IONMODE", mspStorage.Ionmode }));
                    sw.WriteLine(String.Join(": ", new string[] { "INSTRUMENTTYPE", mspStorage.InstrumentType }));
                    //sw.WriteLine(String.Join(": ", new string[] { "INSTRUMENT", mspInstrument }));
                    //sw.WriteLine(String.Join(": ", new string[] { "COLLISIONENERGY", mspCollisionEnergy }));
                    sw.WriteLine(String.Join(": ", new string[] { "COMMENT", mspStorage.Comment }));
                    //spectrum num
                    var numPeaks = peaks.Count.ToString();
                    sw.WriteLine(String.Join(": ", new string[] { "Num Peaks", numPeaks }));
                    //spectrum list
                    foreach (var peak in peaks)
                    {
                        //sw.WriteLine(spectrumList[i]);
                        sw.WriteLine(peak.Mz + "\t" + peak.Intensity);
                    };

                    sw.WriteLine();


                }
            }
            // case FiehnRI
            file = new StreamReader(readFile);
            using (StreamWriter sw = new StreamWriter(mspFileName + "-FiehnRI.msp", false, Encoding.ASCII))
            {
                while ((line = file.ReadLine()) != null)
                {
                    var mspStorage = new MspStorage();

                    var json = line; // 行の内容をjson変数に格納
                    if (json.Length < 2) continue;//はじめと最後の行をskip

                    var serializer = new DataContractJsonSerializer(typeof(RootObject));

                    var ms = new MemoryStream(Encoding.UTF8.GetBytes((json)));
                    ms.Seek(0, SeekOrigin.Begin);
                    var data = serializer.ReadObject(ms) as RootObject;

                    // デシリアライズ
                    var jsonName = data.compound[0].names[0].name;
                    mspStorage.Name = jsonName;

                    // comment用データ
                    var jsonComment01 = data.id; //MoNA ID
                    var jsonComment02 = data.library.description; //library
                    mspStorage.Comment = "DB#=" + jsonComment01 + "; origin=" + jsonComment02;


                    //metaDataから取得
                    var metaDataJson = data.metaData;

                    for (int i = 0; i < metaDataJson.Count; i++)
                    {
                        var meta = metaDataJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "instrument type":
                                mspStorage.InstrumentType = metaValue;
                                break;
                            //case "instrument":
                            //    jsonInstrument = metaValue;
                            //    break;
                            //case "collision energy":
                            //    jsonCollisionEnergy = metaValue;
                            //    break;
                            //case "precursor m/z":
                            //    jsonPrecursorMZ = metaValue;
                            //    break;
                            //case "precursor type":
                            //    jsonPrecursorType = metaValue;
                            //    break;
                            case "ionization mode":
                                var ionMode = metaValue;

                                if (ionMode.ToUpper().Contains("N")) mspStorage.Ionmode = "Negative";
                                else if (ionMode.ToUpper().Contains("P")) mspStorage.Ionmode = "Positive";

                                break;
                            //case "Kovats retention index":
                            //    mspStorage.RetentionIndex = metaValue;
                            //    break;
                            case "FAME retention index":
                                mspStorage.RetentionIndex = metaValue;
                                break;
                            case "exact mass":
                                mspStorage.Exactmass = metaValue;
                                break;
                        }
                    }

                    //Compound-metadataから取得
                    var compoundJson = data.compound[0].metaData;
                    for (int i = 0; i < compoundJson.Count; i++)
                    {
                        var meta = compoundJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "InChIKey":
                                mspStorage.InchiKey = metaValue;
                                break;
                            case "molecular formula":
                                mspStorage.Formula = metaValue;
                                break;
                            case "SMILES":
                                mspStorage.Smiles = metaValue;
                                break;
                                //case "total exact mass":
                                //    jsonExactMass = metaValue;
                                //    break;
                        }
                    }
                    // Ontologyを取得 (Compound-classification)
                    //Compound-metadataから取得
                    var compoundCassyJson = data.compound[0].classification;
                    for (int i = 0; i < compoundCassyJson.Count; i++)
                    {
                        var meta = compoundCassyJson[i];
                        var metaName = meta.name;
                        var metaValue = meta.value.ToString();
                        switch (metaName)
                        {
                            case "direct parent":
                                mspStorage.Ontology = metaValue;
                                break;
                        }
                    }

                    // spectrum list
                    var jsonSpectrum = data.spectrum;
                    // spectrum list
                    string[] spectrumArr = jsonSpectrum.Split(' ');

                    var peaks = new List<NistSdfToMspConvert.Peak>();

                    foreach (var spec in spectrumArr)
                    {
                        var mz = Double.Parse(spec.Split(':')[0]);
                        var intensity = Double.Parse(spec.Split(':')[1]);

                        peaks.Add(new NistSdfToMspConvert.Peak() { Mz = mz, Intensity = intensity });
                    }
                    peaks = peaks.OrderBy(n => n.Mz).ToList();

                    var spectrumList = new List<string>(spectrumArr);

                    //Meta data section 
                    sw.WriteLine(String.Join(": ", new string[] { "NAME", mspStorage.Name }));
                    sw.WriteLine(String.Join(": ", new string[] { "EXACTMASS", mspStorage.Exactmass }));
                    //sw.WriteLine(String.Join(": ", new string[] { "PRECURSORTYPE", mspPrecursorType }));
                    sw.WriteLine(String.Join(": ", new string[] { "FORMULA", mspStorage.Formula }));
                    sw.WriteLine(String.Join(": ", new string[] { "ONTOLOGY", mspStorage.Ontology }));
                    sw.WriteLine(String.Join(": ", new string[] { "INCHIKEY", mspStorage.InchiKey }));
                    sw.WriteLine(String.Join(": ", new string[] { "SMILES", mspStorage.Smiles }));
                    sw.WriteLine(String.Join(": ", new string[] { "RETENTIONINDEX", mspStorage.RetentionIndex }));
                    //sw.WriteLine(String.Join(": ", new string[] { "CCS", mspCCS }));
                    sw.WriteLine(String.Join(": ", new string[] { "IONMODE", mspStorage.Ionmode }));
                    sw.WriteLine(String.Join(": ", new string[] { "INSTRUMENTTYPE", mspStorage.InstrumentType }));
                    //sw.WriteLine(String.Join(": ", new string[] { "INSTRUMENT", mspInstrument }));
                    //sw.WriteLine(String.Join(": ", new string[] { "COLLISIONENERGY", mspCollisionEnergy }));
                    sw.WriteLine(String.Join(": ", new string[] { "COMMENT", mspStorage.Comment }));
                    //spectrum num
                    var numPeaks = peaks.Count.ToString();
                    sw.WriteLine(String.Join(": ", new string[] { "Num Peaks", numPeaks }));
                    //spectrum list
                    foreach (var peak in peaks)
                    {
                        //sw.WriteLine(spectrumList[i]);
                        sw.WriteLine(peak.Mz + "\t" + peak.Intensity);
                    };

                    sw.WriteLine();


                }
            }

        }

    }
}
