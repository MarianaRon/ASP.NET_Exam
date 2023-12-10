CREATE TRIGGER IF NOT EXISTS CountGroupSizeOnInsert
    AFTER
INSERT ON ApplicationUserSchedules
BEGIN
    UPDATE Schedules
    SET GroupSize = (SELECT COUNT(*) FROM ApplicationUserSchedules WHERE ScheduleId = NEW.ScheduleId)
    WHERE Id IN (SELECT Id FROM Schedules WHERE Id = NEW.ScheduleId);
END;

CREATE TRIGGER IF NOT EXISTS CountGroupSizeOnDelete
    AFTER
DELETE ON ApplicationUserSchedules
BEGIN
    UPDATE Schedules
    SET GroupSize = (SELECT COUNT(*) FROM ApplicationUserSchedules WHERE ScheduleId = OLD.ScheduleId)
    WHERE Id IN (SELECT ID FROM Schedules WHERE Id = OLD.ScheduleId);
END;


DROP TRIGGER IF EXISTS CountGroupSizeOnInsert;
DROP TRIGGER IF EXISTS CountGroupSizeOnDelete
