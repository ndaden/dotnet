using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using NUnit.Framework;
using Autofac;

namespace ConsoleApplication1
{
    internal interface IDatabase
    {
        int GetPopulation(string capital);
    }

    /// <summary>
    /// Singleton 
    /// </summary>
    class SingletonDatabase : IDatabase
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

    /// <summary>
    /// Ordinary Database
    /// </summary>
    class OrdinaryDatabase : IDatabase
    {
        private Dictionary<string, int> _capitals;

        public OrdinaryDatabase()
        {
            string filepath = "capitals.txt";
            _capitals = File.ReadAllLines(filepath).Batch(2).ToDictionary(list => list.ElementAt(0).Trim(), list => int.Parse(list.ElementAt(1)));
        }

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

    class ConfigurableFinder
    {
        private IDatabase _database;

        public ConfigurableFinder(IDatabase database)
        {
            this._database = database ?? throw new ArgumentNullException(paramName: nameof(database));               
        }

        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach (var name in names)
            {
                result += _database.GetPopulation(name);
            }

            return result;
        }
    }

    class DummyDatabase : IDatabase
    {
        private Dictionary<string, int> capitals = new Dictionary<string, int> { { "rabat", 1 }, { "casablanca", 2 } };

        public int GetPopulation(string capital)
        {
            return capitals[capital];
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

        [Test]
        public void ConfigurableDatabaseTest()
        {
            var cd = new ConfigurableFinder(new DummyDatabase());

            int result = cd.GetTotalPopulation(new List<string> { "rabat", "casablanca" });

            Assert.AreEqual(3, result);
        }

        [Test]
        public void DIPopulationTest()
        {
            var cb = new ContainerBuilder();
            //whenever somebody asks for IDatabase , they will get OrdinaryDatabase, as a Singleton !
            cb.RegisterType<OrdinaryDatabase>()
                .As<IDatabase>()
                .SingleInstance();

            cb.RegisterType<ConfigurableFinder>();

            using(var c = cb.Build())
            {
                var rf = c.Resolve<ConfigurableFinder>();
                
            }
        }
    }
}
