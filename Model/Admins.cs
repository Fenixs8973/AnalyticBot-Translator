using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;

namespace HabrPost.Model
{
    public class Admins
    {
        static public User[] adminList;

        void SetSubscriptions(long chatId, string firstName, string userName, bool is_admin)
        {
            adminList[adminList.Length].chatId = chatId;
            adminList[adminList.Length].firstName = firstName;
            adminList[adminList.Length].userName = userName;
            adminList[adminList.Length].is_admin = is_admin;
        }

        public Admins()
        {
            UpdateSubscriptions();
        }

        public static void UpdateSubscriptions()
        {
            DBRequest db = new DBRequest();
            adminList = db.GetAdmins();
        }
    }
}