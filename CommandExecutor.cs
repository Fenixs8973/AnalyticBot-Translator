using System.Text.RegularExpressions;
using HabrPost.Controllers.Commands;
using HabrPost.Controllers.Commands.Admin;
using HabrPost.Controllers.DB;
using HabrPost.LogException;
using Telegram.Bot.Types;

namespace HabrPost.Controllers
{
    public class CommandExecutor : ITelegramUpdateRedirection
    {
        private List<ICommand> commandsList;

        public CommandExecutor()
        {
            commandsList = new List<ICommand>()
            {
                new AdminPublishNews(),
                new AdminEditingAdminRemoveYes(),
                new AdminEditingAdminRemove(),
                new AdminEditingNewAdminYes(),
                new AdminEditingNewAdmin(this),
                new AdminEditingAdmin(),
                new AdminPanel(),
                new AdminManager(),
                new AdminNewPublication(this),
                new AdminSubscriptionManager(),
                new AdminSubscriptionNew(this),
                new AdminSubscriptionEditingPrice(this),
                new AdminSubscriptionEditingDescription(this),
                new AdminSubscriptionEditingTitle(this),
                new AdminSubscriptionEditingRemove(),
                new AdminSubscriptionEditingRemoveYes(),
                new AdminSubscriptionEditing(),
                new StartCommand(),
                new SubscriptionManager(),
                new SubsciptionEditing(),
                new SubscribeInvoke(),
                new SubscribeCancel(),

            };
        }
        
        List<string> regexAdminCommandList = new List<string>()
        {
            "^(AdminEditingAdminRemoveYes)",
            "^(AdminEditingAdminRemove)",
            "^(AdminEditingNewAdminYes)",
            "^(AdminEditingNewAdmin)",
            "^(AdminEditingAdmin)",
            "^(AdminSubscriptionManager)",
            "^(AdminManager)",
            "^(AdminNewPublication)",
            "^(AdminPublishNews)",
            "^(AdminSubscriptionEditingRemoveYes)",
            "^(AdminSubscriptionEditingRemove)",
            "^(AdminSubscriptionEditingTitle)",
            "^(AdminSubscriptionEditingDescription)",
            "^(AdminSubscriptionEditingPrice)",
            "^(AdminSubscriptionEditing)",
            "^(AdminSubscriptionNew)",
        };

        List<string> regexUserCommandList = new List<string>()
        {
            "^(SubscriptionEditing)(.*)",
            "^(SubscribeInvoke)(.*)",
            "^(SubscribeCancel)(.*)"

        };
        static Regex adminRegex = new Regex("Admin(.*)");

        private IRedirection? redirection = null;

        public async Task UpdateProcessing(Update update) 
        {
            if(redirection == null)
            {
                await ExecuteCommand(update);
            }
            else
            {
                await redirection.Redirection(update);
            }
        }

        private async Task ExecuteCommand(Update update)
        {
            //Если пришло текстовое сообщение
            if(update.Message != null)
            {
                Message message = update.Message;
                //Ищем команду во всем списке команд
                foreach (var command in commandsList)
                {
                    if (command.Name == message.Text)
                    {
                        await command.Execute(update);
                        return;
                    }
                }
            }
            //Если команда пришла из кнопки
            else if(update.CallbackQuery != null)
            {
                //Ищем совпадения с регулярным выражением adminRegex
                MatchCollection adminCommandMatches = adminRegex.Matches(update.CallbackQuery.Data);
                CallbackQuery callBack = update.CallbackQuery;
                //Перебираем все пользовательские регулярки
                foreach(string i in regexUserCommandList)
                {
                    Regex regexUserCommand = new Regex(i);
                    MatchCollection regexUserCommandMatches = regexUserCommand.Matches(update.CallbackQuery.Data);
                    if(regexUserCommandMatches.Count > 0)
                    {
                        Match match = regexUserCommand.Match(callBack.Data);
                        //Перебираем все команды
                        foreach(var command in commandsList)
                        {
                            //Проверка через регулярку
                            if(match.Groups[1].ToString() == command.Name)
                            {
                                await command.Execute(update);
                                return;
                            }
                            //Проверка целого CallBack
                            else if(update.CallbackQuery.Data == command.Name)
                            {
                                await command.Execute(update);
                                return;
                            }
                        }
                    }
                }
                //Проверяем админ регулярки
                if(adminCommandMatches.Count > 0 && await DBRequest.AdminVerify(update.CallbackQuery.Message.Chat.Id))
                {
                    //Поиск подходящей регулярки
                    foreach(string i in regexAdminCommandList)
                    {
                        Regex regex = new Regex(i);
                        MatchCollection regexAdminCommandMatches = regex.Matches(callBack.Data);
                        if(regexAdminCommandMatches.Count > 0)
                        {
                            Match match = regex.Match(callBack.Data);
                            //Поиск команды из регулярки в общем списке команд
                            foreach (var command in commandsList)
                            {
                                if(match.Groups[1].ToString() == command.Name)
                                {
                                    await command.Execute(update);
                                    return;
                                }
                            }
                        }
                    }                
                }
                //Нет совпадений с регуляркой adminRegex, ищем комадну в commandsList
                else
                {
                    foreach (var command in commandsList)
                    {
                        if(update.CallbackQuery.Data == command.Name)
                        {
                            await command.Execute(update);
                            return;
                        }
                    }
                }
            }
        }

        public void StartListen(IRedirection newRedirection)
        {
            redirection = newRedirection;
        }

        public void StopListen(Update update)
        {
            UpdateDistributor<CommandExecutor>.RemoveRedirection(update);
            redirection = null;
        }

        static public long GetChatId(Update update)
        {
            long chatId = 0;
            if(update.CallbackQuery != null)
                chatId = update.CallbackQuery.Message.Chat.Id;
            else if(update.Message != null)
                chatId = update.Message.Chat.Id;
            
            return chatId;
        }
    }
}