namespace HabrPost.Model.Struct
{
    public struct User
    {
        //Id чата
        public long chatId { get; set; }
        //Имя человека
        public string firstName { get; set; }
        //Имя пользователя
        public string userName { get; set; }
        //Имеет ли права администратора
        public bool is_admin { get; set; }

        public void SetUser(long chatId, string firstName, string userName, bool is_admin)
        {
            this.chatId = chatId;
            this.firstName = firstName;
            this.userName = userName;
            this.is_admin = is_admin;
        }
    }
}