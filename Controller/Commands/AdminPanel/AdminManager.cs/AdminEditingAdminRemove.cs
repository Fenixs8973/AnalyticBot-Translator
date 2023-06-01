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
    class AdminEditingAdminRemove : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminEditingAdminRemove";

        Subscriptions subscriptions = new Subscriptions();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения имени пользователя
            Regex regexAdminUserName = new Regex(@"^AdminEditingAdminRemove(.*)");
            CallbackQuery callBack = update.CallbackQuery;
            //Имя пользователя админа 
            string adminUserName = "";
            //Пробуем получить имя пользователя
            try
            {
                //Выделение имени пользователя
                Match match = regexAdminUserName.Match(callBack.Data);
                //Кешируем название подписки
                adminUserName = match.Groups[1].ToString();
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
                    InlineKeyboardButton.WithCallbackData(text: "Да, удалить", callbackData: "AdminEditingAdminRemoveYes" + adminUserName)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "НЕТ, НЕ удалять", callbackData: "AdminManager" + adminUserName)
                }
            });
            Model.Struct.User adminInfo = db.GetUser(adminUserName);
            string msg = $"Вы уверены, что хотите удалить {adminInfo.userName}?";
            await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
        }
    }
}