using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.RegularExpressions;

namespace HabrPost.Controllers.Commands
{
    public class AdminEditingNewAdmin : ICommand, IRedirection
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminEditingNewAdmin";

        public CommandExecutor Executor { get; }

        public AdminEditingNewAdmin(CommandExecutor executor)
        {
            Executor = executor;
        }

        //Имя пользователя нового админа
        private string? newUserName;

        //Получил ли бот имя пользователя нового админа
        private bool newUserNameAccept = false;

        public async Task Execute(Update update)
        {
            MessageController mc = new MessageController();
            await MessageController.SendSimpleMessage("Пришлите ссылку или имя пользователя нового администратора в виде:\n\n*\"https://t.me/имя_пользователя\"\n*\"t.me/имя_пользователя\"\n*\"@имя_пользователя\"", update);
            Executor.StartListen(this);
        }

        public async Task Redirection(Update update)
        {
            //Префикс AdminManagerNewAdmin
            Regex adminManagerNewAdmin = new Regex(@"^AdminManagerNewAdmin.*");
            MatchCollection matchesAdminManagerNewAdmin = adminManagerNewAdmin.Matches(update.Message.Text);
            //Имя пользователя из ссылки
            Regex userNameURL = new Regex(@"t.me/(.*)");
            MatchCollection matchesUserNameURL = userNameURL.Matches(update.Message.Text);
            //Имя пользователя через "@"
            Regex userNameAt = new Regex(@"@(.*)");
            MatchCollection mathcesUserNameAt = userNameAt.Matches(update.Message.Text);

            //Если нет совпадения ни с одной регуляркой
            if(matchesAdminManagerNewAdmin == null && matchesUserNameURL == null && mathcesUserNameAt  == null)
            {
                //Обрабатываем команду как обычную
                Executor.StopListen(update);
                UpdateDistributor<CommandExecutor> updateDistributor = new UpdateDistributor<CommandExecutor>();
                await updateDistributor.UpdateProcessing(update);
                return;
            }
            
            //Кнопка админ-меню
            InlineKeyboardMarkup menu = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                }
            });

            //Кнопки отмены и подтверждения
            InlineKeyboardMarkup acceptOrCancel = new(new[]
            {                    
                // first row
                new []
                    {
                    InlineKeyboardButton.WithCallbackData(text: "Отмена", callbackData: "/admin"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Подтвердить", callbackData: "AdminEditingNewAdminYes" + newUserName),
                }
            });

            //Получение имени пользователя нового админа
            //Если мы еще не получали имя пользователя
            if(!newUserNameAccept)
            {    
                //Если имя пользователя через ссылку
                if(matchesUserNameURL.Count > 0)
                {
                    //Выделяем чистое имя пользователя
                    Match matchUserNameURL = userNameURL.Match(update.Message.Text);
                    //Кешируем имя пользоватеял
                    newUserName = matchUserNameURL.Groups[1].ToString();
                    newUserNameAccept = true;
                }
                //Если имя пользователя через "@"
                else if(mathcesUserNameAt.Count > 0)
                {
                    //Выделяем чистое имя пользователя
                    Match matchUserNameAt = userNameAt.Match(update.Message.Text);
                    //Кешируем имя пользоватеял
                    newUserName = matchUserNameAt.Groups[1].ToString();
                    newUserNameAccept = true;
                }
                //Если сообщение не проходит через регулярки
                else
                {
                    //Говорим, что имя пользователя не правильное
                    await MessageController.SendInlineKeyboardMessage($"Имя пользователя не распознано. Проверьте корректность", menu, update); 
                    return;
                }
                
                //Отправляем на подтверждение новое имя пользователя
                await MessageController.SendInlineKeyboardMessage($"Имя пользователя нового администратора: {newUserName}", acceptOrCancel, update);  
            }
            //Если мы получили имя пользователя
            else if(newUserNameAccept)
            {
                //Отправляем новое имя пользователя
                await MessageController.SendInlineKeyboardMessage($"Имя пользователя нового администратора: {newUserName}", acceptOrCancel, update);  
            }
        }
    }
}