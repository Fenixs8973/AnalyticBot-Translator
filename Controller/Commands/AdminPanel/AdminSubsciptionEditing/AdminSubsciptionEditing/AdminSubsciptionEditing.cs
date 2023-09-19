using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminSubscriptionEditing : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionEditing";

        public async Task Execute(Update update)
        {
            Regex titleSubscription = new Regex(@"^AdminSubscriptionEditing(.*)");
            string titleSub = "";
            Subscription subscriptions = new Subscription();

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
                    
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Название", callbackData: "AdminSubscriptionEditingTitle" + titleSub),
                    InlineKeyboardButton.WithCallbackData(text: "Описание", callbackData: "AdminSubscriptionEditingDescription" + titleSub)
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Цена", callbackData: "AdminSubscriptionEditingPrice" + titleSub),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Удалить подписку", callbackData: "AdminSubscriptionEditingRemove" + titleSub),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "AdminSubscriptionManager"),
                }
            });
            await MessageController.ReplaceInlineKeyboardMessageForMarkup($"Изменить параметры подписки \"{titleSub}\"", inlineKeyboard, update);
        }
    }
}