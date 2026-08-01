namespace SimpleWorkoutQTE.Models;

public record EasyMode
{
    public bool Enable { get; set; } = false;
    public int NumberOfReps { get; set; } = 15;
}