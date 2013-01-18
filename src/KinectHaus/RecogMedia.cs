using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Globalization;

namespace KinectHaus
{
    public class RecogMedia
    {
        private void LoadGrammer(SpeechRecognitionEngine sre)
        {
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
            sre.LoadGrammar(g);
        }

    }
}
