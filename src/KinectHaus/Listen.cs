﻿using System;
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
        public static readonly IRecog CancelRecog = new Recog();
        public static readonly IRecog ResetRecog = new Recog();
        static readonly IRecog _recogIdle = new RecogIdle();
        readonly ListenContext _listenCtx;

        public Listen(ListenContext listenCtx)
        {
            _listenCtx = listenCtx;
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
            RecogStackClear();
            _listenCtx.BalloonTip(5, "KinectHaus\u2122", string.Format("My name is {0}", RecogIdle.Name), ListenIcon.Info);
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
            TimerStop();
        }

        public static string CleanPath(string x)
        {
            x = Path.GetFileName(x);
            return x.Split('(')[0].Trim();
        }

        #endregion

        #region RecogStack

        readonly Stack<IRecog> _recogStack = new Stack<IRecog>();
        IRecog _recog;

        private void RecogStackClear()
        {
            _recogStack.Clear();
            _sre.UnloadAllGrammars();
            _recog = _recogIdle;
            _recog.Start(_listenCtx, _sre);
            SystemSounds.Beep.Play();
            TimerStop();
        }

        private void RecogStackPop()
        {
            if (_recogStack.Count == 1)
                RecogStackClear();
            else if (_recogStack.Count > 1)
            {
                _recog = _recogStack.Pop();
                _sre.UnloadAllGrammars();
                _recog.Start(_listenCtx, _sre);
            }
        }

        private void RecogStackPush(IRecog item)
        {
            _recogStack.Push(_recog);
            _recog = item;
            _sre.UnloadAllGrammars();
            _recog.Start(_listenCtx, _sre);
        }

        #endregion

        #region Timer

        static readonly TimeSpan _timerPeriod = new TimeSpan(0, 0, 2);
        Timer _timer;
        DateTime _endTime;

        public void TimerStart(int seconds)
        {
            return;
            if (_timer == null)
                _timer = new Timer(new TimerCallback(TimerCompletionCallback), null, _timerPeriod, _timerPeriod);
            _endTime = DateTime.Now.AddSeconds(seconds);
        }

        public void TimerStop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        private void TimerCompletionCallback(object state)
        {
            if (_endTime < DateTime.Now)
                RecogStackClear();
        }

        #endregion

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            lock (_recog)
                if (e.Result.Confidence >= 0.3)
                {

                    var r = _recog.Process(e.Result);
                    if (r == ResetRecog)
                        RecogStackClear();
                    else if (r == CancelRecog)
                    {
                        RecogStackPop();
                        TimerStart(5);
                    }
                    else if (r != null)
                    {
                        RecogStackPush(r);
                        TimerStart(15);
                    }
                    else
                        TimerStart(10);
                }
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            lock (_recog)
            {
                TimerStart(5);
  
            }
        }
    }
}
