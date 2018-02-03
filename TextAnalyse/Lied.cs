using System;
using System.Linq;
using System.Collections.Generic;

namespace TextAnalyse
{
    public class Lied
    {

        public string Kuenstler 
        { 
            get; 
            private set; 
        }
        public string Liedtitel 
        { 
            get; 
            private set; 
        }

        public string Schlagworte
        {
            get;
            private set;
        }

        public List<WortAnzahl> Worte 
        { 
            get; 
            private set; 
        }


        private decimal m_PunkteInWievielenLiedergefunden = 0;
        public decimal PunkteInWievielenLiedergefunden()
        {
            return Math.Round(m_PunkteInWievielenLiedergefunden, 3);
        }

        private decimal m_PunkteWieOftInDenTextenGefunden = 0;
        public decimal PunkteWieOftInDenTextenGefunden()
        {
            return Math.Round(m_PunkteWieOftInDenTextenGefunden , 3);
        }

        public void ErmittlePunkte(List<LiederWortStatistik> liederwortstatistik, int gesamtanzahllieder)
        {
            // Bewertungsfunktion
            // 
            m_PunkteInWievielenLiedergefunden = 0;
            m_PunkteWieOftInDenTextenGefunden = 0;

            foreach (var item in Worte)
            {
                // es wird eine summe gebildet für alle worte die 
                // sowohl im liedvokabular und gesamtvokabular vorkommen
                LiederWortStatistik gefunden = liederwortstatistik.FirstOrDefault(c => c.Wort == item.Wort);
                if (gefunden != null)
                {
                    m_PunkteWieOftInDenTextenGefunden += gefunden.WieOftInDenTextenGefunden;
                    m_PunkteInWievielenLiedergefunden += (gefunden.InWievielenLiedergefunden - 1);
                }
                // die summe für die eigenen vokabeln werden abgezogen
                m_PunkteWieOftInDenTextenGefunden -= item.Anzahl;
            }
            m_PunkteWieOftInDenTextenGefunden /= gesamtanzahllieder;
            m_PunkteWieOftInDenTextenGefunden /= Worte.Count();
            m_PunkteInWievielenLiedergefunden /= gesamtanzahllieder;
            m_PunkteInWievielenLiedergefunden /= Worte.Count();
        }

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


        private static readonly string delimiter = ";";

        public string ZeigeLiedstatistik()
        {
            string result= this.Kuenstler + 
                              delimiter +
                               this.Liedtitel +
                              delimiter +
                               this.Schlagworte +
                              delimiter +
                              Worte.Count() +
                              delimiter +
                              this.PunkteInWievielenLiedergefunden()  +
                              delimiter +
                              this.PunkteWieOftInDenTextenGefunden() 
                             
                               ;

            //foreach (var item in Worte.OrderBy(x=>x.Wort))
            //{
            //    result = result + delimiter + item.Wort;
            //}

            return result;
        }

        public Lied(string liedtext, Dictionary<string, string> ersetzungen)
        {
            AnalysiereText(liedtext, ersetzungen);
        }

        private void AnalysiereText(string songtext, Dictionary<string, string> ersetzungen)
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
            // 3. Zeile = Schlagwort
            this.Schlagworte = lines[2].Trim();
            lines[2] = "";

            // aus den restlichen zeilen wird wieder ein string erstellt, der im folgenden in worte aufgeteilt wird
            string text = String.Join(" ", lines);

            // das sind die zeichen, welche für uns die worte trennen
            char[] delimiters = new char[] { ' ', '\r', '\n', '.', '!', '?', ',', '(', ')', '–', '-', ':' ,'„', '“' , '"'};

            // hier wandeln wir den text in kleinschreibung um
            // und trennen danach den text in einzelne worte auf
            var words = text.Trim().ToLower().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            // damit wir mit LINQ damit arbeiten können, wandeln wirs das array in List<string> um
            List<string> xxx_words = new List<string>(words);


            // Worte werden durch substitute ersetzt um zu vereinheitlichen.
            // so wird z.B. aus "geh'" "gehe" und aus "7." "siebten" 
            // vielleicht??? 
            foreach (var item in ersetzungen)
            {
                xxx_words = xxx_words.Select<string, string>(s => s == item.Key ? item.Value : s).ToList();
            }

            // wieder zusammensetzen, da aus einem wort mehrere werden können
            text = String.Join(" ", xxx_words);
            // wieder splitten
            words = text.Trim().ToLower().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            // und nochmal eine liste machen
            xxx_words = new List<string>(words);

            // und dann nochmals alles durchwühlen. sicher ist sicher
            foreach (var item in ersetzungen)
            {
                xxx_words = xxx_words.Select<string, string>(s => s == item.Key ? item.Value : s).ToList();
            }

            // hier zählen wir, welches wort wie oft vorkommt
            var counts = xxx_words
                .GroupBy(x => x)
                .Select(x => new WortAnzahl() { Wort = x.Key, Anzahl = x.Count() })
                .OrderBy(x => -x.Anzahl);

            this.Worte = counts.ToList();
        }

    }
}
