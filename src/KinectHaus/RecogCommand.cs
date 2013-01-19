using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;
using KinectHaus.Properties;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogCommand : IRecog
    {
        static readonly IRecog _recogSeries = new RecogSeries();
        static readonly IRecog _recogMovies = new RecogMovies();

        public string Title
        {
            get { return "Command"; }
        }

        public void Start(SpeechRecognitionEngine sre)
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Resources.RecogCommand)))
                sre.LoadGrammar(new Grammar(memoryStream));
        }

        public void Stop() { }

        public IRecog Process(RecognitionResult r, out bool show)
        {
            show = true;
            switch (r.Semantics.Value.ToString())
            {
                case "CANCEL":
                    show = false;
                    SystemSounds.Exclamation.Play();
                    return Listen.CancelRecog;
                case "EXIT":
                    show = false;
                    if (r.Confidence >= 0.6)
                    {
                        SystemSounds.Exclamation.Play();
                        Application.Exit();
                    }
                    return null;
                case "SERIES":
                    return _recogSeries;
                case "MOVIES":
                    return _recogMovies;
                case "PLAY":
                    Vlc.Play();
                    return Listen.ResetRecog;
                case "PAUSE":
                    Vlc.Pause();
                    return Listen.ResetRecog;
                default:
                    return null;
            }
        }
    }
}
