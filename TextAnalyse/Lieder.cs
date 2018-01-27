using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TextAnalyse
{
    public class Lieder
    {
        public Lieder()
        {
            LadeLieder();
        }

        private List<Lied> liederliste = new List<Lied>();

        public List<Lied> LiederListe()
        {
            return new List<Lied>(liederliste);
        }

        private void LadeLieder()
        {
            liederliste = new List<Lied>();
            LadeLiedtexteAusDateien();
        }

        /// <summary>
        /// Laden der Liedtexte.
        /// </summary>
        /// <returns>Die Liedtexte als Strings.</returns>
        private void LadeLiedtexteAusDateien()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/liedtexte";
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt", SearchOption.AllDirectories))
            {
                string contents = File.ReadAllText(file);
                liederliste.Add(new Lied(contents));
            }
        }
    }
}
