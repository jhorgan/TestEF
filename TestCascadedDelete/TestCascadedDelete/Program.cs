using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using TestCascadedDelete.Model;

namespace TestCascadedDelete
{
    internal class Program
    {
        private static void Main()
        {
            var id = AddCapacityPlan();
            Console.WriteLine("Inserted Data");
            Console.Read();

            FetchCapacityPlan(id);
            Console.WriteLine("Fetched Data");
            Console.Read();

            DeleteCapacityPlan(id);
            Console.WriteLine("Deleted Data");
            Console.Read();
        }

        private static int AddCapacityPlan()
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Inserting Capacity Plan... ");

            using (var context = new AppDbContext())
            {
                var cp = new CapacityPlan
                {
                    Description = "cp1",
                    CreateDate = DateTime.Now
                };

                CreateProductionUnits(cp, 48);

                context.CapacityPlans.Add(cp);
                context.SaveChanges();

                Console.WriteLine($"Inserted Capacity Plan. Added new id {cp.Id}, Time Taken: {(sw.ElapsedMilliseconds / 1000)} seconds");

                return cp.Id;
            }
        }

        private static void CreateProductionUnits(CapacityPlan cp, int count)
        {
            Console.WriteLine("Inserting ProductionUnits...");
            for (var i = 0; i < count; i++)
            {
                var pu = new ProductionUnit()
                {
                    CapacityPlan = cp,
                    Name = $"Production Unit {i + 1}"
                };
                cp.ProductionUnits.Add(pu);

                CreateProductionUnitModes(pu, 2);
            }

            Console.WriteLine("Inserted ProductionUnits");
        }

        private static void CreateProductionUnitModes(ProductionUnit pu, int count)
        {
            Console.WriteLine("Inserting ProductionUnitModes...");
            for (var i = 0; i < count; i++)
            {
                var pum = new ProductionUnitMode()
                {
                    ProductionUnit = pu,
                    Name = $"ProductionUnit Mode {i + 1}",
                    Description = "ProductionUnit Mode"
                };
                pu.ProductionUnitModes.Add(pum);

                CreateEfficiencyProfileDetails(pum, 2000);
            }
            Console.WriteLine("Inserted ProductionUnitModes");
        }

        private static void CreateEfficiencyProfileDetails(ProductionUnitMode pum, int count)
        {
            Console.WriteLine("Inserting EfficiencyProfileDetails...");
            for (var i = 0; i < count; i++)
            {
                var efd = new EfficiencyProfileDetail()
                {
                    ProductionUnitMode = pum,
                    Efficiency = 12
                };
                pum.EfficiencyProfileDetails.Add(efd);
            }
            Console.WriteLine("Inserted EfficiencyProfileDetails");
        }

        private static void FetchCapacityPlan(int id)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Fetching Capacity Plan... ");

            using (var context = new AppDbContext())
            {
                // context.Database.Log = Console.Write;

                var cp = context.Set<CapacityPlan>()
                    .Where(e => e.Id == id)
                    .Include(e =>
                        e.ProductionUnits.Select(c1 =>
                            c1.ProductionUnitModes.Select(c2 => c2.EfficiencyProfileDetails))).FirstOrDefault();

                var efficiencyProfileDetailsCount = cp?.ProductionUnits.SelectMany(pu => pu.ProductionUnitModes)
                    .Sum(pum => pum.EfficiencyProfileDetails.Count());

                Console.WriteLine($"Fetched id {cp?.Id}, EfficiencyProfileDetails count: {efficiencyProfileDetailsCount} Time Taken: {sw.ElapsedMilliseconds} milliseconds");
            }
        }

        private static void DeleteCapacityPlan(int id)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Deleting Capacity Plan... ");

            using (var context = new AppDbContext())
            {
                // context.Database.Log = Console.Write;

                var cp = new CapacityPlan() { Id = id };
                context.CapacityPlans.Attach(cp);
                context.CapacityPlans.Remove(cp);

                var changes = context.SaveChanges();
                Console.WriteLine($"Removing {id}. Removed {changes} rows. Time Taken: {sw.ElapsedMilliseconds} milliseconds");
            }
        }
    }
}
