using System.Collections.Generic;

namespace TestCascadedDelete.Model
{
    internal class ProductionUnitMode
    {
        public ProductionUnitMode()
        {
            EfficiencyProfileDetails = new List<EfficiencyProfileDetail>();
        }

        public int Id { get; set; }
        public int ProductionUnitId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ProductionUnit ProductionUnit { get; set; }
        public ICollection<EfficiencyProfileDetail> EfficiencyProfileDetails { get; set; }

    }
}
