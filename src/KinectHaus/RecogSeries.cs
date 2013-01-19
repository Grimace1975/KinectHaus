using System;
using System.Linq;
using System.Globalization;
using System.IO;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogSeries : IRecog
    {
        static readonly string _path = @"\\192.168.1.2\Media2\Series\";
        static readonly Choices _choices;

        static RecogSeries()
        {
            _choices = new Choices();
            foreach (var d in Directory.GetDirectories(_path).Select(Path.GetFileName))
                _choices.Add(new SemanticResultValue(Listen.CleanPath(d), d));
        }

        public string Title
        {
            get { return "Movies"; }
        }

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
