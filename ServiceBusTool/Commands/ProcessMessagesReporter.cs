using ServiceBusTool.ServiceBus;
using ShellProgressBar;

namespace ServiceBusTool.Commands;

public class ProcessMessagesReporter : IProcessMessagesReporter, IDisposable
{
    private readonly ProgressBar _progressBar;
    private readonly IProgress<float> _progress;

    public ProcessMessagesReporter()
    {
        _progressBar = new ProgressBar(10000, "Processed messages"); 
        _progress = _progressBar.AsProgress<float>();
    }

    public void ReportProgress(ProcessedMessagesStatistics statistics) =>
        _progress.Report(statistics.TotalMessagesProcessed / (statistics.TotalMessages + float.Epsilon));

    public void Dispose() => _progressBar?.Dispose();
}