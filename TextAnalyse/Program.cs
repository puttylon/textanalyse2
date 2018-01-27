using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextAnalyse
{

    //public class WortAnzahl
    //{
    //    public int Anzahl { get; set; }
    //    public string Wort { get; set; }
    //    //..............
    //}


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


        public Lied(){}

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

    public class Lieder
    {
        // 
        private List<Lied> lieder = new List<Lied>();

        public void LadeLieder()
        {
            lieder = new List<Lied>();
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
                lieder.Add(new Lied(contents));
            }
        }
    }


    public class Program
    {
        public static void Main()
        {

            List<Lied> songs = new List<Lied>();

            List<String> songtexte = LadeSogtexte();

            // hier werden alle songtexte analysiert und in einer Liste gespeichert
            foreach (string songtext in songtexte)
            {
                songs.Add(AnalysiereText(songtext));
            }

            // hier wird die Liste ausgegeben
            foreach (var song in songs.OrderBy(o => o.Liedtitel).ToList())
            {
                song.Dump();
            }


            // welche worte werden überhaupt in wievielen lieder benutzt?
            // hier zählen wir, welches wort wie oft vorkommt
            var results =
                from song in songs
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

        private static Lied AnalysiereText(string songtext)
        {
            // wir geben als ergebnis eine Information über einen Song zurück
            Lied result = new Lied();

            // wir holen die Metadaten aus den ersten zeilen des Songtextes
            // und löschen danach die zeilen aus dem text
            var lines = songtext.Split(new char[] { '\n' });

            // 1. Zeile = Interpret / Artist
            result.Kuenstler = lines[0].Trim();
            lines[0] = "";
            // 2. Zeile = Songtitel
            result.Liedtitel = lines[1].Trim();
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

            result.Worte = counts;

            return result;
        }

        public static List<String> LadeSogtexte()
        {

            List<String> result = new List<string>();

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/liedtexte";
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt", SearchOption.AllDirectories))
            {
                string contents = File.ReadAllText(file);
                result.Add(contents);
            }

            return result;
            //return new List<string>(songtexte_fix);
        }


        static string[] songtexte_fix =

        {

@"Stark, Christin
Wo ist die Liebe hin

Heute Morgen kurz nach acht
Bin ich alleine aufgewacht
Ich drehte mich nach allen Seiten um


Doch sie war weg, weiss nicht warum
Liebe kann doch nicht verloren geh'n
Einfach so, ich kann es nicht versteh'n
Hab' damit niemals ein Problem gehabt
Sie war immer da, es hat doch gut geklappt

Wo ist die Liebe?
Wo ist die Liebe hin?
Wo ist die liebe hin?
Wo ist die Liebe?
Wo ist die Liebe hin?
Wo ist die liebe hin?
Wo ist sie hin?

Ich hab' sofort das ganze Haus durchsucht
Hab' laut geweint und leis' geflucht
Unter'm Bett und hinter jedem Schrank 
Hab' sie gerufen stundenlang
Keine Antwort und kein Zeichen
Sie kann doch nicht so einfach geh'n
Kann sie nicht spüren, nicht berühren
Und ich kann sie auch nicht seh'n

Wo ist die Liebe?
Wo ist die Liebe hin?
Wo ist die liebe hin?
Wo ist die Liebe?
Wo ist die Liebe hin?
Wo ist die liebe hin?
Wo ist sie hin?

Sie ist bestimmt nicht weg
Sie hat sich nur versteckt
Wo ist die Liebe hin?
Wo ist die Liebe hin?

(Wo ist die Liebe?)
(Wo ist die Liebe hin?)
(Wo ist die liebe hin?)

Wo ist die Liebe?
Wo ist die Liebe hin?
Wo ist die liebe hin?
Wo ist die Liebe?
Wo ist die Liebe hin?
Wo ist die liebe hin?
Wo ist sie hin?


"

,


@"Berg, Andrea
Ja ich will
Von Engeln getragen


Ich weiß es noch wie's damals war 
Das erste Mal mit siebzehn Jahren
In uns'rem kleinen Zelt am Baggersee
Die Bilder waren noch schwarz weiß

Ein Bett im Kornfeld, Himbeereis
Im Käfer nach Paris auf 'nem Kaffee
Der erste Kuss beim Flaschendrehen
Mit dir im Mondschein baden gehen das war so schön (war so schön)
Komm lass es wieder geschehen

Ja, ich will (ich will) 
Mit dir am Strand auf weißen Pferden reiten
Ja, ich will (ich will) 
Wild und verrückt sein wie in alten Zeiten
Ich hab' noch immer Gänsehaut 'nen Schmetterling in meinem Bauch
Weil es das allergrößte ist, wie du mich liebst

Ich liege wach in deinem Arm
Und denk' daran, wie es begann
Erinnerung malt Sonnen in die Nacht
Wir waren jung und unbeschwert
Die ganze Welt hat uns gehört 
Und Wolken wurden einfach weg gelacht
Ich hab' nicht einen Tag bereut
Ich geh' mit dir durch Freud' und Leid, egal wie weit
Bis ans Ende der Zeit

Ja, ich will (ich will) 
Mit dir am Strand auf weißen Pferden reiten
Ja, ich will (ich will) 
Wild und verrückt sein wie in alten Zeiten
Ich hab' noch immer Gänsehaut 'nen Schmetterling in meinem Bauch
Weil es das allergrößte ist, wie du mich liebst

Ja, ich will (ich will) 
Mit dir im Autokino Liebe machen
Ja, ich will (ich will) 
An jedem neuen Tag mit dir erwachen
Ich hab noch immer Gänsehaut 'nen Schmetterling in meinem Bauch
Weil es das allergrößte ist, wie du mich liebst
Ja, ich will

"
    ,


@"David, Julian
Wir sind nie allein     

Wenn wir zusammen stehen .
Die selbe Sonne sehen.
Wir sind nie allein .


Weil unsere Sprache Liebe ist und
davon gibt es nie genug .
Egal was auch passiert.
Wohin das Leben führt .
Wir sind nie allein .
Woher wir auch gekommen sind .
Wir passen überall dazu .
Wir sind nie allein .
Ooh oh oh oho .
Das wird für immer so sein .
Wenn wir uns ohne Wort verstehen .
Die gleichen Wege gehen .
Wir sind nie allein .
Denn wenn wir es gemeinsam tun, dann ist es pure Energie.
Wenn man keine Grenze kennt, die uns voneinander trennt .
Wir sind nie allein.
Wenn die Herzen lauter schlagen, hören wir die Zweifel nie .
Wir sind nie allein.
Das wird für immer so sein.
Wir sind nie allein .
Das wird für immer so sein .
Wir sind nie allein .
Dreht sich der Tag zu schnell, so wie im Karussell .
Wir sind nie allein .
Wenn unsere Stunden kürzer sind, dann sollten sie noch bunter sein .
Weil die Hürden tiefer sind, wenn man sie zusammen nimmt .
Wir sind nie allein .
Solange wir noch Ziele haben, werden wir noch stärker sein .
Wir sind nie allein .
Oh ooooooh .
Das wird für immer so sein .
Ooh oooh ohoho.
Wir sind nie allein .
Das wird für immer so sein .
Wir sind nie allein .
Das wird für immer so sein.
WIR SIND NIE ALLEIN!!!


"

,


@"Gilbert
Lass das Leben tanzen

Manchmal bleibst du stehen
Um dich nur im Kreis zu drehen
Und du fragst dich, wie komm‘ ich da wohl raus?


Dann zeigt dir das Leben
Es kann viele Wege geben
Ganz schwerelos und frei trägt's dich hinaus
In eine Welt, die dir gefällt

Lass das Leben tanzen
Über den Asphalt
Wir frieren den Moment ein
Und mein Herz erstrahlt
Tage so wie heute
Sollten endlos sein
Ja, endlos sein

Lass das Leben tanzen
Über den Asphalt
In der Menschenmenge
Wird dir warm und kalt
Tage so wie heute
Sollten endlos sein
Nur endlos sein

Du lässt dich nicht pressieren
Du willst deine Träume spüren
Du küsst die Welt
Auf ihren Mund und bebst
Im Rhythmus deiner Lieder
Findest du dich immer wieder
Ganz federleicht und frei
Mein Freund du lebst
In einer Welt, die dir gefällt

Lass das Leben tanzen
Über den Asphalt
Wir frieren den Moment ein
Und mein Herz erstrahlt
Tage so wie heute
Sollten endlos sein
Ja, endlos sein

Lass das Leben tanzen
Über den Asphalt
In der Menschenmenge
Wird dir warm und kalt
Tage so wie heute
Sollten endlos sein
Nur endlos sein

Lass das Leben tanzen
Über den Asphalt
Wir frieren den Moment ein
Und mein Herz erstrahlt
Tage so wie heute
Sollten endlos sein
Ja, endlos sein

Lass das Leben tanzen
Über den Asphalt
In der Menschenmenge
Wird dir warm und kalt
Tage so wie heute
Sollten endlos sein
Nur endlos sein

"

,

@"Rossi, Semino
Muy Bien

Sternennacht, der Mond erwacht
Und ich wart' auf dich mein Schatz, bei Kerzenlicht
Musik erklingt, in den Himmel, so nah
Mein Herz geht auf, denn plötzlich stehst du da

Du hast dich heute wieder schön gemacht
Muy bien
Ich hab' die ganze Nacht an dich gedacht
Muy bien
Ein Leben wie der Sonnenschein
Weil uns so rot die Sonne scheint
Du hast dich heute wieder schön gemacht
Muy bien

Mondenschein, wir zwei allein
Um uns weit das Strand, das Meer und heisser Sand
Der Abend dient, singt für uns unser Lied
Und ich spür', ich bin noch niemals so verliebt

Du hast dich heute wieder schön gemacht
Muy bien
Ich hab' die ganze Nacht an dich gedacht
Muy bien
Ein Leben wie der Sonnenschein
Weil uns so rot die Sonne scheint
Du hast dich heute wieder schön gemacht
Muy bien

Du hast dich heute wieder schön gemacht
Muy bien
Ich hab' die ganze Nacht an dich gedacht
Muy bien
Ein Leben wie der Sonnenschein
Weil uns so rot die Sonne scheint
Du hast dich heute wieder schön gemacht
Muy bien"
,


@"Kaiser, Roland
Wir geh'n durch die Zeit

Die Jahre flogen wie ein Wimpernschlag vorüber 
Und volle staunen steht drin und gegenüber 
O-oh 
Für immer und du 

Durch alle Stürme und so viele höcher und tiefen
Sind wir unserer Liebe immer treu geblieben
O-oh
Für immer ich und du

Wir geh'n durch die Zeit
Und kennen das Morgen nicht
Ich weiß nur daß es wie Gestern ist
Unser erster Augenblick

Ich liebe wie du lass jener deiner Gestern
Ich liebe's jeden Wunsch von deinen Augen zu lesen
O-oh
Für immer und du

Jeder einzeller moment mit dir ist Wunder und Segen
Ich hoffe um zu noch so viele Jahre begeben
O-oh
Für immer ich und du

Wir geh'n durch die Zeit
Und kennen das Morgen nicht
Ich weiß nur daß es wie Gestern ist
Unser erster Augenblick

Wohin geht die Zeit
Wenn sie Vergangen ist
Ich weiss nur in diesem Augenblick
Du bist mein großtest Glück

Wir geh'n durch die Zeit
Und kennen das Morgen nicht
Ich weiß nur daß es wie Gestern ist
Unser erster Augenblick

Das Jahr verging in flucken diesen schnellen Zeiten
Lass unsere momenten Augenblick vera weil o-oh
Für immer ich und du

Wir geh'n durch die Zeit
Und kennen das Morgen nicht
Ich weiß nur daß es wie Gestern ist
Unser erster Augenblick

Wohin geht die Zeit
Und kennen das Morgen nicht

Die Jahre flogen wie ein Wimpernschlag vorüber
Und volle staunen steht drin und gegenüber
O-oh
Für immer ich und du"

,


@"Carpendale, Howard
Unter einem Himmel

Während hier der Tag beginnt
Ist woanders tiefste Nacht
Jemand träumt von dem
Was ein anderer gerade hat
Das alles passiert zur gleichen Zeit
Wenn sich zwei gerade streiten
Küsst sich irgendwo ein Paar
Und immer wenn was aufhört
Fängt was neues an
Irgendwo
Irgendwann
Das alles passiert
Unter einem Himmel
In der gleichen Stadt
unter einem Dach
In jedem von uns
Und das alles passiert
Unter einem Himmel
Seit Ewigkeiten bis in alle Zeiten
In jedem von uns


Während eine gerade ankommt
Geht ein anderer von Bord
Gemeinsam in der Wüste
Einsam in New York
Das alles passiert zur gleichen Zeit
Wo die einen sich versöhnen
Fehlt den anderen die Kraft
Wem bist du begegnet
Wen habe ich verpasst
Irgendwo
Irgendwann
Das alles passiert
Unter einem Himmel
In der gleichen Stadt
Unter einem Dach
In jedem von uns
Und das alles passiert
Unter einem Himmel
Seit Ewigkeiten bis in alle Zeiten
In jedem von uns
Das alles passiert
Unter einem Himmel
In der gleichen Stadt
Unter einem Dach
In jedem von uns
Und das alles passiert
Das alles passiert
Das alles passiert
Unter einem Himmel
Unter einem Himmel
In der gleichen Stadt
Unter einem Dach
In jedem von uns
Und das alles passiert
Unter einem Himmel
Seit Ewigkeiten bis in alle Zeiten
In jedem von uns"

,


@"Egli, Beatrice
Herz an

Mach einfach mal deine Augen zu
Und stell dir vor, dort der Adler bist du
Er kann fliegen
Ohne Angst
Hör auf dein Herz, es lässt dich nie im Stich
Schon seit du atmest, schlägt es für dich
Es weiß mehr als
Dein Verstand

Weißt du, warum es uns zwei gibt?
Es klopft dadrin für dich und mich
Nur dein Herz weiß alles über dich und mich

Kopf aus
Und schalt doch mal dein Herz an
Und pfeif doch auf die Regeln
Es ist wie's Lichterleben, oh-ouh
Kopf aus
Und schalt doch mal dein Herz an
Lass Träume wieder fliegen
Du kannst dich nicht belügen, oh-ouh
Herz an
 
Wie ein Computer soll'n wir funktionier'n
Doch wer kann uns schon das Herz reparier'n
Wenn unsre Flügel
Gebrochen sind?
Wir brechen aus und wir fangen neu an
Wir folgen nicht mehr dem Logikplan
Für uns gibt es
Nur noch eins

Weißt du, warum es uns zwei gibt?
Es klopft dadrin für dich und mich
Nur dein Herz weiß alles über dich und mich

Kopf aus
Und schalt doch mal dein Herz an
Und pfeif doch auf die Regeln
Es ist wie's Lichterleben, oh-ouh
Kopf aus
Und schalt doch mal dein Herz an
Lass Träume wieder fliegen
Du kannst dich nicht belügen, oh-ouh
Herz an
Kopf aus
Und schalt doch mal dein Herz an
Und pfeif doch auf die Regeln
Es ist wie's Lichterleben, oh-ouh
Kopf aus
Und schalt doch mal dein Herz an
Lass Träume wieder fliegen
Du kannst dich nicht belügen, oh-ouh
Herz an"

,

@"Mai, Vanessa
Regenbogen

Ich kann den Sternenhimmel gar nicht sehen
Weil die Wolken heute endlos sind
Und ich wart' auf dich, für jetzt und immer
Können wir uns heute Nacht noch sehen?
Denn ich muss mit dir so viel bereden
Und ich weiß ich lieb' dich, jetzt und immer, immer, immer, immer
Jetzt und immer

Und ein Regenbogen zeigt den Weg
Zwischen Traum und Nacht, wie es weitergeht
Und ich werde immer bei dir sein, für immer
Und ein Regenbogen hält uns fest
Wenn das Glück uns mal alleine lässt
Und ich werde immer bei dir sein, für immer

Ist der Mond heut' Nacht auch nicht zu sehen
Aber es ist trotzdem wunderschön
Wenn du bei mir bist, für jetzt und immer
Du gibst mir Wärme und du schenkst mir Zeit
Bist meine Sonne in der Dunkelheit
Und ich weiß ich lieb' dich, jetzt und immer, immer, immer, immer
Jetzt und immer

Und ein Regenbogen zeigt den Weg
Zwischen Traum und Nacht, wie es weitergeht
Und ich werde immer bei dir sein, für immer
Und ein Regenbogen hält uns fest
Wenn das Glück uns mal alleine lässt
Und ich werde immer bei dir sein, für immer

Lass dich fallen, lass uns leben, über allen Dingen schweben
Lass und lieben, lass uns treiben, an manchen Dingen reiben
Ich will einfach immer nur bei dir sein

Und ein Regenbogen zeigt den Weg
Zwischen Traum und Nacht, wie es weitergeht
Und ich werde immer bei dir sein, für immer
Und ein Regenbogen hält uns fest
Wenn das Glück uns mal alleine lässt
Und ich werde immer bei dir sein, für immer

Ich will immer nur bei dir sein"

,


@"Anders, Thomas
Das Lied das Leben heisst

Ich schau' zurück
Und was ich sehe, das ist, 'ne Menge Glück
Und zwischendrin mal Regen
Das gehört wohl auch dazu
Denn das alles, das bist du

Ich folge dir
Wie eine Melodie, die mich berührt
Durch Höhen und durch Tiefen führt
Gehört wohl auch dazu
Denn das alles, das bist du

Du bewegst mich, du trägst mich
Und du schickst mich hoch hinaus
Oh du liebst mich, du zerstörst mich
Und du baust mich wieder auf
Du weißt, du bist, du bleibst
Das Lied das Leben heißt

Ich schau' nach vorn
Frag' mich was bleibt von mir, es geht nichts verlor'n
Ich lass' meine Liebe hier
Die Lieder, die ich schreib'
Das ist das was bleibt
 
Du bewegst mich, du trägst mich
Und du schickst mich hoch hinaus
Oh du liebst mich, du zerstörst mich
Und du baust mich wieder auf
Du weißt, du bist, du bleibst
Das Lied das Leben heißt

Ein immer off'nes Ende
Die Geschichte die ich schreib'
Ein Blatt das sich stets wendet
Eine Reise durch die Zeit
Alles das bist du

Du bewegst mich, du trägst mich
Und du schickst mich hoch hinaus
Oh du liebst mich, du zerstörst mich
Und du baust mich wieder auf
Ja du bewegst mich, du trägst mich
Und du schickst mich hoch hinaus
Oh du liebst mich und du zerstörst mich
Und du baust mich wieder auf
Du weißt, du bist, du bleibst
Das Lied das Leben heißt
Das Leben heißt"


,

@"Angelo, Nino de
Mach das nochmal

Ich will es nochmal spürn
Ich will's nochmal riskiern
In den 7. Himmel, mit Dir
Mir ist egal was auch passiert,
selbst wenn mein Herz dabei erfriert

Mach das nochmal mit mir
Lass mich auf Wolken schweben,
selbst wenn ich runterknall,
mit Überschall, so ist das Leben

Ich will's nochmal probiern
Was habe ich zu verliern?
Noch einmal Himmel und Hölle, mit Dir
Es bläst mich immer wieder weg
Dir zu entkommen keinen Zweck

Mach das nochmal mit mir
Lass mich auf Wolken schweben,
selbst wenn ich runterknall,
mit Überschall, so ist das Leben

Mach das nochmal mit mir
Heb'mich ins Universum,
und danach wisch mich weg,
wie einen Fleck,
aus Deinem Leben

Oohh, yeah...
Ich will noch einmal diese Schmerzen spürn
Ich will verdammt nochmal den Kopf verliern
Hörst Du? Hörst Du?
Was soll ich tun? Ich komm nicht von Dir los,
denn was ich fühle ist viel zu groß
Hörst Du? Hörst Du?

Ich will Dich nochmal spürn
Mein Herz nochmal verliern
In den 7. Himmel, mit Dir

Mach das nochmal mit mir
Lass mich auf Wolken schweben,
selbst wenn ich runterknall,
mit Überschall, so ist das Leben

Mach das nochmal mit mir
Heb'mich ins Universum,
und danach wisch mich weg,
wie einen Fleck,
aus Deinem Leben

Mach das nochmal mit Dir.
"

,


@"Kelly, Maite 
Jetzt oder nie

Zu lang von dir und mir geträumt
Den Augenblick versäumt
Dir zu sagen wie ich fühle
Zu stark die Zweifel und die Angst
Ich weiß nicht ob ich's kann
Doch ich will dich berühren
Heute Nacht schaust du mich ganz anders an
Heute Nacht fühlt sich's endlich richtig an
Jetzt oder nie!
Ich spüre diese Energie
Setz' alles auf's Spiel
Und wenn ich verlier'
Verlier' ich mich in dir
Jetzt oder nie!
Ich fühle diese Euphorie
Was soll schon passieren?
Hab' nichts zu verlieren
Du gehörst doch schon zu mir
Ein Blick und alles in mir bebt
So viel noch unbewegt
Doch jetzt will ich es spüren
Heute Nacht fängt ein neues Leben an
Heute Nacht fühlt sich's endlich richtig an
Jetzt oder nie!
Ich spüre diese Energie
Setz' alles auf's Spiel
Und wenn ich verlier'
Verlier' ich mich in dir
Jetzt oder nie!
Ich fühle diese Euphorie
Was soll schon passieren?
Hab' nichts zu verlieren
Du gehörst doch schon zu mir
Heute Nacht schaust du mich ganz anders an
Heute Nacht fühlt sich's endlich richtig an
Jetzt oder nie!
Ich spüre diese Energie
Setz' alles auf's Spiel
Und wenn ich verlier'
Verlier' ich mich in dir
Jetzt oder nie!
Ich fühle diese Euphorie
Was soll schon passieren?
Hab' nichts zu verlieren
Du gehörst doch schon zu mir
Jetzt oder nie!
"

    };
    }

}