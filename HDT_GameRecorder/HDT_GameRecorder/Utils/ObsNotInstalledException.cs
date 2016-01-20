using System;


namespace HDT_GameRecorder.Utils
{
    class ObsNotInstalledException : Exception
    {
        public ObsNotInstalledException() { }

        public ObsNotInstalledException(string message) : base(message) { }

        public ObsNotInstalledException(string message, Exception inner) : base(message, inner) { }
    }
}
