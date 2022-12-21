﻿using Microsoft.Extensions.Hosting;
using UtilityBot.Controllers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UtilityBot.Services;

namespace UtilityBot
{
    internal class Bot : BackgroundService
    {
        //клиент к Telegram.Bot.API
        private ITelegramBotClient _telegramClient;

        // Контроллеры различных видов сообщений
        private InlineKeyboardController _inlineKeyboardController;
        private TextMessageController _textMessageController;
        private DefaultMessageController _defaultMessageController;

        public Bot(
            ITelegramBotClient telegramClient,
            InlineKeyboardController inlineKeyboardController,
            TextMessageController textMessageController,
            DefaultMessageController defaultMessageController)
        {
            _telegramClient = telegramClient;
            _inlineKeyboardController = inlineKeyboardController;
            _textMessageController = textMessageController;
            _defaultMessageController = defaultMessageController;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Настраиваем получение обновлений - разрешены все
            _telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } },
                cancellationToken: stoppingToken);

            Console.WriteLine("Бот запущен.");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //Обрабатываем нажатия на кнопки из Telegram Bot API
            if (update.Type == UpdateType.CallbackQuery)
            {
                await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
                return;
            }

            //Обрабатываем входящие сообщения из Telegram Bot API
            if (update.Type == UpdateType.Message)
            {
                switch (update.Message!.Type)
                {
                    case MessageType.Text:
                        await _textMessageController.Handle(update.Message, cancellationToken);
                        return;
                    default:
                        await _defaultMessageController.Handle(update.Message, cancellationToken);
                        return;
                }
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            Console.WriteLine("Ожидаем 10 секунд перед повторным подключением.");
            Thread.Sleep(10000);
            return Task.CompletedTask;
        }
    }
}
