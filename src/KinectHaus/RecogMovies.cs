using System;
using System.Linq;
using System.Globalization;
using System.IO;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogMovies : IRecog
    {
        static readonly string _path = @"\\192.168.1.2\Media\Movie\";
        static readonly Choices _choices;

        static RecogMovies()
        {
            _choices = new Choices();
            foreach (var d in Directory.GetDirectories(_path).Select(Path.GetFileName))
                _choices.Add(new SemanticResultValue(Listen.CleanPath(d), d));
        }

        public string Title
        {
            get { return "Movies"; }
        }

        private static string Clean(string x) { return x.Split('(')[0].Trim(); }

        public void Start(SpeechRecognitionEngine sre)
        {
            var gb = new GrammarBuilder { Culture = new CultureInfo("en-US") };
            gb.Append(_choices);
            var g = new Grammar(gb);
            sre.LoadGrammar(g);
        }

        public void Stop()
        {
        }

        public IRecog Process(RecognitionResult r)
        {
            return null;
        }
    }
}
