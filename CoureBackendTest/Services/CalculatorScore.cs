namespace CoureBackendTest.Services;

    public static class CalculatorScore
    {
        public static int CalculateScore(IEnumerable<int> numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);

            int score = 0;

            foreach (int number in numbers)
            {
                score += number % 2 == 0 ? 1 : 3;

                if (number == 8)
                {
                    score += 5;
                }
            }

            return score;
        }
    }
