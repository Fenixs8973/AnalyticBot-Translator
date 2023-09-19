using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;

namespace HabrPost.Model
{
    public class SubscriptionsArray
    {
        static public Subscription[] subArray;

        void SetSubscriptions(int id, string title, string description, int price)
        {
            subArray[id].id = id;
            subArray[id].title = title;
            subArray[id].description = description;
            subArray[id].price = price;
        }

        public SubscriptionsArray()
        {
            UpdateSubscriptions();
        }

        public static async Task UpdateSubscriptions()
        {
            subArray = await DBRequest.GetSubscriptions();
        }
    }
}