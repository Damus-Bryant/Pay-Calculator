using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using CsvHelper;

namespace OO_programming
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }


    /// <summary>
    /// Class a capture details accociated with an employee's pay slip record
    /// </summary>
    public class PaySlip
    {
        public Employee Employee { get; set; }
        public double HoursWorked { get; set; }
        public double GrossPay { get; set; }
        public double Superannuation { get; set; }
        public double Tax { get; set; }
        public double NetPay { get; set; }

    }

    /// <summary>
    /// Base class to hold all Pay calculation functions
    /// Default class behaviour is tax calculated with tax threshold applied
    /// </summary>
    public class PayCalculator
    {
        
        
        public static double CalculateGrossPay(double HoursWorked, double HourlyRate)
        {
            return Math.Round(HourlyRate * HoursWorked,2);
        }

        public static double CalculateSuper(double GrossPay)
        {
            return Math.Round(GrossPay * 0.11,2);
        }
    }

    /// <summary>
    /// Extends PayCalculator class handling No tax threshold
    /// </summary>
    public class PayCalculatorNoThreshold : PayCalculator
    {
        public static double CalcualatTax(double GrossPay)
        {
            // read csv file
            List<TaxRate> taxRates = new List<TaxRate>();
            CultureInfo culture = CultureInfo.CurrentCulture;
            StreamReader strReader = new StreamReader(@"..\..\input\taxrate-nothreshold.csv");
            CsvReader csvReader = new CsvReader(strReader, culture);
            taxRates = csvReader.GetRecords<TaxRate>().ToList();
            strReader.Close();

            foreach (TaxRate taxRate in taxRates)
            {
                if (GrossPay <= taxRate.MaxWkSalary)
                {
                    return Math.Round(((GrossPay + 0.99) * taxRate.TaxRateA - taxRate.TaxRateB),2);
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// Extends PayCalculator class handling With tax threshold
    /// </summary>
    public class PayCalculatorWithThreshold : PayCalculator
    {
     public static double CalcualatTax(double GrossPay)
        {
            // read csv file
            List<TaxRate> taxRates = new List<TaxRate>();
            CultureInfo culture = CultureInfo.CurrentCulture;
            StreamReader strReader = new StreamReader(@"..\..\input\taxrate-withthreshold.csv");
            CsvReader csvReader = new CsvReader(strReader, culture);
            taxRates = csvReader.GetRecords<TaxRate>().ToList();
            strReader.Close();

            foreach (TaxRate taxRate in taxRates)
            {
                if(GrossPay <= taxRate.MaxWkSalary)
                {
                    return Math.Round(((GrossPay + 0.99) * taxRate.TaxRateA - taxRate.TaxRateB), 2);
                }
            }
            return 0;
        }
    }

    public class Employee
    {
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double HourlyRate { get; set; }
        public string TaxFreeThreshold { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }

    public class TaxRate
    {
        //MinWkSalary,MaxWkSalary,TaxRateA,TaxRateB
        public double MinWkSalary { get; set; }
        public double MaxWkSalary { get; set; }
        public double TaxRateA { get; set; }
        public double TaxRateB { get; set; }
    }

}
