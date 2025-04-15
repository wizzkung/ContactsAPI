CREATE TABLE Contacts
(
id int primary key identity,
first_name nvarchar(100),
last_name nvarchar(100),
email nvarchar(100),
number nvarchar(12)
)

INSERT INTO Contacts (first_name, last_name, email, number)
VALUES 
(N'Мурат', N'Муратович', N'mura@mail.ru', N'123456789012'),
(N'Артур', N'Артурович', N'art@mail.ru', N'987654321098'),
(N'Сергей', N'Сергеевич', N'ser@mail.ru', N'555123456789'),
(N'Василий', N'Васильевич', N'vasya@mail.ru', N'444987654321');


create proc pGetAll
as
select *
from Contacts


-- Процедура добавления / редактирования
create proc pAdd
    @first_name NVARCHAR(100),
    @last_name NVARCHAR(100),
    @email NVARCHAR(100),
    @number NVARCHAR(12)
as
    if exists (SELECT 1 FROM Contacts WHERE email = @email)
        UPDATE Contacts
        SET
            first_name = @first_name,
            last_name = @last_name,
            number = @number
        WHERE email = @email;
    else
        INSERT INTO Contacts (first_name, last_name, email, number)
        VALUES (@first_name, @last_name, @email, @number);
 


 create proc pDelete
 @id int
 as
 delete from Contacts
 where id = @id
