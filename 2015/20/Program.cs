var input = 34000000;

Console.WriteLine(WorkOutHouse(input, 10, SumFactors));
Console.WriteLine(WorkOutHouse(input, 11, SumFactorsP2));

long WorkOutHouse(long target, int presentsPerElf, Func<long, long> SumFactorsMethod)
{
    long presents = 1;
    long houseNumber = 1;
    long i = 1;

    while(presents * presentsPerElf < input)
    {
        i++;
        houseNumber *= i;
        presents = SumFactors(houseNumber);
    }

    long upperBound = houseNumber;
    var lowerBound = houseNumber/i;
    var jump = houseNumber;

    while(i > 0)
    {
        jump = jump / i;
        var prevUpper = upperBound;

        for (long j = 0; j < upperBound; j += jump)
        {
            if (SumFactorsMethod(lowerBound + j) * presentsPerElf > input)
            {
                upperBound = lowerBound + j;
                break;
            }
        }

        if (prevUpper == upperBound)
        {
            break;
        }

        i--;
    }

    return upperBound;
}

long SumFactors(long num)
{
    long ans = 1 + num;
    for (long i = 2; i <= num / 2; i++)
    {
        if (num % i == 0)
        ans += i;
    }

    return ans;
}

long SumFactorsP2(long num)
{
    long ans = num;

    for (long i = num/51 + 1; i <= num / 2; i++)
    {
        if (num % i == 0 && num / i <= 50)
        {
            ans += i;
        }
    }

    return ans;
}