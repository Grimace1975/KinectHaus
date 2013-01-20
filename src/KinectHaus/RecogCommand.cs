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
            switch (r.Semantics.Value.ToString())
            {
                case "CANCEL":
                    SystemSounds.Exclamation.Play();
                    show = false;
                    return Listen.CancelRecog;
                case "EXIT":
                    if (r.Confidence >= 0.6)
                    {
                        SystemSounds.Exclamation.Play();
                        Application.Exit();
                        show = true;
                        return null;
                    }
                    break;
                case "SERIES":
                    if (r.Confidence >= 0.5)
                    {
                        show = true;
                        return _recogSeries;
                    }
                    break;
                case "MOVIES":
                    if (r.Confidence >= 0.5)
                    {
                        show = true;
                        return _recogMovies;
                    }
                    break;
                case "PLAY": Vlc.Play(); show = true; return Listen.ResetRecog;
                case "PAUSE": Vlc.Pause(); show = true; return Listen.ResetRecog;
            }
            show = false;
            return null;
        }
    }
}
