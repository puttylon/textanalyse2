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
        private List<LiederWortStatistik> liederwortstatistik = new List<LiederWortStatistik>();

        public List<Lied> LiederListe()
        {
            return new List<Lied>(liederliste);
        }

        private void LadeLieder()
        {
            liederliste = new List<Lied>();
            LadeLiedtexteAusDateien();
            ZaehleWorte();
        }

        private void ZaehleWorte()
        {
            // welche worte werden überhaupt in wievielen lieder benutzt?
            // hier zählen wir, welches wort wie oft vorkommt
            var results =
                from song in this.LiederListe()
                from wordcount in song.Worte
                group wordcount by wordcount.Wort
                ;

          
            liederwortstatistik.Clear();
            foreach (var a in results)
            {
                Console.WriteLine(a.Key + ' ' + a.ToList().Count + ' ' + a.ToList().Sum((WortAnzahl arg) => arg.Anzahl));
                liederwortstatistik.Add( new LiederWortStatistik() 
                    {
                        Wort=a.Key , 
                        InWievielenLiedergefunden=a.ToList().Count(), 
                        WieOftInDenTextenGefunden= a.ToList().Sum((WortAnzahl arg) => arg.Anzahl)
                    });
            }


        }

        /// <summary>
        /// Laden der Liedtexte.
        /// </summary>
        /// <returns>Die Liedtexte als Strings.</returns>
        private void LadeLiedtexteAusDateien()
        {
            string pfad = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // lade ersetzungsliste zuerst
            string filepath = pfad + "/ersetzungen.txt";
            var zeilen=File.ReadAllLines(filepath);
            Dictionary<string, string> ersetzungen = new Dictionary<string, string>();
            for (int i = 0; i < zeilen.Length-1; i=i+2)
            {
                ersetzungen.Add(zeilen[i],zeilen[i+1]);
            }

            string folderPath = pfad + "/liedtexte";
         
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt", SearchOption.AllDirectories))
            {
                string contents = File.ReadAllText(file);
                liederliste.Add(new Lied(contents,ersetzungen));
            }
        }

        public void Dump()
        {
            Console.WriteLine("\nunterschiedliche Worte (Vokabular):" + this.liederwortstatistik.Count());

            var x12 = from element in liederwortstatistik
                orderby element.WieOftInDenTextenGefunden descending 
                      select element;
            foreach (var x in x12)
            {
                x.Dump();
            }
        }
    }
}
