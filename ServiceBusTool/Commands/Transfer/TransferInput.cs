namespace ServiceBusTool.Commands.Transfer;

public record TransferInput(string Namespace, string SourcePath, string TargetQueueOrTopic);