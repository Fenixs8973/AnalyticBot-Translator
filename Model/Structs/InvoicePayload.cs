namespace HabrPost.Model.Struct
{
    public struct InvoicePayload
    {
        public long chatId { get; set; }
        public UInt32 invoicePayloadId { get; set; }
        public string subscriptionTitle { get; set; }
    }
}