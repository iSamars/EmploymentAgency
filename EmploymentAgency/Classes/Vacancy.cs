using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmploymentAgency.Classes
{
    class Vacancy
    {
        private int id;
        private ImageSource image;
        private string title;
        private string body;
        private string address;
        private string salary;
        private string mode;
        private string education;

        public Vacancy(int id, ImageSource image, string title, string body, string address, string salary, string mode, string education)
        {
            this.id = id;
            this.image = image;
            this.title = title;
            this.body = body;
            this.address = address;
            this.salary = salary;
            this.mode = mode;
            this.education = education;
        }

        public int getid()
        {
            return this.id;
        }

        public ImageSource getimage()
        {
            return this.image;
        }

        public string gettitle()
        {
            return this.title;
        }

        public string getbody()
        {
            return this.body;
        }

        public string getaddress()
        {
            return this.address;
        }

        public string getsalary()
        {
            return this.salary;
        }

        public string getmode()
        {
            return this.mode;
        }

        public string geteducation()
        {
            return this.education;
        }
    }
}
