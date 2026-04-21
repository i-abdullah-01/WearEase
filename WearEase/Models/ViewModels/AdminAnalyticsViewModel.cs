namespace WearEase.Models.ViewModels
{
    public class AdminAnalyticsViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ApprovedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
    }
}
