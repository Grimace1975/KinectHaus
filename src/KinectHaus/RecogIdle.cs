using System;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;
using KinectHaus.Properties;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class RecogIdle : IRecog
    {
        static readonly IRecog _recogCommand = new RecogCommand();
        static readonly TimeSpan _timerPeriod = new TimeSpan(0, 0, 2);
        Timer _timer;

        public string Title
        {
            get { return "Idle"; }
        }

        public void Start(SpeechRecognitionEngine sre)
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Resources.Recog)))
            {
                var g = new Grammar(memoryStream);
                sre.LoadGrammar(g);
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        public IRecog Process(RecognitionResult r)
        {
            switch (r.Semantics.Value.ToString())
            {
                case "COMMAND":
                    SystemSounds.Exclamation.Play();
                    //_timer = new Timer(new TimerCallback(TimerCompletionCallback), null, _timerPeriod, _timerPeriod);
                    return _recogCommand;
                default:
                    return null;
            }
        }

        private void TimerCompletionCallback(object state)
        {
            SystemSounds.Beep.Play();
        }
    }
}
