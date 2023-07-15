using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using System.Text.RegularExpressions;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminEditingAdminRemove : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminEditingAdminRemove";

        SubscriptionsArray subscriptions = new SubscriptionsArray();

        public async Task Execute(Update update)
        {
            //Регулярка для выделения имени пользователя
            Regex regexAdminUserName = new Regex(@"^AdminEditingAdminRemove(.*)");
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
                    InlineKeyboardButton.WithCallbackData(text: "Да, удалить", callbackData: "AdminEditingAdminRemoveYes" + adminUserName)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "НЕТ, НЕ удалять", callbackData: "AdminManager" + adminUserName)
                }
            });
            Model.Struct.User adminInfo = await DBRequest.GetUser(adminUserName);
            await MessageController.ReplaceInlineKeyboardMessageForMarkup($"Вы уверены, что хотите удалить {adminInfo.userName}?", inlineKeyboardMarkup, update);
        }
    }
}