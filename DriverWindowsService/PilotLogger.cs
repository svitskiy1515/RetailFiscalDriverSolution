using System;
using Fw21;
using Serilog;

namespace DriverWindowsService
{
    public class PilotLogger : LogWriterBase
    {
        private const string Source = "PilotFw21 {0}";
        public override void Trace(Exception ex)
        {
            Log.Error(ex,Source);
        }

        public override void Trace(string text, EcrCtrlConfig.LogLevel entryLevel)
        {
            Log.Information(Source,text);
        }

        public override void Trace(byte[] data, string description)
        {
            TraceInternal(null, data, description);
        }

        public override void Trace(DateTime dt, byte[] data, string description)
        {
            TraceInternal(dt, data, description);
        }

        private void TraceInternal(DateTime? dt, byte[] data, string description)
        {
            if (!string.IsNullOrEmpty(description))
                Log.Information(Source, description);
            foreach (var item in ToStrings(data, dt))
                Log.Information(Source, item);
        }

        protected override object Mutex
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}