Бот предназначен для массовой рассылки новостей подписанным пользователям.
Функции:
* Подписка/отпискиа;
* По команде "/new" бот просит прислать новость для публикации, после чего отправляет вам обратно текст для предпросмотра, под этим собщением будет кнопка для публикации, после нажатия на которую происходит рассылка этой новости всем подписчикам



Для работы с базой данных используется БД Postgres 14.7. Команды для создания таблиц и полей:

Таблица со списком пользователей:
```SQL
CREATE TABLE users
(  
    tg_id BIGINT NOT NULL UNIQUE PRIMARY KEY, 
    first_name VARCHAR(128) NOT NULL,
    username VARCHAR(32) NOT NULL,
    is_admin bool
);
```

Таблица со списком подписок:
```SQL
CREATE TABLE subscriptions
(  
    id SERIAL NOT NULL PRIMARY KEY,
    title VARCHAR(50) NOT NULL,
    description TEXT,
    price INTEGER NOT NULL
);
```

Связывающая таблица подписок и пользователей:
```SQL
CREATE TABLE user_subscriptions 
(
    tg_id BIGINT REFERENCES users(tg_id),    
    subscription_id INTEGER REFERENCES subscriptions(id),
    CONSTRAINT user_subscriptions_pk PRIMARY KEY (tg_id, subscription_id)
);
```

Таблица контроля за InvoicePayload
```SQL
CREATE TABLE invoice_payload
(  
    invoice_payload_id SERIAL NOT NULL PRIMARY KEY,
    chat_id BIGINT NOT NULL,
    title_subscribe VARCHAR(50) NOT NULL,
    payment_completed BOOL NOT NULL
);
```
