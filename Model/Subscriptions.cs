using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;

namespace HabrPost.Model
{
    public class Subscriptions
    {
        static public SubList[] subList;

        void SetSubscriptions(int id, string title, string description, int price)
        {
            subList[id].id = id;
            subList[id].title = title;
            subList[id].description = description;
            subList[id].price = price;
        }

        public Subscriptions()
        {
            UpdateSubscriptions();
        }

        public static void UpdateSubscriptions()
        {
            DBRequest db = new DBRequest();
            subList = db.GetSubscriptions();
        }
    }
}