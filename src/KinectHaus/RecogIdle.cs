using System.IO;
using System.Media;
using System.Text;
using KinectHaus.Properties;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogIdle : IRecog
    {
        static readonly string _localPath = "KinectHaus.v.xml";
        static readonly IRecog _recogCommand = new RecogCommand();
        ListenContext _listenCtx;

        public static string Name { get; private set; }

        public void Start(ListenContext listenCtx, SpeechRecognitionEngine sre)
        {
            _listenCtx = listenCtx;
            using (var s = (!File.Exists(_localPath) ? (Stream)new MemoryStream(Encoding.ASCII.GetBytes(Resources.RecogIdle)) : File.OpenRead(_localPath)))
            {
                var g = new Grammar(s);
                sre.LoadGrammar(g);
                Name = "XBOX";
            }
        }

        public IRecog Process(RecognitionResult r)
        {
            if (!string.IsNullOrEmpty(r.Semantics.Value.ToString()))
            {
                SystemSounds.Exclamation.Play();
                _listenCtx.BalloonTip(2, "KinectHaus\u2122", "Say: Play, Pause, Cancel", ListenIcon.Info);
                return _recogCommand;
            }
            return null;
        }
    }
}
