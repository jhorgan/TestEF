using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace TestCascadedDelete.Model
{
    internal class CapacityPlan
    {
        public CapacityPlan()
        {
            ProductionUnits = new List<ProductionUnit>();    
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }

        public ICollection<ProductionUnit> ProductionUnits { get; set; }
    }
}
