using System;
using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public interface IRecog
    {
        string Title { get; }
        void Start(SpeechRecognitionEngine sre);
        void Stop();
        IRecog Process(RecognitionResult r);
    }
}
