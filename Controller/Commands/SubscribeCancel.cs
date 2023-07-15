using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;
using HabrPost.Controllers.Messages;

namespace HabrPost.Controllers.Commands.Admin
{
    class SubscribeCancel : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "SubscribeCancel";

        public async Task Execute(Update update)
        {
            //Сюда будем записывать данные о подписке
            Subscription subscription = new Subscription();
            //Регулярка для выделения названия подписки
            Regex titleSubscription = new Regex("^SubscribeCancel(.*)");
            //Сюда будем сохранять название подписки
            string titleSub = "";
            //Имеет ли пользователь подписку
            bool hasSubscriptrion = false;
            bool hasUser = false;
            //Пытаемся найти имя подписки
            Match match = titleSubscription.Match(update.CallbackQuery.Data);
            //Получаем имя подписки и callback'a
            titleSub = match.Groups[1].ToString();
            //Получаем данные подписки по названию
            subscription = await DBRequest.GetSubscriptionFromTitle(titleSub);
            //Проверяем имеет ся ли у пользователя эта подписка
            hasSubscriptrion = await DBRequest.HasUserSubscription(update.CallbackQuery.Message.Chat.Id, subscription.id);
            hasUser = await DBRequest.HasUserInDB(update.CallbackQuery.Message.Chat.Id);
            if(!hasUser)
            {
                await DBRequest.SQLInstructionSet($"INSERT INTO users (tg_id) VALUES ('{update.CallbackQuery.Message.Chat.Id}')");
                await MessageController.ReplaceInlineKeyboardMessageForMarkup($"У вас нету подписки {titleSub}", MessageController.Start, update);
                return;
            }
            
            if(!hasSubscriptrion)
            {
                await MessageController.ReplaceInlineKeyboardMessageForMarkup($"У вас нету подписки {titleSub}", MessageController.Start, update);
            }
            else
            {
                await DBRequest.SQLInstructionSet($"DELETE FROM user_subscriptions WHERE tg_id='{update.CallbackQuery.Message.Chat.Id}' AND subscription_id = '{subscription.id}'");
                await MessageController.ReplaceInlineKeyboardMessageForMarkup($"Вы успешно отменили подписку {titleSub}", MessageController.Start, update);
            }        
        }
    }
}