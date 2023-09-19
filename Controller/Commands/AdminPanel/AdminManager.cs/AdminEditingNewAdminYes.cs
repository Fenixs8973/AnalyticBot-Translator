using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using System.Text.RegularExpressions;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminEditingNewAdminYes : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminEditingNewAdminYes";

        SubscriptionsArray subscriptions = new SubscriptionsArray();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения имени пользователя
            Regex regexAdminUserName = new Regex(@"^AdminEditingNewAdminYes(.*)");
            //Имя пользователя
            string adminUserName = "";

            MatchCollection matches = regexAdminUserName.Matches(update.CallbackQuery.Data);
            //Пробуем получить имя пользователя
            if(matches.Count > 0)
            {
                //Выделение имени пользователя
                Match match = regexAdminUserName.Match(update.CallbackQuery.Data);
                //Кешируем имя пользователя
                adminUserName = match.Groups[1].ToString();
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
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                },
            });
            //Меняем is_admin в БД на TRUE для определенного пользователя
            await DBRequest.SQLInstructionSet($"UPDATE users SET is_admin = 'TRUE' WHERE username = '{adminUserName}'");
            //Отправяляем подтверждающее сообщение
            await MessageController.ReplaceInlineKeyboardMessageForMarkup($"{adminUserName} успешно добавлен в список администраторов!", inlineKeyboardMarkup, update);
        }
    }
}