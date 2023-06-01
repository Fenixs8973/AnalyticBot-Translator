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
    class AdminEditingNewAdminYes : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminEditingNewAdminYes";

        Subscriptions subscriptions = new Subscriptions();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения имени пользователя
            Regex regexAdminUserName = new Regex(@"^AdminEditingNewAdminYes(.*)");
            CallbackQuery callBack = update.CallbackQuery;
            //Текст для ответного сообщения
            string msg;
            //Имя пользователя
            string adminUserName = "";
            //Пробуем получить имя пользователя
            try
            {
                //Выделение имени пользователя
                Match match = regexAdminUserName.Match(callBack.Data);
                //Кешируем имя пользователя
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
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                },
            });
            try
            {
                //Меняем is_admin в БД на TRUE для определенного пользователя
                db.SQLInstructionSet($"UPDATE users SET is_admin = 'TRUE' WHERE username = '{adminUserName}'");
                msg = $"{adminUserName} успешно добавлен в список администраторов!";
                //Отправяляем подтверждающее сообщение
                await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
            }
            catch(Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
                msg = $"Произошла ошибка!";
                await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
            }
        }
    }
}