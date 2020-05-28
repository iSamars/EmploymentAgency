using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentAgency.Classes
{
    class Event
    {
        private string name;
        private string description;
        private DateTime date;

        public Event(string name, string description, DateTime date)
        {
            this.name = name;
            this.description = description;
            this.date = date;
        }

        public string getname()
        {
            return this.name;
        }

        public string getdescription()
        {
            return this.description;
        }

        public DateTime getdate()
        {
            return this.date;
        }

        public void setdate(DateTime date)
        {
           this.date = date;
        }
    }
}
