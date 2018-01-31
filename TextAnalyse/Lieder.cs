﻿using System;
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
            foreach (var item in liederliste)
            {
                item.ErmittlePunkte(liederwortstatistik, liederliste.Count());
                //item.Dump();
            }
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
                //Console.WriteLine(a.Key + ' ' + a.ToList().Count + ' ' + a.ToList().Sum((WortAnzahl arg) => arg.Anzahl));
                liederwortstatistik.Add( new LiederWortStatistik() 
                    {
                        Wort=a.Key , 
                        InWievielenLiedergefunden=a.ToList().Count(), 
                        WieOftInDenTextenGefunden= a.ToList().Sum((WortAnzahl arg) => arg.Anzahl)
                    });
            }
            //Console.WriteLine();

        }

        /// <summary>
        /// Laden der Liedtexte.
        /// </summary>
        /// <returns>Die Liedtexte als Strings.</returns>
        private void LadeLiedtexteAusDateien()
        {
            string pfad = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // lade ersetzungsliste zuerst
            string filepath = pfad + "/Dropbox/Sara/Masterarbeit/ersetzungen.txt";

            Dictionary<string, string> ersetzungen = new Dictionary<string, string>();
            if (File.Exists(filepath))
            {
                Console.WriteLine(filepath  + " gefunden");
                var zeilen = File.ReadAllLines(filepath);
                for (int i = 0; i < zeilen.Length - 1; i = i + 2)
                {
                    ersetzungen.Add(zeilen[i], zeilen[i + 1]);
                }
            }
            else
            {
                Console.WriteLine(filepath + " nicht gefunden");
            }

            string folderPath = pfad + "/Dropbox/Sara/Masterarbeit/Alben mit Liedtexten";
         
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt", SearchOption.AllDirectories))
            {
                string contents = File.ReadAllText(file);
                liederliste.Add(new Lied(contents,ersetzungen));
            }
        }

        public void ZeigeVokabular(bool ausgabeindatei)
        {
            Console.WriteLine("\nunterschiedliche Worte (Vokabular):" + this.liederwortstatistik.Count());

            FileStream fs = null;
            StreamWriter sw = null;

            string pfad = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filepath = pfad + "/Dropbox/Sara/Masterarbeit/vokabular.csv";



            if (ausgabeindatei)
            {
                fs = new FileStream(filepath, FileMode.Create);
                sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine("Wort ; InWievielenLiedergefunden ; WieOftInDenTextenGefunden");
            }

            var x12 = from element in liederwortstatistik
                orderby element.Wort ascending 
                      select element;
            foreach (var x in x12)
            {
                Console.WriteLine(x.ZeigeStatistik());
                if (ausgabeindatei)
                {
                    sw.WriteLine(x.ZeigeStatistik());
                }
            }
            Console.WriteLine();

            if (ausgabeindatei)
            {
                sw.Close();
                fs.Close();
            }
        }


        public void ZeigeLiederliste(Boolean ausgabeindatei)
        {
            FileStream fs=null;
            StreamWriter sw=null;

            string pfad = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filepath = pfad + "/Dropbox/Sara/Masterarbeit/liederstatistik.csv";


            if (ausgabeindatei)
            {
                fs = new FileStream(filepath, FileMode.Create);
                sw = new StreamWriter(fs,System.Text.Encoding.UTF8);
                sw.WriteLine("Interpret; Lied; Vokabeln; P1; P2");
            }
            foreach (var item in liederliste)
            {
                Console.WriteLine(item.ZeigeLiedstatistik());
                if (ausgabeindatei)
                {
                    sw.WriteLine(item.ZeigeLiedstatistik());
                }
            }

            if (ausgabeindatei)
            {
                sw.Close();
                fs.Close();
            }
        }
    }
}
