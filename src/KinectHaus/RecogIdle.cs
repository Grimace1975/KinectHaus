using System.IO;
using System.Media;
using System.Text;
using KinectHaus.Properties;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogIdle : IRecog
    {
        static readonly IRecog _recogCommand = new RecogCommand();

        public string Title
        {
            get { return "Idle"; }
        }

        public void Start(SpeechRecognitionEngine sre)
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Resources.Recog)))
                sre.LoadGrammar(new Grammar(memoryStream));
        }

        public void Stop() { }

        public IRecog Process(RecognitionResult r, out bool show)
        {
            switch (r.Semantics.Value.ToString())
            {
                case "COMMAND":
                    SystemSounds.Exclamation.Play();
                    show = true;
                    return _recogCommand;
            }
            show = false;
            return null;
        }
    }
}
