using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using KinectHaus.Properties;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogMovies : IRecog
    {
        static readonly string _path = @"\\192.168.1.2\Media\Movie\";
        static readonly Choices _choices;
        RecognitionResult _lastKnownGood;

        static RecogMovies()
        {
            _choices = new Choices();
            foreach (var d in Directory.GetDirectories(_path))
                _choices.Add(new SemanticResultValue(Listen.CleanPath(d), Path.GetFileName(d) + "|" + d));
        }

        public string Title
        {
            get { return "Movies"; }
        }

        public void Start(SpeechRecognitionEngine sre)
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Resources.RecogMovies)))
                sre.LoadGrammar(new Grammar(memoryStream));
            var gb = new GrammarBuilder { Culture = new CultureInfo("en-US") };
            gb.Append(_choices);
            sre.LoadGrammar(new Grammar(gb));
            _lastKnownGood = null;
        }

        public void Stop() { }

        public IRecog Process(RecognitionResult r, out bool show)
        {
            show = true;
            switch (r.Semantics.Value.ToString())
            {
                case "CANCEL":
                    show = false;
                    return Listen.CancelRecog;
                case "PLAY":
                    var args = _lastKnownGood.Semantics.Value.ToString().Split('|');
                    Vlc.Play(args[1]);
                    return Listen.ResetRecog;
                default:
                    _lastKnownGood = r;
                    return null;
            }
        }
    }
}
