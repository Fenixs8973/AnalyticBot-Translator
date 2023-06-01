using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using HabrPost.Model.Struct;
using System.Text.RegularExpressions;
using HabrPost.LogException;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminSubscriptionEditingRemoveYes : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionEditingRemoveYes";

        Subscriptions subscriptions = new Subscriptions();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения названия подписки
            Regex titleSubscription = new Regex(@"^AdminSubscriptionEditingRemoveYes(.*)");
            CallbackQuery callBack = update.CallbackQuery;
            //Текст для ответного сообщения
            string msg;
            //Название подписки
            string titleSub = "";
            //Пробуем получить название подписки
            try
            {
                //Выделение Groups
                Match match = titleSubscription.Match(callBack.Data);
                //Кешируем название подписки
                titleSub = match.Groups[1].ToString();
            }
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
            } 
            DBRequest db = new DBRequest();
            MessageController mc = new MessageController();
            //Формируем кнопки для ответного сообщения
            InlineKeyboardMarkup inlineKeyboardMarkup = new(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Управление подписками", callbackData: "AdminSubscriptionManager")
                    },
                });
            try
            {
                //Удаляем все записи из user_subscriptions с упоминанием указанной подписки
                db.SQLInstructionSet($"DELETE FROM user_subscriptions AS us USING subscriptions AS sub WHERE sub.id = us.subscription_id AND sub.title = '{titleSub}'");
                //Удаляем указанную подписку
                db.SQLInstructionSet($"DELETE FROM subscriptions WHERE title = '{titleSub}'");
                msg = $"Подписка {titleSub} успешно удалена!";
                await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
            }
            catch(Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
                msg = $"Не удалось удалить {titleSub}";
                await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
            }
        }
    }
}