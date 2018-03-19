using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using NUnit.Framework;

namespace ConsoleApplication1
{
    class SingletonDatabase
    {
        private Dictionary<string, int> _capitals;
        public static int InstanceCount;

        private SingletonDatabase()
        {
            InstanceCount++;
            string filepath = "capitals.txt";
            _capitals = File.ReadAllLines(filepath).Batch(2).ToDictionary(list => list.ElementAt(0).Trim(), list => int.Parse(list.ElementAt(1)));
        }

        private static Lazy<SingletonDatabase> _instance = new Lazy<SingletonDatabase>(() => new SingletonDatabase());

        public static SingletonDatabase Instance => _instance.Value;

        public int GetPopulation(string capital)
        {
            return _capitals[capital];
        }
    }

    class SingletonFinder
    {
        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach(var name in names)
            {
                result += SingletonDatabase.Instance.GetPopulation(name);
            }

            return result;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
           foreach(string s in args)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("Population of rabat: " + SingletonDatabase.Instance.GetPopulation("Rabat"));
            Console.WriteLine(SingletonDatabase.InstanceCount);
            Console.WriteLine("Population of rabat: " + SingletonDatabase.Instance.GetPopulation("Washington"));
            Console.WriteLine(SingletonDatabase.InstanceCount);
            Console.WriteLine("Population of rabat: " + SingletonDatabase.Instance.GetPopulation("New York"));
            Console.WriteLine(SingletonDatabase.InstanceCount);
            Console.Read();
        }
    }

    [TestFixture]
    public class SingletonTests
    {
        [Test]
        public void IsSingletonTest()
        {
            var db = SingletonDatabase.Instance;
            var db2 = SingletonDatabase.Instance;

            Assert.AreSame(db, db2);
            Assert.AreEqual(1, SingletonDatabase.InstanceCount);
        }

        [Test]
        public void TotalPopulationTest()
        {
            IEnumerable<string> names = new List<string> { "Rabat", "Washington" };
            int expectedTotal = 1032000;

            int result = new SingletonFinder().GetTotalPopulation(names);

            Assert.AreEqual(expectedTotal, result);
        }
    }
}
