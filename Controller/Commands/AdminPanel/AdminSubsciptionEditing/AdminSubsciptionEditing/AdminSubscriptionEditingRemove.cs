using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using System.Text.RegularExpressions;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminSubscriptionEditingRemove : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionEditingRemove";

        SubscriptionsArray subscriptions = new SubscriptionsArray();

        public async Task Execute(Update update)
        {
            Regex titleSubscription = new Regex(@"^AdminSubscriptionEditingRemove(.*)");
            CallbackQuery callBack = update.CallbackQuery;
            string titleSub = "";
            MatchCollection matches = titleSubscription.Matches(callBack.Data);
            if(matches.Count > 0)
            {
                Match match = titleSubscription.Match(callBack.Data);
                titleSub = match.Groups[1].ToString();
            }
            else
            {
                return;
            }
            
            InlineKeyboardMarkup inlineKeyboardMarkup = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Да, удалить", callbackData: "AdminSubscriptionEditingRemoveYes" + titleSub)
                },
                // second row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "НЕТ, НЕ удалить", callbackData: $"AdminSubscriptionEditing{titleSub}")
                }
            });
            await MessageController.ReplaceInlineKeyboardMessageForMarkup($"Вы уверены, что хотите удалить подписку {titleSub}?", inlineKeyboardMarkup, update);
        }
    }
}