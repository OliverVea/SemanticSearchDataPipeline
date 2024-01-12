namespace DataPipelines.Models;

public record TextData : Data
{
    public required string Text { get; init; }
}