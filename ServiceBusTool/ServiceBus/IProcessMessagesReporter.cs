namespace ServiceBusTool.ServiceBus;

public interface IProcessMessagesReporter
{
    void ReportProgress(ProcessedMessagesStatistics statistics);
}