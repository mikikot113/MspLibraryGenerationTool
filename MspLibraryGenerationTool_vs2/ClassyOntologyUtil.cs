using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MspLibraryGenerationTool
{
    internal class ClassyOntologyUtil
    {
        public static void GenerateInchikeyClassyfireDBList(string inChIKeyOntologyList, string chemontIDList, string output)
        {
            var classyOntologyDictionary = new Dictionary<string, string>();
            using (var sr = new StreamReader(chemontIDList, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine().Trim();
                    if (line == string.Empty) break;
                    var lineArray = line.Split('\t');
                    classyOntologyDictionary.Add(lineArray[1], lineArray[0]);
                }
            }
            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {
                using (var sr = new StreamReader(inChIKeyOntologyList, Encoding.ASCII))
                {
                    while (sr.Peek() > -1)
                    {
                        var line = sr.ReadLine().Trim();
                        if (line == string.Empty) break;
                        var lineArray = line.Split('\t');
                        //[0] InChIKey [1]Classyfire ontology


                        if (classyOntologyDictionary.ContainsKey(lineArray[2]))
                        {
                            var classyID = classyOntologyDictionary[lineArray[2]];
                            sw.WriteLine(lineArray[1] + "\t" + lineArray[2] + "\t" + classyID);
                            //[0] InChIKey [1]Classyfire ontology [2] Classyfire ID 

                        }
                    }
                }
            }
        }

        public static void GetClassyOntologydataFromSDF(string sdfFile, string output)
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
                        else if (line.Contains("<InChIKey>") || line.Contains("<Direct Parent>"))
                        {
                            writeLine = true;
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


        public static List<MspStorage> AddOntology(List<MspStorage> mspStorage, string[] ontologyList, string inputMsp, string outputFilePath)
        {
            var NewMspStorage = new List<MspStorage>();
            var ontologyDic = OntorogyDic(ontologyList);
            var noOntology = new List<string>();
            foreach (var msp in mspStorage)
            {
                if (msp != null && (msp.Ontology != null || msp.Ontology != ""))
                {
                    if (msp.InchiKey != null)
                    {
                        var shortInChIKey = msp.InchiKey.Split('-')[0];
                        if (ontologyDic.ContainsKey(shortInChIKey))
                        {
                            msp.Ontology = ontologyDic[shortInChIKey];
                        }
                        if (msp.Ontology == null || msp.Ontology == "")
                        {
                            noOntology.Add(msp.Name + "\t" + msp.InchiKey + "\t" + msp.Smiles);
                        }
                    }
                }
                NewMspStorage.Add(msp);
            }

            File.WriteAllLines(inputMsp.Replace(".msp", "_noOntology.txt"), noOntology);

            return NewMspStorage;
        }

        public static Dictionary<string, string> OntorogyDic(string[] input)
        {
            var ontologyDic = new Dictionary<string, string>();
            foreach (var item in input)
            {
                using (var sr = new StreamReader(item, true))
                {
                    while (sr.Peek() > -1)
                    {
                        var line = sr.ReadLine();
                        var lineArray = line.Split('\t');
                        var shortInChIKey = lineArray[0].Split('-')[0];
                        if (!ontologyDic.ContainsKey(shortInChIKey))
                        {
                            ontologyDic[shortInChIKey] = lineArray[1];
                        }
                    }
                }
            }
            return ontologyDic;
        }

    }
}
