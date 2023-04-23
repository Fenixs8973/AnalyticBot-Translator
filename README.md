Бот предназначен для массовой рассылки новостей подписанным пользователям.
Функции:
* Подписка/отпискиа;
* По команде "/new" бот просит прислать новость для публикации, после чего отправляет вам обратно текст для предпросмотра, под этим собщением будет кнопка для публикации, после нажатия на которую происходит рассылка этой новости всем подписчикам



Для работы с базой данных используется БД Postgres. Команды для создания таблиц и полей:
CREATE TABLE news(  
    id int NOT NULL PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    news_text DATE,
    status INTEGER
);

CREATE TABLE users(  
    id int NOT NULL PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    tgid INTEGER,
    subscribed BOOLEAN
);
