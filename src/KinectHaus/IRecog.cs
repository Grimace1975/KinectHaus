using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public interface IRecog
    {
        string Title { get; }
        void Start(SpeechRecognitionEngine sre);
        void Stop();
        IRecog Process(RecognitionResult r, out bool show);
    }

    internal class Recog : IRecog
    {
        public string Title
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Start(SpeechRecognitionEngine sre)
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public IRecog Process(RecognitionResult r, out bool show)
        {
            show = false;
            throw new System.NotImplementedException();
        }
    }
}
