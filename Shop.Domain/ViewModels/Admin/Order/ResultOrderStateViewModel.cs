namespace Shop.Domain.ViewModels.Admin.Order
{
    public class ResultOrderStateViewModel
    {
        public int RequestCount { get; set; }
        public int ProcessingCount { get; set; }
        public int SentCount { get; set; }
        public int CancelCount { get; set; }
    }
}
