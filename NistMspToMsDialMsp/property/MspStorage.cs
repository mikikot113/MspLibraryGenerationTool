using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NistSdfToMspConvert
{
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

        public string Ontology {
            get {
                return ontology;
            }

            set {
                ontology = value;
            }
        }

        public string OntologyID {
            get {
                return ontologyID;
            }

            set {
                ontologyID = value;
            }
        }

        public string BasepeakIntensity {
            get {
                return basepeakIntensity;
            }

            set {
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

    }
}
