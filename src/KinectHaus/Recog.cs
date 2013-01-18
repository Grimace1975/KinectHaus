using KinectHaus.Properties;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KinectHaus
{
    public interface IRecog
    {
        void Process(RecognitionResult r);
    }

    public class Recog : IRecog
    {
        KinectSensor _sensor;
        SpeechRecognitionEngine _sre;
        Action<string> _outputAction;
        IRecog _recog;

        public Recog(Action<string> outputAction)
        {
            _outputAction = outputAction;
        }

        public RecogState State { get; set; }

        private void Idle()
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Resources.Recog)))
            {
                var g = new Grammar(memoryStream);
                _sre.LoadGrammar(g);
            }
            _recog = this;
            State = RecogState.Idle;
            SystemSounds.Beep.Play();
        }

        private SpeechRecognitionEngine StartKinect()
        {
            _sensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            if (_sensor != null)
                try { _sensor.Start(); }
                catch (IOException) { _sensor = null; }
            if (_sensor == null)
                return new SpeechRecognitionEngine(new CultureInfo("en-US"));
            var ri = SpeechRecognitionEngine.InstalledRecognizers().FirstOrDefault(x =>
            {
                string value;
                return (x.AdditionalInfo.TryGetValue("Kinect", out value) && string.Equals("true", value, StringComparison.OrdinalIgnoreCase) && string.Equals("en-US", x.Culture.Name, StringComparison.OrdinalIgnoreCase));
            });
            return (ri != null ? new SpeechRecognitionEngine(ri.Id) : new SpeechRecognitionEngine(new CultureInfo("en-US")));
        }

        public void Start()
        {
            if (_sre != null)
                return;
            _sre = StartKinect();
            _sre.SpeechRecognized += SpeechRecognized;
            _sre.SpeechRecognitionRejected += SpeechRejected;
            if (_sensor != null)
                _sre.SetInputToAudioStream(_sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            else
                _sre.SetInputToDefaultAudioDevice();
            Idle();
            _sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Stop()
        {
            if (_sensor != null)
            {
                _sensor.AudioSource.Stop();
                _sensor.Stop();
                _sensor = null;
            }
            if (_sre != null)
            {
                _sre.SpeechRecognized -= SpeechRecognized;
                _sre.SpeechRecognitionRejected -= SpeechRejected;
                _sre.RecognizeAsyncStop();
                _sre = null;
            }
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.3)
            {
                _outputAction(e.Result.Semantics.Value.ToString());
                Process(e.Result);
            }
        }

        public void Process(RecognitionResult r)
        {
            switch (r.Semantics.Value.ToString())
            {
                case "KINECT":
                    SystemSounds.Beep.Play();
                    SystemSounds.Beep.Play();
                    switch (State)
                    {
                        case RecogState.Media:
                            _recog = new RecogMedia();
                        case RecogState.Vlc:
                            _recog = new RecogVlc();
                    }
                    break;
                case "EXIT":
                    SystemSounds.Exclamation.Play();
                    if (r.Confidence >= 0.6)
                        Application.Exit();
                    break;
            }
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            _outputAction(e.Result.Confidence.ToString());
        }

        public static bool CallWithTimeout(Action action, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () => { threadToKill = Thread.CurrentThread; action(); };
            var result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
                return true;
            }
            threadToKill.Abort();
            return false;
        }
    }
}
