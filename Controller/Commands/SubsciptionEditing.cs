using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;

namespace HabrPost.Controllers.Commands.Admin
{
    class SubsciptionEditing : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "SubscriptionEditing";

        public async Task Execute(Update update)
        {
            Regex titleSubscription = new Regex(@"^SubscriptionEditing(.*)");
            string titleSub = "";
            Subscription subscriptions = new Subscription();
            bool hasSubscriptrion = false;
            //Пытаемся найти имя подписки
            MatchCollection matches = titleSubscription.Matches(update.CallbackQuery.Data);
            if(matches.Count > 0)
            {
                Match match = titleSubscription.Match(update.CallbackQuery.Data);
                titleSub = match.Groups[1].ToString();
                subscriptions = await DBRequest.GetSubscriptionFromTitle(titleSub);
            }
            else
            {
                return;
            }

            InlineKeyboardMarkup SubscribeAccept = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Подписаться", callbackData: $"SubscribeInvoke{titleSub}"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "SubscriptionManager"),
                }
            });
            InlineKeyboardMarkup SubscribeCancel = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Отписаться", callbackData: $"SubscribeCancel{titleSub}")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "SubscriptionManager"),
                }
            });

            hasSubscriptrion = await DBRequest.HasUserSubscription(CommandExecutor.GetChatId(update), subscriptions.id);
            
            if(hasSubscriptrion)
                await MessageController.ReplaceInlineKeyboardMessageForMarkup($"\"{titleSub}\" - Активна\n{subscriptions.description}\n{subscriptions.price}р.", SubscribeCancel, update);
            else
                await MessageController.ReplaceInlineKeyboardMessageForMarkup($"\"{titleSub}\" - Не активна\n{subscriptions.description}\n{subscriptions.price}р.", SubscribeAccept, update);
        }
    }
}