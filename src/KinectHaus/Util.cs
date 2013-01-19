using System;
using System.Threading;

namespace KinectHaus
{
    public class Util
    {
        public static bool CallWithTimeout(Action action, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () => { threadToKill = Thread.CurrentThread; action(); };
            var result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
                return true;
            }
            threadToKill.Abort();
            return false;
        }
    }
}
