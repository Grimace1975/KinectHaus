using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Globalization;

namespace KinectHaus
{
    public class Recog
    {
        static SpeechRecognitionEngine _sre;
        static Action<string> _outputAction;

        public static void Start(Action<string> outputAction)
        {
            _outputAction = outputAction;
            
            var directions = new Choices();
            directions.Add(new SemanticResultValue("forward", "FORWARD"));
            directions.Add(new SemanticResultValue("forwards", "FORWARD"));
            directions.Add(new SemanticResultValue("straight", "FORWARD"));
            directions.Add(new SemanticResultValue("backward", "BACKWARD"));
            directions.Add(new SemanticResultValue("backwards", "BACKWARD"));
            directions.Add(new SemanticResultValue("back", "BACKWARD"));
            directions.Add(new SemanticResultValue("turn left", "LEFT"));
            directions.Add(new SemanticResultValue("turn right", "RIGHT"));
            directions.Add(new SemanticResultValue("end", "END"));
            var gb = new GrammarBuilder { Culture = new CultureInfo("en-US") };
            gb.Append(directions);
            var g = new Grammar(gb);

            _sre = new SpeechRecognitionEngine(new CultureInfo("en-US"));
            _sre.LoadGrammar(g);
            _sre.SpeechRecognized += SpeechRecognized;
            _sre.SpeechRecognitionRejected += SpeechRejected;
            _sre.SetInputToDefaultAudioDevice();
            //_sre.SetInputToAudioStream(sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            _sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        public static void Stop()
        {
            _sre.SpeechRecognized -= SpeechRecognized;
            _sre.SpeechRecognitionRejected -= SpeechRejected;
            _sre.RecognizeAsyncStop();
        }

        private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            const double ConfidenceThreshold = 0.3; // Speech utterance confidence below which we treat speech as if it hadn't been heard
            if (e.Result.Confidence >= ConfidenceThreshold)
                _outputAction(e.Result.Semantics.Value.ToString());
        }

        private static void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            _outputAction(e.Result.Confidence.ToString());
        }
    }
}
