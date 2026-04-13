using _simpleWorkoutQTE.Models.Enums;

namespace _simpleWorkoutQTE.Models;

public record NonEasyMode
{
    public int NumberOfEvents { get; set; }
    public float StartSpeed { get; set; }
    public float EndSpeed { get; set; }
    public float StartX { get; set; }
    public float EndX { get; set; }
    public float StartY { get; set; }
    public float EndY { get; set; }
    public QteProgressionType ProgressionType { get; set; }
}