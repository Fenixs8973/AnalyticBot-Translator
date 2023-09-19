using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using System.Text.RegularExpressions;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminSubscriptionEditingRemoveYes : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionEditingRemoveYes";

        SubscriptionsArray subscriptions = new SubscriptionsArray();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения названия подписки
            Regex titleSubscription = new Regex(@"^AdminSubscriptionEditingRemoveYes(.*)");
            //Название подписки
            string titleSub = "";

            //Получаем название подписки
            MatchCollection matches = titleSubscription.Matches(update.CallbackQuery.Data);
            if(matches.Count > 0)
            {
                //Выделение Groups
                Match match = titleSubscription.Match(update.CallbackQuery.Data);
                //Кешируем название подписки
                titleSub = match.Groups[1].ToString();
            }
            else
            {
                return;
            }
            
            //Формируем кнопки для ответного сообщения
            InlineKeyboardMarkup inlineKeyboardMarkup = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Управление подписками", callbackData: "AdminSubscriptionManager")
                },
            });
            //Удаляем все записи из user_subscriptions с упоминанием указанной подписки
            await DBRequest.SQLInstructionSet($"DELETE FROM user_subscriptions AS us USING subscriptions AS sub WHERE sub.id = us.subscription_id AND sub.title = '{titleSub}'");
            //Удаляем указанную подписку
            await DBRequest.SQLInstructionSet($"DELETE FROM subscriptions WHERE title = '{titleSub}'");
            await MessageController.ReplaceInlineKeyboardMessageForMarkup($"Подписка {titleSub} успешно удалена!", inlineKeyboardMarkup, update);
        }
    }
}