

namespace UtilityBot.Utilities
{
    public static class SumCalculator
    {

        public static string Calculation(string inputMessage, out string result)
        {
            result = String.Empty;
            var test1 = inputMessage.Replace(" -","");
            var test2 = test1.Replace(" ","");

            if (int.TryParse(test2, out int success))
            {
                string[] numbers = inputMessage.Split(" ");
                int sum = 0;
                int[] array = new int[numbers.Length];
                for (int i = 0; i < numbers.Length; i++)
                {
                    sum += int.Parse(numbers[i]);
                    array[i] = int.Parse(numbers[i]);
                }
                result = sum.ToString();
                return result;
            }
            else
            {
                result = "не число. Вы ввели что-то не то. Прочитайте условие и попробуйте заново";
                return result;
            }
        }
    }
}
