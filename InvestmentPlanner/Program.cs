using System;

namespace InvestmentPlanner
{
    class Program
    {
        static void Main(string[] args)
        {
            DetermineUsersRequiredMonthlyInput();
            Console.WriteLine();

            Console.WriteLine("Welcome to investment basis let us begin with a few questions to evaluate your investment results.");
            var basis = GetInvestmentBasis();

            int maxYield = 10;
            Console.WriteLine($"Initial Parameters: Initial Investment Amount: {basis.InitialInvestment.Value}, Monthly Input: {basis.MonthlyContributions.Value}, Yield Range: 0-{maxYield}, # Of Years: {basis.Years.Value}");
            Console.WriteLine();

            for (int i = 0; i <= maxYield; i++)
            {
                var investmentResult = CalculateReturn(basis, i / 100d, basis.Years.Value);
                Console.WriteLine($"    With an initial investment of: {basis.InitialInvestment.Value} and monthly input of {basis.MonthlyContributions.Value}");
                Console.WriteLine($"    Total of all deposits without adjustment: {investmentResult.TotalWithoutGrowth}");
                Console.WriteLine($"    Max possible return of: {investmentResult.Total}");
                Console.WriteLine($"    With a total growth of: {investmentResult.Growth} for a given growth factor of: %{i}");
                Console.WriteLine($"    With a possible Monthly Growth Rate of {investmentResult.MonthlyGrowthRate}");
                Console.WriteLine("---------------------------------------------------------------------");
                Console.WriteLine();
            }

            Console.WriteLine("Above are your results based on provided parameters on what growth you may expect.");

            Console.WriteLine("Do you wish to try different parameters? (y/n)");
            if (GetUserResponse())
            {
                Main(null);
            }
        }

        static bool GetUserResponse()
        {
            var userResponse = Console.ReadKey();
            Console.WriteLine();
            if (userResponse.Key == ConsoleKey.N) return false;
            if (userResponse.Key == ConsoleKey.Y) return true;
            Console.WriteLine("Please enter (y/n).");
            return GetUserResponse();
        }

        static InvestmentReturn CalculateReturn(InvestmentBasis basis, double annualpr, int yearsToInvest)
        {
            var result = new InvestmentReturn
            {
                Total = basis.InitialInvestment.Value,
                TotalWithoutGrowth = basis.InitialInvestment.Value + (basis.MonthlyContributions.Value * (12 * yearsToInvest)),
                MonthlyPercentageRate = annualpr / 12d
            };

            for (int i = 0; i < 12 * yearsToInvest; i++)
            {
                var newMonthlyTotal = result.Total + basis.MonthlyContributions.Value;
                var monthlyGrowth = newMonthlyTotal * result.MonthlyPercentageRate;
                result.Total = newMonthlyTotal + monthlyGrowth;
                result.Growth += monthlyGrowth;
            }
            
            return result;
        }

        static InvestmentBasis GetInvestmentBasis(InvestmentBasis basis = null)
        {
            basis = basis ?? new InvestmentBasis();

            if (!basis.InitialInvestment.HasValue)
            {
                Console.WriteLine("Please input your initial investment amount:");
                var val = GetUserInputValue();
                basis.InitialInvestment = val;
            }

            if (!basis.MonthlyContributions.HasValue)
            {
                Console.WriteLine("Please input your expected monthly contributions:");
                var val = GetUserInputValue();
                basis.MonthlyContributions = val;
            }

            if (!basis.Years.HasValue)
            {
                Console.WriteLine("Please input the number of years you plan to invest: (1 or more)");
                var val = GetUserInputValue();
                if (val == 0) { GetInvestmentBasis(basis); }
                basis.Years = (int)val;
            }

            return basis;
        }

        static double GetUserInputValue()
        {
            var input = Console.ReadLine();
            if (!double.TryParse(input, out var result) || result < 0)
            {
                Console.WriteLine("Please input an appropriate value");
                return GetUserInputValue();
            }

            return result;
        }

        static void DetermineUsersRequiredMonthlyInput()
        {
            Console.WriteLine("Would you like to see what you would need to invest to meet a monthly withdraw goal? (y/n)");
            if (!GetUserResponse())
            {
                return;
            }

            double initialInvestment = 0;
            Console.WriteLine("Do you intend to have an initial starting investment? (y/n)");
            if (GetUserResponse())
            {
                Console.WriteLine("How much do you intend to initially invest?");
                initialInvestment = GetUserInputValue();
            }

            Console.WriteLine("How much do you intend to draw monthly?");
            var monthlyIncome = GetUserInputValue();

            for (int i = 1; i <= 5; i++) //Decades
            {
                Console.WriteLine($"For an investment duration of {i * 10} years");

                for (int j = 0; j <= 10; j++) //APR 1-10%
                {
                    // This needs refinement as it is not accurate as to how compound interest should work
                    // In theory, this needs to calculate roughly a total then build up
                    var mpr = j / 100d / 12d;
                    var months = 12 * i * 10;
                    var total = monthlyIncome / mpr * 1.2d; //1.2d to provide an overhead to offset underperformance
                    var monthlyInput = (total - initialInvestment) / months;

                    Console.WriteLine($"For an income of {monthlyIncome}, you would need a total of {total} at a APR of %{j}");
                    Console.WriteLine($"You would need to invest {monthlyInput} monthly");
                    Console.WriteLine();
                }

                Console.WriteLine("---------------------------------------------------------------------");
                if (i < 5)
                {
                    Console.WriteLine("Press any key to continue loading results");
                    Console.ReadKey();
                }
            }


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press any key to continue to investment basis.");
            Console.ReadKey();
        }
    }

    public class InvestmentReturn
    {
        public double Growth { get; set; }
        public double Total { get; set; }
        public double TotalWithoutGrowth { get; set; }
        public double MonthlyPercentageRate { get; set; }
        public double MonthlyGrowthRate { get { return Total * MonthlyPercentageRate; } }
    }

    public class InvestmentBasis
    {
        public double? InitialInvestment { get; set; }
        public double? MonthlyContributions { get; set; }
        public int? Years { get; set; }
    }
}