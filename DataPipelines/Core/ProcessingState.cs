namespace DataPipelines.Core;

public enum ProcessingState
{
    None,
    WaitingForInput,
    OutputQueueFull,
    Processing,
    Cancelled,
    NoInputPipe,
    NoOutputPipes
}