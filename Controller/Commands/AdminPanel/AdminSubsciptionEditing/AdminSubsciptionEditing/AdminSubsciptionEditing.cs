using System.Text.RegularExpressions;
using HabrPost.LogException;
using HabrPost.Model;
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
            CallbackQuery callBack = update.CallbackQuery;
            string titleSub = "";
            SubList subscriptions = new SubList();
            try
            {
                Match match = titleSubscription.Match(callBack.Data);
                titleSub = match.Groups[1].ToString();
                DBRequest db = new DBRequest();
                subscriptions = db.GetSubscriptionTitle(titleSub);
            }
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
            }         
            MessageController mc = new MessageController();
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
            await mc.ReplaceInlineKeyboardMessageForMarkup($"Изменить параметры подписки \"{titleSub}\"", inlineKeyboard, update);
        }
    }
}