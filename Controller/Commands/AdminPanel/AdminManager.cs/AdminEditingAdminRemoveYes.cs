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
    class AdminEditingAdminRemoveYes : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminEditingAdminRemoveYes";

        Subscriptions subscriptions = new Subscriptions();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения имени пользователя
            Regex regexAdminUserName = new Regex(@"^AdminEditingAdminRemoveYes(.*)");
            CallbackQuery callBack = update.CallbackQuery;
            //Имя пользователя админа 
            string adminUserName = "";
            //Ответное сообщение
            string msg;
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
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                }
            });

            try
            {
                //Запрос на изменение прав
                db.SQLInstructionSet($"UPDATE users SET is_admin = 'FALSE' WHERE username = '{adminUserName}'");
                msg = $"{adminUserName} успешно добавлен в список администраторов!";
                await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
            }
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
                msg = "Произошла ошибка";
                await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
            }
        }
    }
}