using Heiflow.Core.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Heiflow.Models.Subsurface.MT3DMS
{
    public class SpeciesManager
    {
        public static string[] _default_mobile_species = new string[] { "Ca", "Cl", "Mg", "Na", "S(6)", "C(4)", "K" };
        public static float[] _default_mobile_species_con = new float[] { 0.009f, 0.03f, 0.0017f, 0.02f, 0.0067f, 0.0003f, 0.0007f };
        public static string[] _default_exchange_species = new string[] { "Ca", "Na", "Mg" };
        public static float[] _default_exchange_species_con = new float[] { 0.0001f, 0.0001f, 0.0001f };

        public SpeciesManager()
        {
            SpeciesCollection = new ObservableCollection<Species>();
        }
        public ObservableCollection<Species> SpeciesCollection
        {
            get;
            private set;
        }

        public int NumSelectedSpecies
        {
            get
            {
                var buf = from sp in SpeciesCollection where sp.Selected select sp;
                return buf.Count();
            }
        }

        public bool Contains(string speciename)
        {
            var buf = from sp in SpeciesCollection where sp.Name == speciename select sp;
            return buf.Any();
        }

        public Species Select(string speciename)
        {
            var buf = from sp in SpeciesCollection where sp.Name == speciename select sp;
            if (buf.Any())
                return buf.First();
            else
                return null;
        }

        public void CheckMandaryMobileSpecies()
        {
            if (!this.Contains("pH"))
            {
                var species = new Species()
                {
                    Name = "pH",
                    Selected = true,
                    InitialConcentration = 7.9f
                };
                SpeciesCollection.Add(species);
            }

            if (!this.Contains("pe"))
            {
                var species = new Species()
                {
                    Name = "pe",
                    Selected = true,
                    InitialConcentration = 8f
                };
                SpeciesCollection.Add(species);
            }

        }

        public void SetDefaultMobileSpecies()
        {
            int i = 0;
            foreach (var sp in _default_mobile_species)
            {

                if (!this.Contains(sp))
                {
                    var species = new Species()
                    {
                        Name = sp,
                        Selected = true,
                        InitialConcentration = _default_mobile_species_con[i]
                    };
                    SpeciesCollection.Add(species);
                }
                else
                {
                    var spec = Select(sp);
                    spec.Selected = true;
                }
                i++;
            }
        }

        public void SetDefaultExchangeSpecies()
        {
            int i = 0;
            foreach (var sp in _default_exchange_species)
            {
                if (!this.Contains(sp))
                {
                    var species = new Species()
                    {
                        Name = sp,
                        Selected = true,
                        InitialConcentration = _default_exchange_species_con[i]
                    };
                    SpeciesCollection.Add(species);
                }
                else
                {
                    var spec = Select(sp);
                    spec.Selected = true;
                }
                i++;
            }

            var Ca = Select("Ca");
            Ca.LonNum = 2;
            var Na = Select("Na");
            Na.LonNum = 1;
            var Mg = Select("Mg");
            Mg.LonNum = 1;
        }

        public void LoadCollectionFromDB(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            string line = "";
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line.Contains("SOLUTION_MASTER_SPECIES"))
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                line = sr.ReadLine();
            }
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line.Contains("SOLUTION_SPECIES"))
                    break;
                if (TypeConverterEx.IsNotNull(line))
                {
                    var buf = TypeConverterEx.Split<string>(line);
                    var species = new Species()
                    {
                        Name = buf[0],
                        Selected=false
                    };
                    SpeciesCollection.Add(species);
                }
            }

            sr.Close();
        }

    }
}
