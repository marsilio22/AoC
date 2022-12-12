(int row, int col) input = (2947, 3029);
// (int row, int col) input = (3, 5);

var prev = 20_151_125L;

var mult = 252_533L;
var modu = 33_554_393L;

(int row, int column) pos = (1, 1);
var currentDiag = 1;

while (pos != input)
{
    if (pos.row == 1)
    {
        currentDiag++;
        pos = (currentDiag, 1);
    }
    else 
    {
        pos = (pos.row - 1, pos.column + 1);
    }

    prev = (prev * mult) % modu;
}

Console.WriteLine(prev);