using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextAnalyse
{

    public class Program
    {
        public static void Main()
        {
            Lieder lieder = new Lieder();

            // hier wird die Liste ausgegeben
            foreach (var song in lieder.LiederListe().OrderBy(o => o.Liedtitel).ToList())
            {
                song.Dump();
            }


            // welche worte werden überhaupt in wievielen lieder benutzt?
            // hier zählen wir, welches wort wie oft vorkommt
            var results =
                from song in lieder.LiederListe()
                from wordcount in song.Worte
                group wordcount by wordcount.Wort
                ;

            Console.WriteLine("\nunterschiedliche Worte (Vokabular):" + results.Count());


            Console.WriteLine("\nWie oft kommt ein bestimmtes Wort in den Lieder vor (Häufigkeit):");


            foreach (var a in results.OrderBy(w => -w.Count()).ToList())
            {
                Console.WriteLine(a.Key + " " + a.Count());
            }

            Console.WriteLine("\nWie oft kommt ein bestimmtes Wort in den Lieder vor (Alphabet):");
            foreach (var a in results.OrderBy(w => w.Key).ToList())
            {
                Console.WriteLine(a.Key + " " + a.Count());
            }
        }

    }
}