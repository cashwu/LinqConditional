using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace testLinqConditional
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new MyDbContext();
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
            ctx.Persons.AddRange(GetData());
            ctx.SaveChanges();

            Console.WriteLine("-------------------");
            var persons = ctx.Persons.AsQueryable();

            persons = persons.Where(a => a.Id > 2);

            persons = persons.If(true,
                                 a => a.Where(b => b.Id < 10),
                                 b => b.Where(c => c.Id > 1));

            persons = persons.If(false, a => a.Where(b => b.Id < 8));

            Console.WriteLine(JsonSerializer.Serialize(persons.ToList()));

            Console.ReadKey();
        }

        static IEnumerable<Person> GetData()
        {
            return Enumerable.Range(1, 20).Select(a => new Person
            {
                Id = a,
                Name = a + " " + Guid.NewGuid()
            });
        }
    }

    public static class LinqExtensions
    {
        public static IQueryable<T> If<T>(this IQueryable<T> query,
                                          bool should,
                                          params Func<IQueryable<T>, IQueryable<T>>[] transforms)
        {
            return should
                       ? transforms.Aggregate(query,
                                              (current, transform) => transform.Invoke(current))
                       : query;
        }

        public static IEnumerable<T> If<T>(this IEnumerable<T> query,
                                           bool should,
                                           params Func<IEnumerable<T>, IEnumerable<T>>[] transforms)
        {
            return should
                       ? transforms.Aggregate(query,
                                              (current, transform) => transform.Invoke(current))
                       : query;
        }
    }
}