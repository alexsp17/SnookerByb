using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Awpbs
{
    public class AgeHelper
    {
        public const int DefaultAge = 20;

        public int CalcAge(DateTime date, DateTime dob)
        {
            if (dob.Year <= 1900)
                return DefaultAge;

            TimeSpan timeSpan = date.Date - dob.Date;

            int age = date.Year - dob.Year;
            if (date.Month < dob.Month || (date.Month == dob.Month && date.Day < dob.Day))
                age -= 1;

            return age;
        }

        public AgeGroupEnum CalcAgeGroup(DateTime? resultDate, DateTime? dob, GenderEnum gender)
        {
            int age = DefaultAge;
            if (dob != null && resultDate != null)
                age = this.CalcAge(resultDate.Value, dob.Value);

            if (gender == GenderEnum.Female)
            {
                if (age < 35)
                    return AgeGroupEnum.FOpen;
                if (age < 40)
                    return AgeGroupEnum.F35;
                if (age < 45)
                    return AgeGroupEnum.F40;
                if (age < 50)
                    return AgeGroupEnum.F45;
                if (age < 55)
                    return AgeGroupEnum.F50;
                if (age < 60)
                    return AgeGroupEnum.F55;
                if (age < 65)
                    return AgeGroupEnum.F60;
                if (age < 70)
                    return AgeGroupEnum.F65;
                if (age < 75)
                    return AgeGroupEnum.F70;
                return AgeGroupEnum.F80;
            }
            else
            {
                if (age < 35)
                    return AgeGroupEnum.MOpen;
                if (age < 40)
                    return AgeGroupEnum.M35;
                if (age < 45)
                    return AgeGroupEnum.M40;
                if (age < 50)
                    return AgeGroupEnum.M45;
                if (age < 55)
                    return AgeGroupEnum.M50;
                if (age < 60)
                    return AgeGroupEnum.M55;
                if (age < 65)
                    return AgeGroupEnum.M60;
                if (age < 70)
                    return AgeGroupEnum.M65;
                if (age < 75)
                    return AgeGroupEnum.M70;
                return AgeGroupEnum.M80;
            }
        }
    }
}
