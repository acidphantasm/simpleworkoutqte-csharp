using SimpleWorkoutQTE.Models.Enums;

namespace SimpleWorkoutQTE.Models;

public record NonEasyMode
{
    public int NumberOfReps { get; set; } = 15;
    public float StartSpeed { get; set; } = 1;
    public float EndSpeed { get; set; } = 4;
    public float StartX { get; set; } = 0.45f;
    public float EndX { get; set; } = 0.0f;
    public float StartY { get; set; } = 0.175f;
    public float EndY { get; set; } = 0.07f;
    public QteProgressionType ProgressionType { get; set; } = QteProgressionType.Exponential;
}