using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using CsvHelper;

namespace OO_programming
{
    public partial class Form1 : Form
    {
        // module level variables so that can be used anywhere in module
        CultureInfo culture = CultureInfo.CurrentCulture;
        List<Employee> employees = new List<Employee>();
        PaySlip ps = new PaySlip();

        public Form1()
        {
            InitializeComponent();

            // Add code below to complete the implementation to populate the listBox
            // by reading the employee.csv file into a List of PaySlip objects, then binding this to the ListBox.
            // CSV file format: <employee ID>, <first name>, <last name>, <hourly rate>,<taxthreshold>
            StreamReader strReader = new StreamReader(@"..\..\input\employee.csv");
            CsvReader csvReader = new CsvReader(strReader, culture);
            employees = csvReader.GetRecords<Employee>().ToList();
            // display the employee names in listbox
            lbEmployee.DataSource = employees;
            lbEmployee.DisplayMember = "FullName";
            strReader.Close();

        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            // Add code below to complete the implementation to populate the
            // payment summary (textBox2) using the PaySlip and PayCalculatorNoThreshold
            // and PayCalculatorWithThresholds classes object and methods.

            // read and validate the hours
            double hours = 0;
            try
            {
                hours = double.Parse(tbHoursWorked.Text);
            }
            catch (Exception ex)
            {
                // display error message
                MessageBox.Show("Please enter the value between 0 and 40 only");
                return;
            }

            // assign to payslip class
            ps.HoursWorked = hours;
            ps.Employee = lbEmployee.SelectedItem as Employee;

            // check houts between 0 and 40
            if(hours >0 && hours <=40)
            {
                // calculate gross pay and super
                ps.GrossPay = PayCalculator.CalculateGrossPay(ps.HoursWorked,ps.Employee.HourlyRate);
                //ps.GrossPay = double.Parse($"{ps.GrossPay:N2}");
                ps.Superannuation = PayCalculator.CalculateSuper(ps.GrossPay);
                //ps.Superannuation = double.Parse($"{ps.Superannuation:N2}");
                // calculate tax based on TaxFreeThresold
                if (ps.Employee.TaxFreeThreshold=="Y")
                {
                    ps.Tax = PayCalculatorWithThreshold.CalcualatTax(ps.GrossPay);
                    //ps.Tax = double.Parse($"{ps.Tax:N2}");
                }
                else
                {
                    ps.Tax = PayCalculatorNoThreshold.CalcualatTax(ps.GrossPay);
                    //ps.Tax = double.Parse($"{ps.Tax:N2}");
                }
                ps.NetPay = ps.GrossPay - ps.Tax;
                //ps.NetPay = double.Parse($"{ps.NetPay:N2}");
            }
            else
            {
                // display error message
                MessageBox.Show("Please enter the value between 0 and 40 only");
                return;
            }
            string paySlipText = $"Employee ID: {ps.Employee.EmployeeID}{Environment.NewLine}";
            paySlipText += $"Full Name: {ps.Employee.FullName}{Environment.NewLine}";
            paySlipText += $"Hours Worked: {ps.HoursWorked}{Environment.NewLine}";
            paySlipText += $"Hourly Rate: {ps.Employee.HourlyRate.ToString("c")}{Environment.NewLine}";
            paySlipText += $"Taxfree Threshold: {ps.Employee.TaxFreeThreshold}{Environment.NewLine}";
            paySlipText += $"Gross Pay: {ps.GrossPay.ToString("c")}{Environment.NewLine}";
            paySlipText += $"Super: {ps.Superannuation.ToString("c")}{Environment.NewLine}";
            paySlipText += $"Tax: {ps.Tax.ToString("c")}{Environment.NewLine}";
            paySlipText += $"Net Pay: {ps.NetPay.ToString("c")}{Environment.NewLine}";
            // display payslip in textbox
            tbPaySlip.Text = paySlipText;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Add code below to complete the implementation for saving the
            // calculated payment data into a csv file.
            // File naming convention: Pay_<full name>_<datetimenow>.csv
            // Data fields expected - EmployeeId, Full Name, Hours Worked, Hourly Rate, Tax Threshold, Gross Pay, Tax, Net Pay, Superannuation


            // get stream writer
            StreamWriter strWriter = new StreamWriter($@"..\..\output\Pay-{ps.Employee.EmployeeID}-{ps.Employee.FullName}-{DateTime.Now.ToFileTime()}.csv");
            CsvWriter csvWriter = new CsvWriter(strWriter,culture);
            // create a list of payslip
            List<PaySlip> psList = new List<PaySlip>();
            psList.Add(ps); // add payslip to list
            csvWriter.WriteRecords<PaySlip>(psList);
            MessageBox.Show("Pay Slip csv saved");
            strWriter.Close();
        }
    }
}
