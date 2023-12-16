var input = File.ReadAllLines("./input.txt");
var contraption = new Dictionary<(int, int), char>();
var rowCount = 0;
foreach (var line in input)
{
    for (var i = 0; i < line.Length; i++)
    {
        contraption.Add((i, rowCount), line[i]);
    }
    rowCount++;
}

var contraptionWidth = contraption.Max(x => x.Key.Item1);
var contraptionHeight = contraption.Max(x => x.Key.Item2);
var startingBeams = new List<(int, int, int, Direction)>();
for (int i = 0; i <= contraptionHeight; i++)
{
    for (int j = 0; j <= contraptionWidth; j++)
    {
        if (i == 0) startingBeams.Add((j, i, 1, Direction.Down));
        if (i == contraptionHeight) startingBeams.Add((j, i, 1, Direction.Up));
        if (j == 0) startingBeams.Add((j, i, 1, Direction.Right));
        if (j == contraptionWidth) startingBeams.Add((j, i, 1, Direction.Left));
    }
}

var maxEnergisedCells = 0;
foreach (var startingBeam in startingBeams)
{
    var energisedCells = new List<(int, int)> { (startingBeam.Item1, startingBeam.Item2) };
    var beams = new List<(int, int, int, Direction)> { startingBeam };
    var usedBeams = new List<(int, int, Direction)>();
    while (beams.Any())
    {
        for (var i = 0; i < beams.Count; i++)
        {
            var beam = beams.ElementAt(i);
            var currentCell = (beam.Item1, beam.Item2);
            var currentDirection = beam.Item4;
            var currentCellValue = contraption[currentCell];
            if (usedBeams.Contains((beam.Item1, beam.Item2, beam.Item4)))
            {
                beams.RemoveAll(s => s.Item1 == beam.Item1 && s.Item2 == beam.Item2 && s.Item4 == beam.Item4);
                continue;
            }

            usedBeams.Add((beam.Item1, beam.Item2, beam.Item4));

            if (currentCellValue == '.' ||
                (currentCellValue == '-' &&
                 (currentDirection == Direction.Left || currentDirection == Direction.Right)) ||
                (currentCellValue == '|' && (currentDirection == Direction.Up || currentDirection == Direction.Down)))
            {
                beam = MoveOn(beam);
            }

            if (currentCellValue == '/' || currentCellValue == '\\')
            {
                beam = MoveMirrored(currentCellValue, beam);
            }

            if ((currentCellValue == '-' && (currentDirection == Direction.Up || currentDirection == Direction.Down)) ||
                (currentCellValue == '|' &&
                 (currentDirection == Direction.Left || currentDirection == Direction.Right)))
            {
                var returnedBeams = SplitBeam(beam, beams.Max(x => x.Item3) + 1);
                beam = returnedBeams[0];
                beams.Add(returnedBeams[1]);
            }

            if (beam.Item1 < 0 || beam.Item2 < 0 || beam.Item1 > contraptionWidth || beam.Item2 > contraptionHeight)
            {
                beams.RemoveAll(s => s.Item3 == beam.Item3);
            }
            else
            {
                beams.RemoveAll(s => s.Item3 == beam.Item3);
                beams.Add(beam);
            }

            foreach (var b in beams)
            {
                if (!energisedCells.Contains((b.Item1, b.Item2)))
                {
                    energisedCells.Add((b.Item1, b.Item2));
                }
            }
        }
    }

    if (energisedCells.Count > maxEnergisedCells)
    {
        maxEnergisedCells = energisedCells.Count;
        Console.WriteLine($"{startingBeam.Item1},{startingBeam.Item2},{startingBeam.Item4} - {maxEnergisedCells}");
    }
}

Console.WriteLine(maxEnergisedCells);

static List<(int, int, int, Direction)> SplitBeam((int, int, int, Direction) beam, int nextBeamId)
{
    var returnedBeams = new List<(int, int, int, Direction)>();
    (int, int, int, Direction) newBeam = (0, 0, 0, Direction.Up);
    if (beam.Item4 == Direction.Up || beam.Item4 == Direction.Down)
    {
        newBeam = (beam.Item1++, beam.Item2, nextBeamId, Direction.Right);
        beam.Item1--;
        beam.Item4 = Direction.Left;
    }
    else if (beam.Item4 == Direction.Left || beam.Item4 == Direction.Right)
    {
        newBeam = (beam.Item1, beam.Item2++, nextBeamId, Direction.Down);
        beam.Item2--;
        beam.Item4 = Direction.Up;
    }
    returnedBeams.Add(beam);
    returnedBeams.Add(newBeam);
    return returnedBeams;
}

static (int, int, int, Direction) MoveMirrored(char mirrorChar, (int, int, int, Direction) beam)
{

    if (beam.Item4 == Direction.Up && mirrorChar == '/')
    {
        beam.Item1++;
        beam.Item4 = Direction.Right;
    }
    else if (beam.Item4 == Direction.Down && mirrorChar == '/')
    {
        beam.Item1--;
        beam.Item4 = Direction.Left;
    }
    else if (beam.Item4 == Direction.Left && mirrorChar == '/')
    {
        beam.Item2++;
        beam.Item4 = Direction.Down;
    }
    else if (beam.Item4 == Direction.Right && mirrorChar == '/')
    {
        beam.Item2--;
        beam.Item4 = Direction.Up;
    }
    else if (beam.Item4 == Direction.Up && mirrorChar == '\\')
    {
        beam.Item1--;
        beam.Item4 = Direction.Left;
    }
    else if (beam.Item4 == Direction.Down && mirrorChar == '\\')
    {
        beam.Item1++;
        beam.Item4 = Direction.Right;
    }
    else if (beam.Item4 == Direction.Left && mirrorChar == '\\')
    {
        beam.Item2--;
        beam.Item4 = Direction.Up;
    }
    else if (beam.Item4 == Direction.Right && mirrorChar == '\\')
    {
        beam.Item2++;
        beam.Item4 = Direction.Down;
    }
    return beam;
}

static (int, int, int, Direction) MoveOn((int, int, int, Direction) beam)
{

    if (beam.Item4 == Direction.Up)
    {
        beam.Item2--;
    }
    if (beam.Item4 == Direction.Down)
    {
        beam.Item2++;
    }
    if (beam.Item4 == Direction.Left)
    {
        beam.Item1--;
    }
    if (beam.Item4 == Direction.Right)
    {
        beam.Item1++;
    }
    return beam;
}

void DisplayEnergisedCells(List<(int, int)> energisedCells)
{
    for (int i = 0; i <= contraptionHeight; i++)
    {
        for (int j = 0; j <= contraptionWidth; j++)
        {
            if (energisedCells.Contains((j, i)))
            {
                Console.Write('#');

            }
            else
            {
                Console.Write('.');
            }
        }
        Console.WriteLine();
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}