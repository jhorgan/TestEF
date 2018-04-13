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
            Console.ReadKey();

            FetchCapacityPlan(id);
            Console.WriteLine("Fetched Data");
            Console.ReadKey();

            DeleteCapacityPlan(id);
            Console.WriteLine("Deleted Data");
            Console.ReadKey();
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

        private static CapacityPlan FetchCapacityPlan(int id)
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

                return cp;
            }
        }

        private static void DeleteCapacityPlan(int capacityPlanId)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Deleting Capacity Plan... ");

            using (var context = new AppDbContext())
            {
                // context.Database.Log = Console.Write;

                // Delete the EfficiencyProfileDetails
                var q1 = from ef in context.EfficiencyProfileDetails
                         join pum in context.ProductionUnitModes on ef.ProductionUnitModeId equals pum.Id
                         join pu in context.ProductionUnits on pum.ProductionUnitId equals pu.Id
                         where pu.CapacityPlanId == capacityPlanId
                         select ef;

                context.EfficiencyProfileDetails.RemoveRange(q1);

                // Delete the ProductionUnitModes
                var q2 = from pum in context.ProductionUnitModes
                         join pu in context.ProductionUnits on pum.ProductionUnitId equals pu.Id
                         where pu.CapacityPlanId == capacityPlanId
                         select pum;

                context.ProductionUnitModes.RemoveRange(q2);

                // Delete the ProductionUnits
                var q3 = from pu in context.ProductionUnits
                    where pu.CapacityPlanId == capacityPlanId
                    select pu;

                context.ProductionUnits.RemoveRange(q3);

                // Delete the ProductionUnits
                var q4 = from cp in context.CapacityPlans
                    where cp.Id == capacityPlanId
                    select cp;

                context.CapacityPlans.RemoveRange(q4);

                var changes = context.SaveChanges();
                Console.WriteLine($"Removing {capacityPlanId}. Removed {changes} rows. Time Taken: {sw.ElapsedMilliseconds} milliseconds");
            }
        }
    }
}
