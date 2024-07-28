SELECT * FROM Owner;
SELECT * FROM Member;
SELECT * FROM Vehicle;

DELETE FROM Owner;
DELETE FROM Member;
DELETE FROM Vehicle;

DELETE FROM Vehicle
WHERE Patent = 'XC7415';

SELECT * FROM Vehicle
ORDER BY Driver;

SELECT * FROM Vehicle
WHERE Type = 'Supercar'
ORDER BY Driver;

SELECT * FROM Vehicle
WHERE Type = 'Supercar'
AND 
Driver = '17961621-1';

SELECT * FROM Vehicle
WHERE Driver = '17961621-1';

SELECT * FROM Vehicle
WHERE Driver = '18745236-K';

SELECT * FROM Vehicle
WHERE Type = 'Supercar'
AND 
Driver = '18745236-K';

SELECT * FROM Vehicle
ORDER BY Driver;

INSERT INTO Owner(ID, Fullname, Age)
VALUES ('17961621-1', 'José Jr.', 32);

INSERT INTO Member(ID, Username, Password)
VALUES ('17961621-1', 'Pelao', 123456);

INSERT INTO Owner(ID, Fullname, Age)
VALUES ('18745236-K', 'Juan', 29);

INSERT INTO Member(ID, Username, Password)
VALUES ('18745236-K', 'Juancho', 789456);

