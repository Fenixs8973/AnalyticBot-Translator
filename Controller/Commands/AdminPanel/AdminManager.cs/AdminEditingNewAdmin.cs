using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.DB;
using System.Text.RegularExpressions;
using HabrPost.LogException;

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

        MessageController mc = new MessageController();

        //Имя пользователя нового админа
        private string? newUserName;

        //Получил ли бот имя пользователя нового админа
        private bool newUserNameAccept = false;

        public async Task Execute(Update update)
        {
            MessageController mc = new MessageController();
            await mc.SendSimpleMessage("Пришлите ссылку или имя пользователя нового администратора в виде:\n\n*\"https://t.me/имя_пользователя\"\n*\"t.me/имя_пользователя\"\n*\"@имя_пользователя\"", update);
            Executor.StartListen(this);
        }

        public async Task Redirection(Update update)
        {
            //Префикс AdminManagerNewAdmin
            Regex adminManagerNewAdmin = new Regex(@"^AdminManagerNewAdmin.*");
            MatchCollection matchesAdminManagerNewAdmin = null;
            //Имя пользователя из ссылки
            Regex userNameURL = new Regex(@"t.me/(.*)");
            MatchCollection matchesUserNameURL = null;
            //Имя пользователя через "@"
            Regex userNameAt = new Regex(@"@(.*)");
            MatchCollection mathcesUserNameAt = null;
            try
            {
                matchesAdminManagerNewAdmin = adminManagerNewAdmin.Matches(update.Message.Text);
                matchesUserNameURL = userNameURL.Matches(update.Message.Text);
                mathcesUserNameAt = userNameAt.Matches(update.Message.Text);
            }
            catch
            {
                
            }

            //Если нет совпадения ни с одной регуляркой
            if(matchesAdminManagerNewAdmin == null && matchesUserNameURL == null && mathcesUserNameAt  == null)
            {
                //Обрабатываем команду как обычную
                Executor.StopListen(update);
                UpdateDistributor<CommandExecutor> updateDistributor = new UpdateDistributor<CommandExecutor>();
                updateDistributor.UpdateProcessing(update);
                return;
            }
            
            DBRequest db = new DBRequest();
            MessageController mc = new MessageController();

            //Кнопка админ-меню
            InlineKeyboardMarkup menu = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                }
            });

            //Получение имени пользователя нового админа
            try
            {
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
                        await mc.SendInlineKeyboardMessage($"Имя пользователя не распознано. Проверьте корректность", menu, update); 
                        return;
                    }
                        
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

                    //Отправляем на подтверждение новое имя пользователя
                    await mc.SendInlineKeyboardMessage($"Имя пользователя нового администратора: {newUserName}", acceptOrCancel, update);  
                }
                //Если мы получили имя пользователя
                else if(newUserNameAccept)
                {
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
                    //Отправляем новое имя пользователя
                    await mc.SendInlineKeyboardMessage($"Имя пользователя нового администратора: {newUserName}", acceptOrCancel, update);  
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
                Executor.StopListen(update);
            }
        }
    }
}