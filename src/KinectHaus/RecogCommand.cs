using System.IO;
using System.Media;
using System.Text;
using KinectHaus.Properties;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogCommand : IRecog
    {
        static readonly IRecog _recogPlay = new RecogPlay();
        ListenContext _listenCtx;

        public void Start(ListenContext listenCtx, SpeechRecognitionEngine sre)
        {
            _listenCtx = listenCtx;
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Resources.RecogCommand)))
                sre.LoadGrammar(new Grammar(memoryStream));
        }

        public IRecog Process(RecognitionResult r)
        {
            switch (r.Semantics.Value.ToString())
            {
                case "CANCEL":
                    SystemSounds.Exclamation.Play();
                    return Listen.ResetRecog;
                case "EXIT":
                    if (r.Confidence >= 0.6)
                    {
                        SystemSounds.Exclamation.Play();
                        _listenCtx.Exit();
                        return null;
                    }
                    break;
                case "PLAY":
                    if (r.Confidence >= 0.5)
                    {
                        _listenCtx.BalloonTip(2, "Play", "Name, Series, Episode, Go", ListenIcon.Info);
                        return _recogPlay;
                    }
                    break;
                case "PAUSE":
                    Vlc.Pause(); 
                    return Listen.ResetRecog;
            }
            return null;
        }
    }
}
