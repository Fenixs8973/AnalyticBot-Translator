using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using System.Text.RegularExpressions;
using HabrPost.LogException;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminEditingAdmin : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminEditingAdmin";

        Subscriptions subscriptions = new Subscriptions();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения имени пользователя
            Regex regexAdminUserName = new Regex(@"^AdminEditingAdmin(.*)");
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
                    InlineKeyboardButton.WithCallbackData(text: "Удалить", callbackData: "AdminEditingAdminRemove" + adminUserName)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "AdminManager" + adminUserName)
                }
            });
            Model.Struct.User adminInfo = db.GetUser(adminUserName);
            string msg = $"Имя: {adminInfo.firstName}\nИмя пользователя: {adminInfo.userName}";
            await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
        }
    }
}