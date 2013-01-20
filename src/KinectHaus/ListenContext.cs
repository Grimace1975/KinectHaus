using System;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
namespace KinectHaus
{
    public class ListenContext
    {
        readonly Form _form;
        readonly Action<int, string, string, ListenIcon> _balloonTip;

        public ListenContext(Form form, Action<int, string, string, ListenIcon> balloonTip)
        {
            _form = form;
            _balloonTip = balloonTip;
        }

        public void BalloonTip(string title, RecognitionResult r)
        {
            var args = r.Semantics.Value.ToString().Split('|');
            var text = string.Format("{0} {1:P0}", args[0], r.Confidence);
            _balloonTip(15, title, text, ListenIcon.Info);
        }
        public void BalloonTip(int time, string title, string text, RecognitionResult r, ListenIcon icon)
        {
            var args = r.Semantics.Value.ToString().Split('|');
            text = string.Format("{0} {1:P0}", text, r.Confidence);
            _balloonTip(time, title, text, icon);
        }

        public void BalloonTip(int time, string title, string text, ListenIcon icon)
        {
            _balloonTip(time, title, text, icon);
        }

        public void Exit()
        {
            _form.Close();
            Application.Exit();
        }
    }
}
