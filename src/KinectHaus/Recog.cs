using KinectHaus.Properties;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KinectHaus
{
    public class Recog
    {
        KinectSensor _sensor;
        SpeechRecognitionEngine _sre;
        Action<string> _outputAction;

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
            State = RecogState.Idle;
        }

        private SpeechRecognitionEngine StartKinect()
        {
            _sensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            if (_sensor != null)
                try { _sensor.Start(); }
                catch (IOException) { _sensor = null; }
            if (_sensor != null)
            {
                var ri = SpeechRecognitionEngine.InstalledRecognizers().FirstOrDefault(x =>
                {
                    string value;
                    return (x.AdditionalInfo.TryGetValue("Kinect", out value) && string.Equals("true", value, StringComparison.OrdinalIgnoreCase) && string.Equals("en-US", x.Culture.Name, StringComparison.OrdinalIgnoreCase));
                });
                if (ri != null)
                    return new SpeechRecognitionEngine(ri.Id);
            }
            return new SpeechRecognitionEngine(new CultureInfo("en-US"));
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
            const double ConfidenceThreshold = 0.3; // Speech utterance confidence below which we treat speech as if it hadn't been heard
            if (e.Result.Confidence >= ConfidenceThreshold)
                _outputAction(e.Result.Semantics.Value.ToString());
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            _outputAction(e.Result.Confidence.ToString());
        }
    }
}
