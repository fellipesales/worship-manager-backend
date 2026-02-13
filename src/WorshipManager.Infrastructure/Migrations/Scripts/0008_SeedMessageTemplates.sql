-- Seed default message templates

INSERT INTO "MessageTemplates" ("Id", "Name", "Type", "Template", "IsActive", "CreatedAt")
VALUES
    (1, 'Notificação de Escala', 'ScheduleNotification',
     E'Olá {MemberName}! 🎵\n\nVocê foi escalado para o culto de *{ServiceType}* no dia *{Date}*.\n\nPor favor, confirme sua presença.\n\nDeus abençoe! 🙏',
     true, '2025-01-01T00:00:00Z'),
    (2, 'Lembrete de Culto', 'Reminder',
     E'Olá {MemberName}! 🎵\n\nLembrando que você está escalado para o culto de *{ServiceType}* amanhã ({Date}).\n\nNos vemos lá! 🙏',
     true, '2025-01-01T00:00:00Z'),
    (3, 'Pedido de Confirmação', 'ConfirmationRequest',
     E'Olá {MemberName}! 🎵\n\nPrecisamos da sua confirmação para o culto de *{ServiceType}* no dia *{Date}*.\n\nVocê poderá participar?\n\nResponda: SIM ou NÃO',
     true, '2025-01-01T00:00:00Z'),
    (4, 'Alteração na Escala', 'ScheduleChanged',
     E'Olá {MemberName}! ⚠️\n\nHouve uma alteração na escala do culto de *{ServiceType}* no dia *{Date}*.\n\n{ChangeDescription}\n\nQualquer dúvida, entre em contato.',
     true, '2025-01-01T00:00:00Z');

-- Ensure the sequence is past the seeded IDs
SELECT setval(pg_get_serial_sequence('"MessageTemplates"', 'Id'), 4);
