using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using System.Text.RegularExpressions;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminEditingAdmin : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminEditingAdmin";

        SubscriptionsArray subscriptions = new SubscriptionsArray();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения имени пользователя
            Regex regexAdminUserName = new Regex(@"^AdminEditingAdmin(.*)");
            //Имя пользователя админа 
            string adminUserName = "";

            MatchCollection matches = regexAdminUserName.Matches(update.CallbackQuery.Data);
            //Пробуем получить имя пользователя
            if(matches.Count > 0)
            {
                //Выделение имени пользователя
                Match match = regexAdminUserName.Match(update.CallbackQuery.Data);
                //Кешируем название подписки
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
                    InlineKeyboardButton.WithCallbackData(text: "Удалить", callbackData: "AdminEditingAdminRemove" + adminUserName)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "AdminManager" + adminUserName)
                }
            });
            Model.Struct.User adminInfo = await DBRequest.GetUser(adminUserName);
            await MessageController.ReplaceInlineKeyboardMessageForMarkup($"Имя: {adminInfo.firstName}\nИмя пользователя: {adminInfo.userName}", inlineKeyboardMarkup, update);
        }
    }
}