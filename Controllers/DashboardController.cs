using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Models;
using System.Globalization;

namespace MoneyControl.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index()
        {
            // last 7 days
            DateTime StartTime = DateTime.Today.AddDays(-6);
            DateTime EndTime = DateTime.Today;

            List<Transcation> SelectedTransactions = await _context.Transcations
                .Include(x => x.category)
                .Where(y => y.Date >= StartTime && y.Date <= EndTime)
                .ToListAsync();

            //Total Income
            int TotalIncome = SelectedTransactions
                .Where(i => i.category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(i => i.category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C0}", Balance);

            //doughnut chart
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.category.Type == "Expense")
                .GroupBy(j => j.category.categoryId)
                .Select(k => new
                {
                    CategoryTitleWithIcon = k.First().category.Icon + " " + k.First().category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            //SplineData Chart
            //Income
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                })
                .ToList();

            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                })
                .ToList();

            //Combine Income & Expense
            string[] Last7Days = Enumerable.Range(0, 7)
                .Select(i => StartTime.AddDays(i).ToString("dd-MMM"))
                .ToArray();
            ViewBag.SplineChartData = from day in Last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };
            //Recent transactions
            ViewBag.RecentTransactions = await _context.Transcations
               .Include(i => i.category)
               .OrderByDescending(j => j.Date)
               .Take(5)
               .ToListAsync();

            return View();

        }
    }
    public class SplineChartData 
        {
        public string day;
        public int income;
        public int expense;
        }
}
