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
            lieder.ZeigeVokabular(true);
            lieder.ZeigeLiederliste(true);

            Console.WriteLine("Anzahl Lieder:" + lieder.LiederListe().Count());
          
        }

    }
}