using System;
using System.Linq;
using System.Collections.Generic;

namespace TextAnalyse
{
    public class Lied
    {

        // das sind die zeichen, welche für uns die worte trennen
        private readonly char[] m_delimiters = new char[] { ' ', '\r', '\n', '.', '!', '?', ',', '(', ')', '–', '-', ':', '„', '“', '"' };

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

        public string Kommentar
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

        public decimal P3
        {
            get;
            set;
        }

        public void ErmittlePunkte(List<LiederWortStatistik> liederwortstatistik, int gesamtanzahllieder)
        {
            // Bewertungsfunktion m_PunkteInWievielenLiedergefunden
            // diese Zahl sagt aus, zu wie viel Prozent die eigenen Vokabeln 
            // denen der restlichen Lieder übereinstimmen
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
                    // die summe für die eigenen vokabeln werden abgezogen
                    m_PunkteWieOftInDenTextenGefunden -= item.Anzahl;

                    m_PunkteInWievielenLiedergefunden += (gefunden.InWievielenLiedergefunden - 1);
                }
            }
            //// wie verhält sich der wert im verhältnis zur gesamtpunktzahl?
            //var summeAllerVokaleAnzahl = liederwortstatistik.Sum(item=>item.InWievielenLiedergefunden);
            //var summeDieseVokaleAnzahl = this.Worte.Count();

            m_PunkteWieOftInDenTextenGefunden /= (gesamtanzahllieder-1);
            m_PunkteWieOftInDenTextenGefunden /= Worte.Count();

            m_PunkteInWievielenLiedergefunden /= (gesamtanzahllieder-1);
            m_PunkteInWievielenLiedergefunden /= Worte.Count();

            P3 = m_PunkteInWievielenLiedergefunden * ((decimal) this.Worte.Count() / liederwortstatistik.Count());

            // Bewertungsfunktion 
            // Man nimmt die Vokabelliste und erhält eine Summe der Werte A10, B4, C9, etc. z.B. 3000
            // davon zieht man die Länge der eigenen Vokabelliste
            /// blablabla... mal noch die testdaten ansehen
        }

        // Gesamtanzahl der Worte in diesem Lied
        public int? GesamtanzahlWorte()
        {
            var result = (from wc in this.Worte select wc.Anzahl).Sum();
            return result;
        }

        // Anzahl der einzelnen Worte
        public int? AnzahlEinzelnerWorte()
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
                               this.Kommentar +
                              delimiter +
                              Worte.Count() +
                              delimiter +
                              this.PunkteInWievielenLiedergefunden()  +
                              delimiter +
                              this.PunkteWieOftInDenTextenGefunden() 
                              +
                               delimiter +
                              this.P3 

                               ;

            foreach (var item in Worte.OrderBy(x=>x.Wort))
            {
                result = result + delimiter + item.Wort;
            }

            return result;
        }

        public Lied(string liedtext, Dictionary<string, string> ausschreibungen, Dictionary<string, string> ersetzungen)
        {
            AnalysiereText(liedtext, ausschreibungen, ersetzungen);
        }

        private string ExtrahiereMetadaten(string songtext)
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
            // 4. Zeile = Schlagwort
            this.Kommentar = lines[3].Trim();
            lines[3] = "";


            // aus den restlichen zeilen wird wieder ein string erstellt, der im folgenden in worte aufgeteilt wird
            return String.Join(" ", lines).Trim();
    
        }

        private List<string> SplitteTextInWorte(string songtext)
        {
            // und trennen danach den text in einzelne worte auf
            return new List<string>(songtext.Split(m_delimiters, StringSplitOptions.RemoveEmptyEntries));
        }

        private string ErsetzeWorte(string songtext, Dictionary<string, string> ersetzungen)
        {

            // Worte werden aufgesplitet
            List<string> wortliste = SplitteTextInWorte(songtext);

            // Worte werden durch substitute ersetzt um zu vereinheitlichen.
            // so wird z.B. aus "geh'" "gehe" und aus "7." "siebten" 
            // vielleicht??? 
            foreach (var item in ersetzungen)
            {
                wortliste = wortliste.Select<string, string>(s => s == item.Key ? item.Value : s).ToList();
            }

            // wieder zusammensetzen, da aus einem wort mehrere werden können
            songtext = String.Join(" ", wortliste).Trim();

            // wieder splitten
            wortliste = SplitteTextInWorte(songtext);

            // und dann nochmals alles durchwühlen, da nun aus einem Wort mehrere geworden sein können
            foreach (var item in ersetzungen)
            {
                wortliste = wortliste.Select<string, string>(s => s == item.Key ? item.Value : s).ToList();
            }

            // wir geben die aktuellen Worte als String zurück
            return String.Join(" ", wortliste).Trim();
            
        }

        private void AnalysiereText(string songtext, Dictionary<string, string> ausschreibungen, Dictionary<string, string> ersetzungen)
        {

            // metadaten auslesen
            songtext = ExtrahiereMetadaten(songtext);

            // wir wandeln wir den text in kleinschreibung um
            songtext = songtext.ToLower();

            // Wortersetzungen
            songtext = ErsetzeWorte(songtext, ausschreibungen);

            List<string> wortliste = SplitteTextInWorte(songtext);


            // hier zählen wir, welches wort wie oft vorkommt
            var counts = wortliste
                .GroupBy(x => x)
                .Select(x => new WortAnzahl() { Wort = x.Key, Anzahl = x.Count() })
                .OrderBy(x => -x.Anzahl);

            this.Worte = counts.ToList();
        }

    }
}
