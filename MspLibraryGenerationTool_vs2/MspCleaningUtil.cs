using NCDK;
using NCDK.Default;
using NCDK.Graphs.InChI;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MspLibraryGenerationTool
{
    public class MspCleaningUtil
    {
        public static List<MspStorage> MspCleaner(List<MspStorage> mspStorage, string inputMsp, string outputFilePath)
        {
            var CleanedMspstorage = new List<MspStorage>();
            var ExcludeList = new List<string>();

            foreach (var item in mspStorage)
            {
                var msp = MspCleaning(item);
                if (msp.MspStorage != null)
                {
                    CleanedMspstorage.Add(msp.MspStorage);
                }
                else if (msp.ExcludeItem != null)
                {
                    ExcludeList.Add(msp.ExcludeItem);
                }
            }
            //クリーニングの結果を出力
            if (ExcludeList.Count > 0)
            {
                var ExcludeFileName = Path.Combine(outputFilePath, $"{Path.GetFileNameWithoutExtension(inputMsp)}_Exclude.txt");
                using (var sw = new StreamWriter(ExcludeFileName, false, Encoding.UTF8))
                {
                    foreach (var item in ExcludeList)
                    {
                        sw.WriteLine(item);
                    }
                }
            }
            return CleanedMspstorage;
        }
        public static CheckItem MspCleaning(MspStorage msp)
        {
            var exclude = "";
            if (msp.PrecursorMz == null || msp.Formula == null || msp.Formula == "")
            {
                exclude = "no Data\t" + msp.Name + "\t" + msp.Formula + "\t" + msp.Comment + "\traw PrecursorMz: " + msp.PrecursorMz + "\traw Formula: " + msp.Formula;
                return new CheckItem { MspStorage = null, ExcludeItem = exclude };
            }
            // adductを変換
            if (Dic.GnpsAdductToMF.ContainsKey(msp.PrecursorType))
            {
                msp.PrecursorType = Dic.GnpsAdductToMF[msp.PrecursorType];
            }
            // PrecursorType と ionmodeが不一致の場合はスキップ
            var precursorSuffix = "";
            try
            {
                precursorSuffix = msp.PrecursorType.Substring(msp.PrecursorType.Length - 1, 1);
            }
            catch
            {
                exclude = "ionmode unmatch\t" + msp.Name + "\t" + msp.Formula + "\t" + msp.Comment + "\traw PrecursorType: " + msp.PrecursorType + "\traw Ionmode: " + msp.Ionmode;
                return new CheckItem { MspStorage = null, ExcludeItem = exclude };
            }
            precursorSuffix = msp.PrecursorType.Substring(msp.PrecursorType.Length - 1, 1);
            var ionmodeCase = "+";
            if (msp.Ionmode == "Negative") { ionmodeCase = "-"; }
            if (precursorSuffix != ionmodeCase)
            {
                exclude = "ionmode unmatch\t" + msp.Name + "\t" + msp.Formula + "\t" + msp.Comment + "\traw PrecursorType: " + msp.PrecursorType + "\traw Ionmode: " + msp.Ionmode;
                return new CheckItem { MspStorage = null, ExcludeItem = exclude };
            }

            if (!msp.Comment.Contains("registered in NIST 20") && !msp.Comment.Contains("registered in NIST14") && !msp.Comment.Contains("PlaSMA"))
            {
                // PrecursorMZを計算
                IAtomContainer molecule = null;
                if (!string.IsNullOrWhiteSpace(msp.Smiles))
                {
                    // SMILES を先に試す
                    try
                    {
                        var SmilesParser = new SmilesParser();
                        molecule = SmilesParser.ParseSmiles(msp.Smiles);
                    }
                    catch (InvalidSmilesException)
                    {
                        molecule = null;
                    }
                }

                if (molecule == null &&
                    !string.IsNullOrWhiteSpace(msp.Inchi) &&
                    msp.Inchi.StartsWith("InChI="))
                {
                    // SMILES 取得に失敗した or そもそも無い場合のみ InChI をパース
                    try
                    {
                        var result = InChIToStructure.FromInChI(msp.Inchi, ChemObjectBuilder.Instance);
                        molecule = result.AtomContainer;
                        var SmilesGenerator = new SmilesGenerator(SmiFlavors.Isomeric);
                        msp.Smiles = SmilesGenerator.Create(molecule);
                    }
                    catch (Exception)
                    {
                        return new CheckItem { MspStorage = null, ExcludeItem = exclude };
                    }
                }

                if (molecule == null)
                {
                    // どちらも取得できなかった場合はリターン
                    return new CheckItem { MspStorage = null, ExcludeItem = exclude };
                }

                double calcedExactMass = AtomContainerManipulator
                    .GetMass(molecule, MolecularWeightTypes.MonoIsotopic);

                double calcedPrecursorMZ = 0.0;
                if (Dic.AdductToCalc.ContainsKey(msp.PrecursorType))
                {
                    calcedPrecursorMZ = calcedExactMass + Dic.AdductToCalc[msp.PrecursorType];
                }
                else
                {
                    exclude = "wrong adduct type\t" + msp.Name + "\t" + msp.Formula + "\t" + msp.Comment + "\traw PrecursorType: " + msp.PrecursorType + "\traw Ionmode: " + msp.Ionmode;
                    return new CheckItem { MspStorage = null, ExcludeItem = exclude };
                }

                var xMer = msp.PrecursorType[1];
                if (xMer == '2') { calcedPrecursorMZ = calcedPrecursorMZ * 2 - Dic.AdductToCalc[msp.PrecursorType]; }
                int chargeNum = int.Parse(ExtractNumberAfterBracket(msp.PrecursorType));
                if (chargeNum > 1)
                {
                    calcedPrecursorMZ = calcedPrecursorMZ / chargeNum;
                }

                // 計算されたPrecursorMZが元データのPrecursorMZと乖離していたらスキップ
                if (Math.Abs(double.Parse(msp.PrecursorMz) - calcedPrecursorMZ) > 0.5)
                {
                    exclude = "precursorMZ unmatch\t" + msp.Name + "\t" + msp.Formula + "\t" + msp.Comment + "\traw PrecursorMz: " + msp.PrecursorMz + "\tcalucurated PrecursorMz: " + calcedPrecursorMZ + "\traw PrecursorType: " + msp.PrecursorType;
                    return new CheckItem { MspStorage = null, ExcludeItem = exclude };
                }
                else
                {
                    msp.PrecursorMz = calcedPrecursorMZ.ToString();
                }
            }
            if (!msp.PrecursorType.Contains("M"))
            {
                exclude = "wrong adduct type\t" + msp.Name + "\t" + msp.Formula + "\t" + msp.Comment + "\traw PrecursorType: " + msp.PrecursorType + "\traw Ionmode: " + msp.Ionmode;
                return new CheckItem { MspStorage = null, ExcludeItem = exclude };
            }
            return new CheckItem { MspStorage = msp, ExcludeItem = null };
        }


        public static void ModeSeparator(string Msp)
        {
            var mspStorage = MspParser.MspFileParser(Msp);
            using (var swNeg = new StreamWriter(
                Msp.Replace(".msp", "_Neg.msp"), false, Encoding.ASCII))
            {
                using (var swPos = new StreamWriter(
                        Msp.Replace(".msp", "_Pos.msp"), false, Encoding.ASCII))
                {
                    foreach (var msp in mspStorage)
                    {
                        if (msp != null)
                        {
                            if (msp.Ionmode.ToUpper().StartsWith("P"))
                            {
                                MspParser.writeMspFields(msp, swPos);
                            }
                            else if (msp.Ionmode.ToUpper().StartsWith("N"))
                            {
                                MspParser.writeMspFields(msp, swNeg);
                            }
                        }
                    }
                }
            }
        }

        static string ExtractNumberAfterBracket(string s)
        {
            int idx = s.IndexOf(']');
            if (idx < 0 || idx + 1 >= s.Length) return null;

            // ] の次の文字から数字が続く限り取得
            string result = "";
            for (int i = idx + 1; i < s.Length; i++)
            {
                if (char.IsDigit(s[i]))
                    result += s[i];
                else
                    break;
            }
            return result == "" ? "1" : result;
        }

        public class CheckItem
        {
            public MspStorage MspStorage { get; set; }
            public string ExcludeItem { get; set; }
        }


    }
    public class Dic
    {
        public static Dictionary<string, string> GnpsAdductToMF = new Dictionary<string, string>()
        {
            {"[M+2H]", "[M+2H]2+"},
            {"[M+H]", "[M+H]+"},
            {"[M+K]", "[M+K]+"},
            {"[M+Na]", "[M+Na]+"},
            {"[M-H2O]", "[M-H2O]+"},
            {"2M+H", "[2M+H]+"},
            {"2M+Na", "[2M+Na]+"},
            {"M", "[M]+"},
            {"M+", "[M]+"},
            {"M+23", "[M+Na]+"},
            {"M+2H", "[M+2H]2+"},
            {"M+2H/2", "[M+H]+"},
            {"M+2K-H", "[M+2K-H]+"},
            {"M+2Na", "[M+2Na]2+"},
            {"M+2Na+K-2H", "[M+2Na+K-2H]+"},
            {"M+2Na-H", "[M+2Na-H]+"},
            {"M+3H", "[M+3H]3+"},
            {"M+ACN+H", "[M+ACN+H]+"},
            {"M+Cl", "[M+Cl]+"},
            {"M+H", "[M+H]+"},
            {"M+H+K", "[M+H+K]2+"},
            {"M+H+Na", "[M+H+Na]2+"},
            {"M+HCl", "[M+HCl]+"},
            {"M+H-H20", "[M+H-H2O]+"},
            {"M+H-H2O", "[M+H-H2O]+"},
            {"M+K+H2O", "[M+K+H2O]+"},
            {"M+Na", "[M+Na]+"},
            {"M+Na+2K-2H", "[M+Na+2K-2H]+"},
            {"M+Na+H2O", "[M+Na+H2O]+"},
            {"M+Na+K", "[M+Na+K]2+"},
            {"M+Na+K-H", "[M+Na+K-H]+"},
            {"M+Na-2H", "[M+Na-2H]+"},
            {"M+NH4", "[M+NH4]+"},
            {"M+OH", "[M+OH]+"},
            {"M-18", "[M-H2O]+"},
            {"M-2H", "[M-2H]2-"},
            {"M-H", "[M-H]-"},
            {"M-H2O", "[M-H2O]+"},
            {"M-H2O+H", "[M-H2O+H]+"},
            {"M-MeOH+H", "[M-CH4O+H]+"},
            {"M-NH3", "[M-NH3]+"},
            {"M-NH3+H", "[M-NH3+H]+"},
            //
            {"[M-H]1-", "[M-H]-"},
            {"[M+CH3COOH-H]-", "[M+CH3COO]-"},
        };

        public static Dictionary<string, double> AdductToCalc = new Dictionary<string, double>()
        {
            {"[M+H]+",1.00727642},
            {"[M+2H]2+",1.00727642*2},
            {"[M+3H]3+",1.00727642*3},
            {"[M+NH4]+",18.03382555},
            {"[M+Na]+",22.9892207},
            {"[2M+H]+",1.00727642},
            {"[M+K]+",38.96315142},
            {"[M+2H]+",2.01510142},
            {"[2M+Na]+",22.9892207},
            {"[M]+",-0.00054858},
            {"[M+H+Na]+",23.9970457},
            {"[M-H2O+H]+",-17.00328858},
            {"[M+H-H2O]+",-17.00328858},
            {"[M-MeOH+H]+",-31.01893858},
            {"[M+ACN+H]+",42.03382542},
            {"[2M+NH4]+",18.03382555},
            {"[M+CH3OH+H]+",33.03349142},
            {"[M-H]-",-1.00727642},
            {"[M-H2O-H]-",-19.01784142},
            {"[M-H-H2O]-",-19.01784142},
            {"[M+FA-H]-",44.99820285},
            {"[M+Cl]-",34.96830442},
            {"[M+K-2H]-",36.94859858},
            {"[2M-H]-",-1.00727642},
            {"[M+H-H2O]-",-17.00219142},
            {"[M+Hac-H]-",59.013851},
            {"[M+HCOO]-",44.99820285},
            {"[M+CH3COO]-",59.013851},

            {"[M-2H2O+H]+",1.00727642-36.021129369},
            {"[M+H-2H2O]+",1.00727642-36.021129369},
            {"[2M+K]+",38.96315142},

            {"[M+Na-2H]-",20.974666},
            {"[M+Br]-",78.918885},
            {"[M+TFA-H]-",112.985586},
            {"[2M+Hac-H]-",59.013851},
            {"[3M-H]-",1.007276},
            {"[M-2H]2-",-1.007276*2},
            {"[M-3H]3-",-1.007276*3},
            
            ////{"[M+CH3OH+H]+",33.033489},
            ////{"[M+Li]+",7.01600455000},
            ////{"[M+H-2H2O]+",-30.012756},
            ////{"[M+2Na-H]+",44.97116},
            ////{"[M+IsoProp+H]+",61.06534},
            ////{"[M+ACN+Na]+",64.015765},
            ////{"[M+2K-H]+",76.91904},
            ////{"[M+DMSO+H]+",79.02122},
            ////{"[M+2ACN+H]+",83.06037},
            ////{"[M+IsoProp+Na+H]+",84.05511},
            ////{"[2M+3H2O+2H]+",28.02312},
            ////{"[2M+K]+",38.963158},

        };

    }

}

