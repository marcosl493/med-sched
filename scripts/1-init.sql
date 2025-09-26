CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE TABLE "Users" (
        "Id" uuid NOT NULL,
        "Name" character varying(500) NOT NULL,
        "Email" character varying(255) NOT NULL,
        "HashedPassword" character varying(44) NOT NULL,
        "Salt" character varying(24) NOT NULL,
        "Role" integer NOT NULL,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE TABLE "Patients" (
        "Id" uuid NOT NULL,
        "DateOfBirth" timestamp with time zone NOT NULL,
        "UserId" uuid NOT NULL,
        CONSTRAINT "PK_Patients" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Patients_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE TABLE "Physicians" (
        "Id" uuid NOT NULL,
        "Specialty" character varying(100) NOT NULL,
        "LicenseNumber" character varying(50) NOT NULL,
        "StateAbbreviation" character varying(2) NOT NULL,
        "UserId" uuid NOT NULL,
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_Physicians" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Physicians_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE TABLE "Schedules" (
        "Id" uuid NOT NULL,
        "PhysicianId" uuid NOT NULL,
        "StartTime" timestamp with time zone NOT NULL,
        "EndTime" timestamp with time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Schedules" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Schedules_Physicians_PhysicianId" FOREIGN KEY ("PhysicianId") REFERENCES "Physicians" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE TABLE "Appointments" (
        "Id" uuid NOT NULL,
        "Reason" character varying(500) NOT NULL,
        "Status" integer NOT NULL,
        "PatientId" uuid NOT NULL,
        "ScheduleId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Appointments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Appointments_Patients_PatientId" FOREIGN KEY ("PatientId") REFERENCES "Patients" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Appointments_Schedules_ScheduleId" FOREIGN KEY ("ScheduleId") REFERENCES "Schedules" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Appointments_PatientId_ScheduleId_Status" ON "Appointments" ("PatientId", "ScheduleId", "Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE INDEX "IX_Appointments_ScheduleId" ON "Appointments" ("ScheduleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Patients_UserId" ON "Patients" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Physicians_UserId" ON "Physicians" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Schedules_PhysicianId_StartTime_EndTime" ON "Schedules" ("PhysicianId", "StartTime", "EndTime");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250921201309_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250921201309_InitialCreate', '9.0.9');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250924193501_RemoveSaltFromUser') THEN
    ALTER TABLE "Users" DROP COLUMN "Salt";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250924193501_RemoveSaltFromUser') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250924193501_RemoveSaltFromUser', '9.0.9');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250924194829_ImproveHashedPasswordLenght') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250924194829_ImproveHashedPasswordLenght', '9.0.9');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250924195245_ImproveHashedPasswordLenghtOk') THEN
    ALTER TABLE "Users" ALTER COLUMN "HashedPassword" TYPE character varying(60);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250924195245_ImproveHashedPasswordLenghtOk') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250924195245_ImproveHashedPasswordLenghtOk', '9.0.9');
    END IF;
END $EF$;
COMMIT;

