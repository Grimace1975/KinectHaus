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
            {
                var g = new Grammar(memoryStream);
                sre.LoadGrammar(g);
            }
        }

        public void Stop()
        {
        }

        public IRecog Process(RecognitionResult r)
        {
            switch (r.Semantics.Value.ToString())
            {
                case "EXIT":
                    SystemSounds.Exclamation.Play();
                    if (r.Confidence >= 0.6)
                        Application.Exit();
                    return null;
                case "SERIES":
                    return _recogSeries;
                case "MOVIES":
                    return _recogMovies;
                case "PLAY":
                    SystemSounds.Exclamation.Play();
                    Vlc.Play();
                    return null;
                case "PAUSE":
                    SystemSounds.Exclamation.Play();
                    Vlc.Pause();
                    return null;
                default:
                    return null;
            }
        }
    }
}
