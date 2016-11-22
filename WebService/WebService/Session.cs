using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;

namespace WebService
{
    public class Session : User
    {
        public bool Status { get; set; }
        private Timer SessionTimer = new Timer(1200000);

        public Session(string rName, string rHash, int rToken)
        {
            Status = true;
            name = rName;
            hash = rHash;
            token = rToken;

            SessionTimer.Enabled = true;
            SessionTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        public void Refresh()
        {
            SessionTimer.Stop();
            SessionTimer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            //session has expired token will be removed from the list of valid "active users" and session will be destroyed
            SessionTimer.Enabled = false;
            SessionTimer.Dispose();
            Status = false;
        }
    }
}