namespace TestCascadedDelete.Model
{
    internal class EfficiencyProfileDetail
    {
        public int Id { get; set; }
        public int ProductionUnitModeId { get; set; }
        public double Efficiency { get; set; }

        public ProductionUnitMode ProductionUnitMode { get; set; }
    }
}
