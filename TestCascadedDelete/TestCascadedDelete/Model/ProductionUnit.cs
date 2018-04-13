using System.Collections.Generic;

namespace TestCascadedDelete.Model
{
    internal class ProductionUnit
    {
        public ProductionUnit()
        {
            ProductionUnitModes = new List<ProductionUnitMode>();
        }

        public int Id { get; set; }
        public int CapacityPlanId { get; set; }
        public string Name { get; set; }

        public CapacityPlan CapacityPlan { get; set; }

        public ICollection<ProductionUnitMode> ProductionUnitModes { get; set; }
    }
}
