namespace Devq.Bids.Services {
    public class CreateBidContext {
        public virtual string Author { get; set; }
        public virtual string BidPrice { get; set; }
        public virtual int BidedOn { get; set; }
    }
}