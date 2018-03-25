using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monostate
{
    public class CEO
    {
        private static string name;
        private static int age;

        public string Name {
            get => name;
            set => name = value;
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public override string ToString()
        {
            return $"CEO : {Name} - {Age} yo";
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            var ceo = new CEO()
            {
                Name = "Nabil DADEN",
                Age = 26
            };

            var ceo2 = new CEO();
            bool isSingleton = ceo.Equals(ceo2);
            Debug.Print(ceo.ToString());
            Debug.Print(ceo2.ToString());
        }
    }
}
