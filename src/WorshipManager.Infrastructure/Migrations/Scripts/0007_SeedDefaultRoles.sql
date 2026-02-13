-- Seed default roles (global, no OrganizationId)

INSERT INTO "Role" ("Id", "OrganizationId", "Name", "Category", "Icon", "DisplayOrder", "IsActive")
VALUES
    (1, NULL, 'Vocal', 'Vocal', 'microphone', 1, true),
    (2, NULL, 'Violão', 'Instrumento', 'guitar', 2, true),
    (3, NULL, 'Guitarra', 'Instrumento', 'guitar-electric', 3, true),
    (4, NULL, 'Baixo', 'Instrumento', 'bass', 4, true),
    (5, NULL, 'Bateria', 'Instrumento', 'drums', 5, true),
    (6, NULL, 'Teclado', 'Instrumento', 'piano', 6, true),
    (7, NULL, 'Produção de Áudio', 'Produção', 'sliders', 7, true),
    (8, NULL, 'Mídia', 'Produção', 'video', 8, true),
    (9, NULL, 'Back Vocal', 'Vocal', 'microphone-lines', 9, true),
    (10, NULL, 'Direção Musical', 'Liderança', 'music', 10, true);

-- Ensure the sequence is past the seeded IDs
SELECT setval(pg_get_serial_sequence('"Role"', 'Id'), 10);
