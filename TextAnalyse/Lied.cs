using System;
using System.Linq;
using System.Collections.Generic;

namespace TextAnalyse
{
    public class Lied
    {

        public string Kuenstler { get; set; }
        public string Liedtitel { get; set; }
        public IEnumerable<WortAnzahl> Worte { get; set; }

        // Gesamtanzahl der Worte in diesem Lied
        public System.Nullable<int> GesamtanzahlWorte()
        {
            var result = (from wc in this.Worte select wc.Anzahl).Sum();
            return result;
        }

        // Anzahl der einzelnen Worte
        public System.Nullable<int> AnzahlEinzelnerWorte()
        {
            if (Worte != null)
                return Worte.Count();
            else
                return 0;
        }

        public void Dump()
        {
            Console.WriteLine(this.Kuenstler + " - " + this.Liedtitel);
            int counter = 1;
            foreach (var a in this.Worte)
            {
                Console.WriteLine(a.Wort + ' ' + a.Anzahl);
                if (counter++ == 10)
                    break;
            }
            Console.WriteLine();
        }


        public Lied(string liedtext)
        {
            AnalysiereText(liedtext);
        }

        private void AnalysiereText(string songtext)
        {

            // wir holen die Metadaten aus den ersten zeilen des Songtextes
            // und löschen danach die zeilen aus dem text
            var lines = songtext.Split(new char[] { '\n' });

            // 1. Zeile = Interpret / Artist
            this.Kuenstler = lines[0].Trim();
            lines[0] = "";
            // 2. Zeile = Songtitel
            this.Liedtitel = lines[1].Trim();
            lines[1] = "";

            // aus den restlichen zeilen wird wieder ein string erstellt, der im folgenden in worte aufgeteilt wird
            string text = String.Join(" ", lines);

            // das sind die zeichen, welche für uns die worte trennen
            char[] delimiters = new char[] { ' ', '\r', '\n', '.', '!', '?', ',', '(', ')', '–', '-', ':' };

            // hier wandeln wir den text in kleinschreibung um
            // und trennen danach den text in einzelne worte auf
            var words = text.Trim().ToLower().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            // damit wir mit LINQ damit arbeiten können, wandeln wirs das array in IEnumerable um
            IEnumerable<string> xxx_words = words.Cast<string>();


            // Worte werden durch substitute ersetzt um zu vereinheitlichen.
            // so wird z.B. aus "geh'" "gehe" und aus "7." "siebten" 
            // vielleicht??? 


            // hier zählen wir, welches wort wie oft vorkommt
            var counts = xxx_words
                .GroupBy(x => x)
                .Select(x => new WortAnzahl() { Wort = x.Key, Anzahl = x.Count() })
                .OrderBy(x => -x.Anzahl);

            this.Worte = counts;
        }

    }
}
