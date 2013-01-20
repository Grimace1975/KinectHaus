using Microsoft.Speech.Recognition;

namespace KinectHaus
{
    public interface IRecog
    {
        void Start(ListenContext ctx, SpeechRecognitionEngine sre);
        IRecog Process(RecognitionResult r);
    }

    internal class Recog : IRecog
    {
        public void Start(ListenContext ctx, SpeechRecognitionEngine sre)
        {
            throw new System.NotImplementedException();
        }

        public IRecog Process(RecognitionResult r)
        {
            throw new System.NotImplementedException();
        }
    }
}
