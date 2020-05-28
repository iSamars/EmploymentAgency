using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentAgency.Classes
{
    public class User
    {
        private int id;
        private string login;
		private string unhashedEmail;
        private string pass;
        private bool admin;
        private string name;
        private string lastname;
        private string patronymic;
        private int sex;
        private DateTime dateOfBirth;
        private string country;

		public User(int id, string login, string unhashedEmail, string pass, bool admin, string name, string lastname, string patronymic, int sex, DateTime dateOfBirth, string country)
		{
			this.id = id;
			this.login = login;
			this.unhashedEmail = unhashedEmail;
			this.pass = pass;
			this.admin = admin;
			this.name = name;
			this.lastname = lastname;
			this.patronymic = patronymic;
			this.sex = sex;
			this.dateOfBirth = dateOfBirth;
			this.country = country;
		}

		public int getid()
		{
			return this.id;
		}

		public string getlogin()
		{
			return this.login;
		}

		public string getunhashedEmail()
		{
			return this.unhashedEmail;
		}

		public string getpass()
		{
			return this.pass;
		}

		public bool getadmin()
		{
			return this.admin;
		}

		public string getname()
		{
			return this.name;
		}

		public string getlastname()
		{
			return this.lastname;
		}

		public string getpatronymic()
		{
			return this.patronymic;
		}

		public int getsex()
		{
			return this.sex;
		}

		public DateTime getdateOfBirth()
		{
			return this.dateOfBirth;
		}

		public string getcountry()
		{
			return this.country;
		}

		public void setid(int id)
		{
			this.id = id;
		}

		public void setlogin(string login)
		{
			this.login = login;
		}

		public void setunhashedEmail(string unhashedEmail)
		{
			this.unhashedEmail = unhashedEmail;
		}

		public void setpass(string pass)
		{
			this.pass = pass;
		}

		public void setadmin(bool admin)
		{
			this.admin = admin;
		}

		public void setname(string name)
		{
			this.name = name;
		}

		public void setlastname(string lastname)
		{
			this.lastname = lastname;
		}

		public void setpatronymic(string patronymic)
		{
			this.patronymic = patronymic;
		}

		public void setsex(int sex)
		{
			this.sex = sex;
		}

		public void setdateOfBirth(DateTime dateOfBirth)
		{
			this.dateOfBirth = dateOfBirth;
		}

		public void setcountry(string country)
		{
			this.country = country;
		}
	}
}
