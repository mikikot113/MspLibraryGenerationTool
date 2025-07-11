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
