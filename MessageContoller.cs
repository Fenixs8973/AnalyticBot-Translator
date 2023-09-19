using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Model;
using HabrPost.LogException;
using HabrPost.Controllers;

namespace HabrPost.Controllers.Messages
{
    //Message message = await botClient.SendInvoiceAsync
    class MessageController
    {
        public static TelegramBotClient botClient => Bot.GetTelegramBot();

        ///<summary>
        ///Отправка сообщения с текстом
        ///</summary>
        public static async Task SendSimpleMessage(string msg, Update update)
        {
            long chatId = CommandExecutor.GetChatId(update);

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
        public static async Task SendInlineKeyboardMessage(string msg, InlineKeyboardMarkup inlineKeyboardMarkup, Update update)
        {
            long chatId = CommandExecutor.GetChatId(update);
            
            try
            {
                Message sendMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: msg,
                    replyMarkup: inlineKeyboardMarkup);
            }
            catch(Exception exception)
            {
                ExceptionLogger.NewException(exception);
            }
        }

        ///<summary>
        ///Отправка сообщения с текстом и кнопкой оплаты счета
        ///</summary> 
        public static async Task SendInvokeMessage(string subTitle, string subDescription, int subPrice, UInt32 payload, Update update)
        {
            long chatId = CommandExecutor.GetChatId(update);

            LabeledPrice[] prices = new LabeledPrice[1] { new LabeledPrice(subTitle, subPrice * 100)};
            try
            {
                Message sendMessage = await botClient.SendInvoiceAsync(chatId: chatId, 
                                                                        title: subTitle, 
                                                                        description: subDescription,
                                                                        payload: payload.ToString(),
                                                                        providerToken: "1744374395:TEST:c64e08679d65dcc627ac", 
                                                                        currency: "RUB",
                                                                        prices: (IEnumerable<LabeledPrice>)prices
                                                                        );
            }
            catch(Exception exception)
            {
                ExceptionLogger.NewException(exception);
            }
        }


        ///<summary>
        ///Подтверждающий ответ оплаты
        ///</summary>
        public static async Task SendAnswerPreCheckoutQuery(Update update)
        {
            await botClient.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id);
        }

        ///<summary>
        ///Изменение кнопок и текста сообщения
        ///</summary>
        public static async Task ReplaceInlineKeyboardMessageForMarkup(string msg, InlineKeyboardMarkup? inlineKeyboardMarkup, Update update)
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
                ExceptionLogger.NewException(exception);
            }
        }

        ///<summary>
        ///Изменение текста сообщения
        ///</summary>
        public static async Task ReplaceInlineKeyboardMessageForText(string msg, Update update)
        {
            long chatId = CommandExecutor.GetChatId(update);
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
        public static async Task<InlineKeyboardMarkup> GetInlineKeyboardForSubscriptions()
        {
            int rows;
            int h = 0;
            InlineKeyboardMarkup keyboardInline;

            await SubscriptionsArray.UpdateSubscriptions();
            //Если количество подписок четное
            if(SubscriptionsArray.subArray.Length % 2 == 0)
            {
                rows = SubscriptionsArray.subArray.Length/2 + 1;
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 1; i++)
                {
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h].title, callbackData: "SubscriptionEditing" + SubscriptionsArray.subArray[h].title),
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h + 1].title, callbackData: "SubscriptionEditing" + SubscriptionsArray.subArray[h + 1].title)
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
            else if(SubscriptionsArray.subArray.Length % 2 != 0)
            {
                rows = (SubscriptionsArray.subArray.Length/2) + 2;
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 2; i++)
                {
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h].title, callbackData: "SubscriptionEditing" + SubscriptionsArray.subArray[h].title),
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h + 1].title, callbackData: "SubscriptionEditing" + SubscriptionsArray.subArray[h + 1].title)
                    };
                    h += 2;
                }
                qwerty[rows - 2] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[SubscriptionsArray.subArray.Length - 1].title, callbackData: "SubscriptionEditing" + SubscriptionsArray.subArray[SubscriptionsArray.subArray.Length - 1].title)
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
        public static async Task<InlineKeyboardMarkup> GetInlineKeyboardForAdminSubscriptions()
        {
            int rows;
            int h = 0;
            InlineKeyboardMarkup keyboardInline;

            await SubscriptionsArray.UpdateSubscriptions();
            //Если количество подписок четное
            if(SubscriptionsArray.subArray.Length % 2 == 0)
            {
                rows = SubscriptionsArray.subArray.Length/2 + 2;
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 2; i++)
                {
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h].title, callbackData: "AdminSubscriptionEditing" + SubscriptionsArray.subArray[h].title),
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h + 1].title, callbackData: "AdminSubscriptionEditing" + SubscriptionsArray.subArray[h + 1].title)
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
            else if(SubscriptionsArray.subArray.Length % 2 != 0)
            {
                rows = (SubscriptionsArray.subArray.Length/2) + 3;
                InlineKeyboardButton[][] qwerty = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 3; i++)
                {
                    qwerty[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h].title, callbackData: "AdminSubscriptionEditing" + SubscriptionsArray.subArray[h].title),
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h + 1].title, callbackData: "AdminSubscriptionEditing" + SubscriptionsArray.subArray[h + 1].title)
                    };
                    h += 2;
                }
                qwerty[rows - 3] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[SubscriptionsArray.subArray.Length - 1].title, callbackData: "AdminSubscriptionEditing" + SubscriptionsArray.subArray[SubscriptionsArray.subArray.Length - 1].title)
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
        public static async Task<InlineKeyboardButton[][]> GetInlineKeyboardForNewPublication()
        {
            int rows;
            int h = 0;

            await SubscriptionsArray.UpdateSubscriptions();
            //Если количество подписок четное
            if(SubscriptionsArray.subArray.Length % 2 == 0)
            {
                rows = SubscriptionsArray.subArray.Length/2 + 2;
                InlineKeyboardButton[][] inlineKeyboardButton = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 2; i++)
                {
                    inlineKeyboardButton[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h].title, callbackData: "AdminNewPublication" + SubscriptionsArray.subArray[h].title),
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h + 1].title, callbackData: "AdminNewPublication" + SubscriptionsArray.subArray[h + 1].title)
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
            else if(SubscriptionsArray.subArray.Length % 2 != 0)
            {
                rows = (SubscriptionsArray.subArray.Length/2) + 3;
                InlineKeyboardButton[][] inlineKeyboardButton = new InlineKeyboardButton[rows][];
                for(int i = 0; i < rows - 3; i++)
                {
                    inlineKeyboardButton[i] = new InlineKeyboardButton[2]
                    {
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h].title, callbackData: "AdminNewPublication" + SubscriptionsArray.subArray[h].title),
                        InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[h + 1].title, callbackData: "AdminNewPublication" + SubscriptionsArray.subArray[h + 1].title)
                    };
                    h += 2;
                }
                inlineKeyboardButton[rows - 3] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: SubscriptionsArray.subArray[SubscriptionsArray.subArray.Length - 1].title, callbackData: "AdminNewPublication" + SubscriptionsArray.subArray[SubscriptionsArray.subArray.Length - 1].title)
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
        public static readonly InlineKeyboardMarkup Start = new(new[]
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
        public static async Task<InlineKeyboardMarkup> GetInlineKeyboardForAdmins()
        {
            int rows;
            int h = 0;
            InlineKeyboardMarkup keyboardInline;

            await Admins.UpdateSubscriptions();
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