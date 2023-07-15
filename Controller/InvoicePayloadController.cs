using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;
using Telegram.Bot.Types;

namespace HabrPost.Controllers
{
    public class InvoicePayloadController
    {
        private static UInt32 InvoicePayloadID;


        ///<summary>
        ///Возвращает последний id InviocePayload
        ///</summary>
        public static UInt32 GetInvoicePayloadID()
        {
            return InvoicePayloadID;
        }

        ///<summary>
        ///Добаление новой записи о платеже в БД
        ///</summary>
        public static async Task SetNewInviocePayload(long chatId, string subTitle)
        {
            await DBRequest.SQLInstructionSet($"INSERT INTO invoice_payload VALUES ('{chatId}', '{subTitle}')");
            InvoicePayloadID++;
        }

        public async Task SuccessfulPayment(Update update)
        {
            //Получаем Id invoicePayload
            UInt32 invoicePayloadID = Convert.ToUInt32(update.Message.SuccessfulPayment.InvoicePayload);
            
            InvoicePayload thisPayLoad = await DBRequest.GetInfoInviocePayload(invoicePayloadID);
            //Получаем данные подписки
            Subscription subscription = await DBRequest.GetSubscriptionFromTitle(thisPayLoad.subscriptionTitle);
            //Добавляем пользователю новую подписку
            await DBRequest.SQLInstructionSet($"INSERT INTO user_subscriptions VALUES ('{update.Message.Chat.Id}', '{subscription.id}')");
            await DBRequest.SQLInstructionSet($"UPDATE invoice_payload SET payment_completed = TRUE WHERE invoice_payload_id = {invoicePayloadID}");
        }
    }
}