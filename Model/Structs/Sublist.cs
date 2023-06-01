namespace HabrPost.Model.Struct
{
    public struct SubList
    {
        //id подписки в бд
        public int id { get; set; }
        public string title { get; set; }
        //Описание подписки
        public string description { get; set; }
        public int price { get; set; }

        public void SetSubscription(int id, string title, string description, int price)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.price = price;
        }
    }
}