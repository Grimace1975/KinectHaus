using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public class Listen
    {
        static readonly IRecog _recogIdle = new RecogIdle();
        readonly Stack<IRecog> _recogStack = new Stack<IRecog>();
        readonly Action<string, string, ListenIcon> _balloonTip;
        IRecog _recog = _recogIdle;

        public Listen(Action<string, string, ListenIcon> balloonTip)
        {
            _balloonTip = balloonTip;
        }

        #region Listener

        KinectSensor _sensor;
        SpeechRecognitionEngine _sre;

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
            MoveToIdle();
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

        public static string CleanPath(string x)
        {
            return x.Split('(')[0].Trim();
        }

        #endregion

        private void MoveToIdle()
        {
            _recog.Start(_sre);
            _balloonTip("KinectHaus\u2122", "My name is SLAVE", ListenIcon.Info);
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            lock (_recog)
                if (e.Result.Confidence >= 0.3)
                {
                    var text = string.Format("{0} {1:P0}", e.Result.Semantics.Value, e.Result.Confidence);
                    _balloonTip(_recog.Title, text, ListenIcon.None);
                    var r = _recog.Process(e.Result);
                    if (r != null)
                    {
                        _recogStack.Push(_recog);
                        _recog = r;
                        _sre.UnloadAllGrammars();
                        _recog.Start(_sre);
                    }
                }
        }

        private void SpeechRevert()
        {
            lock (_recog)
                if (_recogStack.Count > 0)
                {
                    _recog = _recogStack.Pop();
                    _sre.UnloadAllGrammars();
                    _recog.Start(_sre);
                }
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            var text = string.Format("{0} {1}", e.Result.Semantics.Value, e.Result.Confidence);
            _balloonTip(_recog.Title, text, ListenIcon.Error);
        }
    }
}
