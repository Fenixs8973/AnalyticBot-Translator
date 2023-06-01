using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Model;
using HabrPost.LogException;

namespace HabrPost.Controllers.Messages
{
    class MessageController
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        ///<summary>
        ///Отправка сообщения с текстом
        ///</summary>
        public async Task SendSimpleMessage(string msg, Update update)
        {
            long chatId = 0;
            try
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
            }
            catch
            {
                chatId = update.Message.Chat.Id;
            }

            try
            {
                Message message = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: msg
                    );
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
        ///<summary>
        ///Отправка сообщения с текстом и кнопками
        ///</summary>
        public async Task SendInlineKeyboardMessage(string msg, InlineKeyboardMarkup inlineKeyboardMarkup, Update update)
        {
            long chatId = CommandExecutor.GetChatId(update);
            
            try
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: msg,
                    replyMarkup: inlineKeyboardMarkup);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }

        ///<summary>
        ///Изменение кнопок и текста сообщения
        ///</summary>
        public async Task ReplaceInlineKeyboardMessageForMarkup(string msg, InlineKeyboardMarkup? inlineKeyboardMarkup, Update update)
        {
            long chatId = CommandExecutor.GetChatId(update);
            
            //ID сообщеня, которое нужно изменить
            int messageId = update.CallbackQuery.Message.MessageId;

            try
            {
                Message EditText = await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: messageId,
                    text: msg,
                    replyMarkup: inlineKeyboardMarkup
                );
            }
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
            }
        }

        ///<summary>
        ///Изменение текста сообщения
        ///</summary>
        public async Task ReplaceInlineKeyboardMessageForText(string msg, Update update)
        {
            long chatId = update.CallbackQuery.Message.Chat.Id;
            int messageId = update.CallbackQuery.Message.MessageId;
            Message EditText = await botClient.EditMessageTextAsync(
                chatId: update.CallbackQuery.Message.Chat.Id,
                messageId: messageId,
                text: msg
            );
        }

        ///<summary>
        ///Формирование динамических кнопок для просмотра подписок для пользователя
        ///</summary>
        public InlineKeyboardMarkup GetInlineKeyboardForSubscriptions()
        {
            int rows;
            int h = 0;
            InlineKeyboardMarkup keyboardInline;

            Subscriptions.UpdateSubscriptions();
            //Если количество подписок четное
            if(Subscriptions.subList.Length % 2 == 0)
            {
                rows = Subscriptions.subList.Length/2 + 1;
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 1; i++)
                {
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h].title, callbackData: "SubscriptionEditing" + Subscriptions.subList[h].title),
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h + 1].title, callbackData: "SubscriptionEditing" + Subscriptions.subList[h + 1].title)
                    };
                    h += 2;
                }
                qwerty[rows - 1] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/start")                    
                };
                keyboardInline = new(qwerty);
            }
            //Если количество подписок не четное
            else if(Subscriptions.subList.Length % 2 != 0)
            {
                rows = (Subscriptions.subList.Length/2) + 2;
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 2; i++)
                {
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h].title, callbackData: "SubscriptionEditing" + Subscriptions.subList[h].title),
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h + 1].title, callbackData: "SubscriptionEditing" + Subscriptions.subList[h + 1].title)
                    };
                    h += 2;
                }
                qwerty[rows - 2] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[Subscriptions.subList.Length - 1].title, callbackData: "SubscriptionEditing" + Subscriptions.subList[Subscriptions.subList.Length - 1].title)
                };
                qwerty[rows - 1] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/start")
                };
                keyboardInline = new(qwerty);
            }
            else
            {
                keyboardInline = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/start")
                    }
                });
            }
            return keyboardInline;
        }

        ///<summary>
        ///Формирование динамических кнопок для просмотра подписок для администратора
        ///</summary>
        public InlineKeyboardMarkup GetInlineKeyboardForAdminSubscriptions()
        {
            int rows;
            int h = 0;
            InlineKeyboardMarkup keyboardInline;

            Subscriptions.UpdateSubscriptions();
            //Если количество подписок четное
            if(Subscriptions.subList.Length % 2 == 0)
            {
                rows = Subscriptions.subList.Length/2 + 2;
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 2; i++)
                {
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h].title, callbackData: "AdminSubscriptionEditing" + Subscriptions.subList[h].title),
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h + 1].title, callbackData: "AdminSubscriptionEditing" + Subscriptions.subList[h + 1].title)
                    };
                    h += 2;
                }
                qwerty[rows - 2] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Добавить новую подписку", callbackData: "AdminSubscriptionNew")                    
                };
                qwerty[rows - 1] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")                    
                };
                keyboardInline = new(qwerty);
            }
            //Если количество подписок не четное
            else if(Subscriptions.subList.Length % 2 != 0)
            {
                rows = (Subscriptions.subList.Length/2) + 3;
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 3; i++)
                {
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h].title, callbackData: "AdminSubscriptionEditing" + Subscriptions.subList[h].title),
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h + 1].title, callbackData: "AdminSubscriptionEditing" + Subscriptions.subList[h + 1].title)
                    };
                    h += 2;
                }
                qwerty[rows - 3] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[Subscriptions.subList.Length - 1].title, callbackData: "AdminSubscriptionEditing" + Subscriptions.subList[Subscriptions.subList.Length - 1].title)
                };
                qwerty[rows - 2] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Добавить новую подписку", callbackData: "AdminSubscriptionNew")                    
                };
                qwerty[rows - 1] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                };
                keyboardInline = new(qwerty);
            }
            //Если subList пустой или равен нулю
            else
            {
                keyboardInline = new(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Добавить подписку", callbackData: "AdminSubscriptionEditingTitle"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin"),
                    }
                });
            }
            return keyboardInline;
        }

        ///<summary>
        ///Формирование динамических кнопок для публикации новости
        ///</summary>
        public InlineKeyboardButton[][] GetInlineKeyboardForNewPublication()
        {
            int rows;
            int h = 0;

            Subscriptions.UpdateSubscriptions();
            //Если количество подписок четное
            if(Subscriptions.subList.Length % 2 == 0)
            {
                rows = Subscriptions.subList.Length/2 + 2;
                InlineKeyboardButton[][] inlineKeyboardButton = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 2; i++)
                {
                    inlineKeyboardButton[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h].title, callbackData: "AdminNewPublication" + Subscriptions.subList[h].title),
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h + 1].title, callbackData: "AdminNewPublication" + Subscriptions.subList[h + 1].title)
                    };
                    h += 2;
                }
                inlineKeyboardButton[rows - 2] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Опубликовать", callbackData: "AdminPublishNews")                    
                };
                inlineKeyboardButton[rows - 1] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")                    
                };
                return inlineKeyboardButton;
            }
            //Если количество подписок не четное
            else if(Subscriptions.subList.Length % 2 != 0)
            {
                rows = (Subscriptions.subList.Length/2) + 3;
                InlineKeyboardButton[][] inlineKeyboardButton = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 3; i++)
                {
                    inlineKeyboardButton[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h].title, callbackData: "AdminNewPublication" + Subscriptions.subList[h].title),
                        InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[h + 1].title, callbackData: "AdminNewPublication" + Subscriptions.subList[h + 1].title)
                    };
                    h += 2;
                }
                inlineKeyboardButton[rows - 3] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: Subscriptions.subList[Subscriptions.subList.Length - 1].title, callbackData: "AdminNewPublication" + Subscriptions.subList[Subscriptions.subList.Length - 1].title)
                };
                inlineKeyboardButton[rows - 2] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Опубликовать", callbackData: "AdminPublishNews")
                };
                inlineKeyboardButton[rows - 1] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                };
                return inlineKeyboardButton;
            }
            else
            {
                InlineKeyboardButton[][] inlineKeyboardButton = new InlineKeyboardButton[1][];
                inlineKeyboardButton[0] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                };
                return inlineKeyboardButton;
            }
        }
        
        ///<summary>
        ///Кнопки для команды /start
        ///</summary>
        static public readonly InlineKeyboardMarkup Start = new(new[]
        {
            // first row
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/start")
            }
        });


        ///<summary>
        ///Формирование динамических кнопок для списка администраторов
        ///</summary>
        public InlineKeyboardMarkup GetInlineKeyboardForAdmins()
        {
            int rows;
            int h = 0;
            InlineKeyboardMarkup keyboardInline;

            Admins.UpdateSubscriptions();
            //Если количество администраторов четное
            if(Admins.adminList.Length % 2 == 0)
            {
                //Считаем количество строк
                rows = Admins.adminList.Length/2 + 2;
                //Создаем строки для кнопок
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                //Заполняем строки кнопками
                for(int i = 0; i < rows - 2; i++)
                {
                    //Две кнопки на строку
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: Admins.adminList[h].userName, callbackData: "AdminEditingAdmin" + Admins.adminList[h].userName),
                        InlineKeyboardButton.WithCallbackData(text: Admins.adminList[h + 1].userName, callbackData: "AdminEditingAdmin" + Admins.adminList[h + 1].userName)
                    };
                    h += 2;
                }
                //Однра кнопка на строку
                qwerty[rows - 2] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Добавить нового администратора", callbackData: "AdminEditingNewAdmin")                    
                };
                //Однра кнопка на строку
                qwerty[rows - 1] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")                    
                };
                keyboardInline = new(qwerty);
            }
            //Если количество администраторов не четное
            else if(Admins.adminList.Length % 2 != 0)
            {
                //Считаем количество строк
                rows = (Admins.adminList.Length/2) + 3;
                //Создаем строки для кнопок
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                //Заполняем строки кнопками
                for(int i = 0; i < rows - 3; i++)
                {
                    //Две кнопки на строку
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: Admins.adminList[h].userName, callbackData: "AdminEditingAdmin" + Admins.adminList[h].userName),
                        InlineKeyboardButton.WithCallbackData(text: Admins.adminList[h + 1].userName, callbackData: "AdminEditingAdmin" + Admins.adminList[h + 1].userName)
                    };
                    h += 2;
                }
                //Одна кнопка на строку
                qwerty[rows - 3] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: Admins.adminList[Admins.adminList.Length - 1].userName, callbackData: "AdminEditingAdmin" + Admins.adminList[Admins.adminList.Length - 1].userName)
                };
                //Однра кнопка на строку
                qwerty[rows - 2] = new InlineKeyboardButton[1]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Добавить нового администратора", callbackData: "AdminEditingNewAdmin")                    
                };
                //Одна кнопка на строку
                qwerty[rows - 1] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                };
                keyboardInline = new(qwerty);
            }
            else
            {
                keyboardInline = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                    }
                });
            }
            return keyboardInline;
        }
    }
}