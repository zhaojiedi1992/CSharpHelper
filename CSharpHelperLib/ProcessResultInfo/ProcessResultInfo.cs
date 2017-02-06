using System;

namespace CSharpHelperLib.ProcessResultInfo
{
    public class ProcessResultInfo
    {
        public ProcessResultInfo()
        {
            Reset();
        }

        public void Reset()
        {
            InputMessage = string.Empty;
            OutMessage = string.Empty;
            ErrorMessage = string.Empty;
            ExitCode = int.MinValue;
        }

        public string InputMessage { get; set; }
        public string OutMessage { get; set; }
        public string ErrorMessage { get; set; }
        public int ExitCode { get; set; }
        public TimeSpan TotalProcessorTime { get; set; }
    }
}
