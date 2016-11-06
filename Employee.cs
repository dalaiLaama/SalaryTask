using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{

    public class Employee
    {
        public string personName { get; set; }
        public int personId { get; set; }
        public double hourlyWage = 3.75;
        public double eveningCompensation = 1.15;
        public double workHours = 0;
        public double monthlyWage = 0;
        public double overtime = 0;
        public double overtimeComp = 0;
        public double eveningTime = 0;
        public double eveningComp = 0;
        public double totalWage = 0;



        public Employee(string name, int id)
        {
            this.personName = name;
            this.personId = id;
        }
        // Lisätään tunnit ylös
        public void addHours(double startTime, double endTime)
        {
                // tsekataan jos työntekijä on jatkanut seuraavalle vuorokaudelle.
            if (endTime < startTime)
            {
                double workTime = (24 - startTime + endTime);
                this.workHours = this.workHours + workTime;
                checkTime(workTime, startTime, endTime);

            }
            else if (endTime > startTime)
            {
                double workTime = (endTime - startTime);
                this.workHours = this.workHours + workTime;
                checkTime(workTime, startTime, endTime);
            }
        }

        // tarkistetaan työtunnit
        public void checkTime(double worktime, double startTime, double endTime)
        {
            double compensation;
            if (worktime <= 8)
            {
                // no overtime compensations
                checkEvening(worktime, startTime, endTime);
                calculateWage(worktime);

            }
            else if (worktime > 8 && worktime <= 10)
            {
                // hourly wage * 1,25
                compensation = (worktime - 8) * hourlyWage * 0.25;
                this.overtime = this.overtime + (worktime - 8);
                this.overtimeComp = this.overtimeComp + compensation;
                this.overtimeComp = Math.Round(this.overtimeComp, 2);
                calculateWage(worktime);
            }
            else if (worktime > 10 && worktime <= 12)
            {
                // hourly wage * 1,50
                compensation = (worktime - 10) * hourlyWage * 0.50 + (2 * hourlyWage * 0.25);
                this.overtime = this.overtime + (worktime - 8);
                this.overtimeComp = this.overtimeComp + compensation;
                this.overtimeComp = Math.Round(this.overtimeComp, 2);
                calculateWage(worktime);

            }
            else if (worktime > 12)
            {
                // hourly wage * 2
                compensation = (worktime - 12) * hourlyWage + (2 * hourlyWage * 0.5) +(2 * hourlyWage * 0.25);
                this.overtime = this.overtime + (worktime - 8);
                this.overtimeComp = this.overtimeComp + compensation;
                this.overtimeComp = Math.Round(this.overtimeComp, 2);
                calculateWage(worktime);

            }
        }
        // laskee peruspalkan
        public void calculateWage(double worktime)
        {
            double dailyWage = worktime * hourlyWage;
            this.monthlyWage = this.monthlyWage + dailyWage;
            this.monthlyWage = Math.Round(this.monthlyWage, 2);
          
        }
        // laskee kokonaispalkan
        public void calculateTotal()
        {
            this.totalWage = this.totalWage + this.monthlyWage + this.overtimeComp + this.eveningComp;
            this.totalWage = Math.Round(this.totalWage, 2);
        }
        // laskee iltalisät
        public void checkEvening(double worktime, double startTime, double endTime)
        {
            double compensation;
            if (startTime > 6 && endTime < 18)
            {
                // Ei kompensaatiota
            }
            else if ((startTime + worktime) > 18 && (startTime + worktime) < 30)
            {
                compensation = (startTime + worktime) - 18;
                this.eveningComp = this.eveningComp + compensation*eveningCompensation;
                this.eveningTime = this.eveningTime + compensation;
            }
            else if (startTime < 6)
            {
                compensation = (6 + worktime) - (startTime + worktime);
                this.eveningComp = this.eveningComp + compensation * eveningCompensation;
                this.eveningTime = this.eveningTime + compensation;
            }
            else if (startTime > 18 && endTime < 8)
            {
                if (endTime > 6)
                {
                    compensation = worktime - (endTime - 6);
                    this.eveningComp = this.eveningComp + compensation * eveningCompensation;
                    this.eveningTime = this.eveningTime + compensation;
                }
                else {
                    compensation = (startTime + worktime - 18);
                  this.eveningComp = this.eveningComp + compensation*eveningCompensation;
                    this.eveningTime = this.eveningTime + compensation;

                }
            }
        }





    }
}



// Total Daily Pay = Regular Daily Wage + Evening Work Compensation + Overtime Compensations
// Regular Daily Wage = Regular Working Hours * Hourly Wage
// Evening work compensation is +$1.15/hour
// Evening Work Compensation = Evening Hours * Evening Work Compensation
// Overtime Compensations[] = Overtime Hours * Overtime Compensation Percent * Hourly Wage+

//First 2 Hours > 8 Hours = Hourly Wage + 25%
//Next 2 Hours = Hourly Wage + 50%
//After That = Hourly Wage + 100%