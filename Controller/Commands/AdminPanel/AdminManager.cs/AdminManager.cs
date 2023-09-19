using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using HabrPost.Controllers.DB;
using HabrPost.Model;
using Telegram.Bot.Types.ReplyMarkups;

namespace HabrPost.Controllers.Commands
{
    public class AdminManager : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminManager";

        HabrPost.Model.Struct.User[] adminList;

        public async Task Execute(Update update)
        {
            DBRequest db = new DBRequest();
            MessageController mc = new MessageController();
            //Обновляем список администраторов
            await Admins.UpdateSubscriptions();
            //Получаем список администраторов
            adminList = Admins.adminList;
            //Текст для сообщения
            string msg = $"Список администраторов:\n\n";
            //Формируем кнопки для всех администраторов
            InlineKeyboardMarkup inlineKeyboard = await MessageController.GetInlineKeyboardForAdmins();

            //Формируем текст сообщения
            if(adminList.Length == 0)
            {
                msg += "Администраторов нету";
            }
            else
            {
                foreach(HabrPost.Model.Struct.User i in adminList)
                {
                    msg += "Имя:" + i.firstName + "\n" + "Имя пользователя: " + i.userName + "\n\n";
                }
            }
            await MessageController.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboard, update);
        }
    }
}