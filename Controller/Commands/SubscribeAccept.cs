using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;
using HabrPost.Controllers.Messages;

namespace HabrPost.Controllers.Commands.Admin
{
    class SubscribeAccept : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "SubscribeAccept";

        public async Task Execute(Update update)
        {
            //Сюда будем записывать данные о подписке
            Subscription subscription = new Subscription();
            //Регулярка для выделения названия подписки
            Regex titleSubscription = new Regex("^SubscribeAccept(.*)");
            //Сюда будем сохранять название подписки
            string titleSub = "";
            //Имеет ли пользователь подписку
            bool hasSubscriptrion = false;
            bool hasUser = false;

            MatchCollection matches = titleSubscription.Matches(update.CallbackQuery.Data);
            if(matches.Count > 0)
            {
                Match match = titleSubscription.Match(update.CallbackQuery.Data);
                //Получаем имя подписки и callback'a
                titleSub = match.Groups[1].ToString();
                //Получаем данные подписки по названию
                subscription = await DBRequest.GetSubscriptionFromTitle(titleSub);
                //Проверяем имеется ли у пользователя эта подписка
                hasSubscriptrion = await DBRequest.HasUserSubscription(update.CallbackQuery.Message.Chat.Id, subscription.id);
                if(!hasUser)
                {
                    await DBRequest.SQLInstructionSet($"INSERT INTO users (tg_id) VALUES ('{update.CallbackQuery.Message.Chat.Id}')");
                    await DBRequest.SQLInstructionSet($"INSERT INTO user_subscriptions VALUES ('{update.CallbackQuery.Message.Chat.Id}', '{subscription.id}')");
                }
                
                if(!hasSubscriptrion)
                {
                    await DBRequest.SQLInstructionSet($"INSERT INTO user_subscriptions VALUES ('{update.CallbackQuery.Message.Chat.Id}', '{subscription.id}')");
                    await MessageController.ReplaceInlineKeyboardMessageForMarkup($"Подписка успешно оформлена", MessageController.Start, update);
                }
                else
                {
                    await MessageController.ReplaceInlineKeyboardMessageForMarkup($"Подписка уже оформлена", MessageController.Start, update);
                }
            }
        }
    }
}