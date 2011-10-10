using System;
using System.Threading;
using DJMatty.AMIP.ClientWrapper;

namespace BabelIm.Net.Xmpp.InstantMessaging.PersonalEventing {
    public sealed class AmipNowPlayingListerner {
        private readonly XmppSession session;
        private AMIPClient client;
        private Thread workerThread;

        internal AmipNowPlayingListerner(XmppSession session) {
            this.session = session;
        }

        public void Start() {
            Stop();

            workerThread = new Thread(Listen);
            workerThread.IsBackground = true;
            workerThread.Start();
        }

        public void Stop() {
            if (workerThread != null)
            {
                workerThread.Interrupt();
                workerThread.Join();
                workerThread = null;
            }
        }

        private void Listen() {
            string lastSong = null;

            try
            {
                // using will force dispose to be called -- dispose will ensure that the AMIP SDK is uninitialized
                // and any server listeners are removed
                using (client = new AMIPClient("127.0.0.1", 60333, 5000, 5, 1, true))
                {
                    while (true)
                    {
                        /* var_s		: Song
                         * var_sl		: Song length
                         * var_psec		: Position
                         * var_br		: Bit rate
                         * var_sr		: Sample rate
                         * var_typ		: Song mode e.g. Stereo or Mono
                         * var_channels	:	1 for Mono, 2 for Stereo
                         * var_stat		: Status
                         * 		0: Stopped
                         * 		3: Paused
                         * 		1: Playing
                         * 		other: Unknown
                         * var_fn		: Full file name with extension and path
                         * var_1		: Artist
                         * var_2		: Title
                         * var_4		: Album
                         * var_5		: Year
                         * var_6		: Comment
                         * var_7		: Genre
                         */

                        string currentSong = null;
                        int status = 0;

                        try
                        {
                            status = Convert.ToInt32(client.Eval("var_stat"));
                            currentSong = client.Eval("var_s");
                        }
                        catch (AMIPException)
                        {
                        }

                        if (status == 1 || status == 3)
                        {
                            if (!String.IsNullOrEmpty(currentSong) &&
                                (lastSong == null || lastSong != currentSong))
                            {
                                if (session.PersonalEventing.SupportsUserTune)
                                {
                                    string title = client.Eval("var_4");

                                    var tune = new XmppUserTuneEvent
                                        (
                                        client.Eval("var_1"),
                                        (ushort) (Convert.ToUInt16(client.Eval("var_sl"))*1000),
                                        null,
                                        ((!String.IsNullOrEmpty(title)) ? title : currentSong),
                                        client.Eval("var_2"),
                                        null,
                                        null
                                        );

                                    session.PublishTune(tune);

                                    lastSong = currentSong;
                                }
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(lastSong))
                            {
                                // If something has been played
                                session.StopTunePublication();

                                lastSong = null;
                            }
                        }


                        // Wait 10 seconds before continue checking if something is being playing
                        Thread.Sleep(10000);
                    }
                }
                ;
            }
            catch (ThreadInterruptedException)
            {
            }
            catch (Exception)
            {
            }

            client = null;
        }
    }
}