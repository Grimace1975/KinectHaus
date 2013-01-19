using System;
using System.Windows.Forms;
namespace KinectHaus
{
    public class Vlc
    {
        static readonly string _path = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";

        public static void Play(string mediaPath)
        {
            if (string.IsNullOrEmpty(mediaPath))
                throw new ArgumentNullException("mediaPath");
            Extensions.SwitchApp("vlc", _path, "\"" + mediaPath + "\"");
        }

        public static void Play()
        {
            Extensions.ActivateApp("vlc", null, null);
            SendKeys.SendWait(" ");
        }

        public static void Pause()
        {
            Extensions.ActivateApp("vlc", null, null);
            SendKeys.SendWait(" ");
        }
    }
}
