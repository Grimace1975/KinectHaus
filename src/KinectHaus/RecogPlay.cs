using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using KinectHaus.Properties;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogPlay : IRecog
    {
        static readonly Dictionary<string, string> _paths = new Dictionary<string, string> { 
            {"Series", @"\\192.168.1.2\Media2\Series\"},
            {"Movies", @"\\192.168.1.2\Media\Movie\"} };
        static readonly Choices _choices;
        ListenContext _listenCtx;
        RecognitionResult _lastKnownGood;

        static RecogPlay()
        {
            _choices = new Choices();
            foreach (var p in _paths)
                foreach (var d in Directory.GetDirectories(p.Value))
                    _choices.Add(new SemanticResultValue(Listen.CleanPath(d), Path.GetFileName(d) + "|" + p.Key + "|" + d));
        }

        public void Start(ListenContext listenCtx, SpeechRecognitionEngine sre)
        {
            _listenCtx = listenCtx;
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Resources.RecogPlay)))
                sre.LoadGrammar(new Grammar(memoryStream));
            var gb = new GrammarBuilder { Culture = new CultureInfo("en-US") };
            gb.Append(_choices);
            sre.LoadGrammar(new Grammar(gb));
            _lastKnownGood = null;
        }

        public IRecog Process(RecognitionResult r)
        {
            var semanticsValue = r.Semantics.Value.ToString();
            if (string.IsNullOrEmpty(semanticsValue))
                return null;
            string[] args;
            switch (semanticsValue)
            {
                case "CANCEL":
                    return Listen.ResetRecog;
                case "GO":
                    args = _lastKnownGood.Semantics.Value.ToString().Split('|');
                    Vlc.Play(args[2]);
                    _listenCtx.BalloonTip(args[1], r);
                    return Listen.ResetRecog;
                default:
                    args = r.Semantics.Value.ToString().Split('|');
                    if (args.Length == 3)
                    {
                        _lastKnownGood = r;
                        _listenCtx.BalloonTip(args[1], r);
                    }
                    else
                        _listenCtx.BalloonTip("Play", r);
                    return null;
            }
        }
    }
}
