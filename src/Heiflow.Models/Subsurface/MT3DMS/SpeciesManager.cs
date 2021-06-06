using Heiflow.Core.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Heiflow.Models.Subsurface.MT3DMS
{
    public class SpeciesManager
    {
        public SpeciesManager()
        {
            SpeciesCollection = new ObservableCollection<Species>();
        }
        public ObservableCollection<Species> SpeciesCollection
        {
            get;
            private set;
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
                        Name = buf[1],
                        Selected=false
                    };
                    SpeciesCollection.Add(species);
                }
            }

            sr.Close();
        }

    }
}
