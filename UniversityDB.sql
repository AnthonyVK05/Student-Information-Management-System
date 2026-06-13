USE master;
GO

IF DB_ID('UniversityDB') IS NOT NULL
BEGIN
    ALTER DATABASE UniversityDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE UniversityDB;
END
GO

CREATE DATABASE UniversityDB
ON
(
    NAME = UniversityDB_Data,
    FILENAME = 'C:\Users\linke\UniversityDB_Final.mdf'
)
LOG ON
(
    NAME = UniversityDB_Log,
    FILENAME = 'C:\Users\linke\UniversityDB_Final_log.ldf'
);
GO

USE UniversityDB;
GO

CREATE TABLE [USER]
(
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    email VARCHAR(100) UNIQUE,
    role VARCHAR(20) NOT NULL,
    is_active BIT DEFAULT 1,
    email_verified BIT DEFAULT 0,
    verification_token VARCHAR(100) NULL,
    verification_expiry DATETIME NULL,
    created_at DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE PROGRAMME
(
    programme_id INT IDENTITY(1,1) PRIMARY KEY,
    programme_name VARCHAR(100) NOT NULL,
    code VARCHAR(20) NOT NULL UNIQUE,
    duration_years INT,
    department VARCHAR(100)
);
GO

CREATE TABLE STUDENT
(
    student_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    programme_id INT NOT NULL,
    matric_number VARCHAR(20) UNIQUE NOT NULL,
    nric_passport VARCHAR(30) UNIQUE NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    date_of_birth DATE,
    admission_year INT NULL,
    admission_month INT NULL,
    status VARCHAR(20) DEFAULT 'Active',
    cgpa DECIMAL(4,2) DEFAULT 0.00,

    FOREIGN KEY(user_id) REFERENCES [USER](user_id),
    FOREIGN KEY(programme_id) REFERENCES PROGRAMME(programme_id)
);
GO

CREATE TABLE LECTURER
(
    lecturer_id INT IDENTITY(1,1) PRIMARY KEY,

    user_id INT NOT NULL,

    staff_id VARCHAR(20) UNIQUE NOT NULL,

    nric_passport VARCHAR(30) UNIQUE NOT NULL,

    full_name VARCHAR(100) NOT NULL,

    specialisation VARCHAR(100),

    hire_date DATE,

    status VARCHAR(20) DEFAULT 'Active',

    FOREIGN KEY(user_id) REFERENCES [USER](user_id)
);
GO
CREATE TABLE COURSE
(
    course_id INT IDENTITY(1,1) PRIMARY KEY,
    programme_id INT NOT NULL,
    course_name VARCHAR(100) NOT NULL,
    course_code VARCHAR(20) NOT NULL UNIQUE,
    credit_hours INT NOT NULL,
    semester VARCHAR(20),
    max_students INT DEFAULT 30,
    is_active BIT DEFAULT 1,

    FOREIGN KEY(programme_id) REFERENCES PROGRAMME(programme_id)
);
GO

CREATE TABLE ACADEMIC_CALENDAR
(
    calendar_id INT IDENTITY(1,1) PRIMARY KEY,
    academic_year VARCHAR(20),
    semester VARCHAR(20),
    start_date DATE,
    end_date DATE,
    status VARCHAR(20)
);
GO

CREATE TABLE LECTURER_COURSE
(
    lc_id INT IDENTITY(1,1) PRIMARY KEY,
    lecturer_id INT NOT NULL,
    course_id INT NOT NULL,
    academic_year VARCHAR(20),
    semester VARCHAR(20),

    FOREIGN KEY(lecturer_id) REFERENCES LECTURER(lecturer_id),
    FOREIGN KEY(course_id) REFERENCES COURSE(course_id),

    CONSTRAINT UQ_LecturerCourse UNIQUE
    (lecturer_id, course_id, academic_year, semester)
);
GO

CREATE TABLE ENROLMENT
(
    enrolment_id INT IDENTITY(1,1) PRIMARY KEY,
    student_id INT NOT NULL,
    course_id INT NOT NULL,
    calendar_id INT NOT NULL,
    semester VARCHAR(20),
    academic_year VARCHAR(20),
    enrol_date DATE DEFAULT GETDATE(),
    status VARCHAR(20) DEFAULT 'Pending',

    FOREIGN KEY(student_id) REFERENCES STUDENT(student_id),
    FOREIGN KEY(course_id) REFERENCES COURSE(course_id),
    FOREIGN KEY(calendar_id) REFERENCES ACADEMIC_CALENDAR(calendar_id),

    CONSTRAINT UQ_StudentCourse UNIQUE
    (student_id, course_id, academic_year, semester)
);
GO

CREATE TABLE ATTENDANCE
(
    attendance_id INT IDENTITY(1,1) PRIMARY KEY,
    enrolment_id INT NOT NULL,
    lecturer_id INT NOT NULL,
    class_date DATE,
    status VARCHAR(20),
    remarks VARCHAR(200),

    FOREIGN KEY(enrolment_id) REFERENCES ENROLMENT(enrolment_id),
    FOREIGN KEY(lecturer_id) REFERENCES LECTURER(lecturer_id)
);
GO

CREATE TABLE COURSE_MARKS
(
    mark_id INT IDENTITY(1,1) PRIMARY KEY,
    enrolment_id INT NOT NULL,
    coursework_marks DECIMAL(5,2),
    final_exam_marks DECIMAL(5,2),
    total_marks DECIMAL(5,2),
    grade VARCHAR(5),
    grade_point DECIMAL(4,2),

    FOREIGN KEY(enrolment_id) REFERENCES ENROLMENT(enrolment_id)
);
GO

CREATE TABLE ASSESSMENT
(
    assessment_id INT IDENTITY(1,1) PRIMARY KEY,
    course_id INT NOT NULL,
    title VARCHAR(100),
    type VARCHAR(50),
    max_marks DECIMAL(10,2),
    weight_percent DECIMAL(5,2),

    FOREIGN KEY(course_id) REFERENCES COURSE(course_id)
);
GO

CREATE TABLE GRADE
(
    grade_id INT IDENTITY(1,1) PRIMARY KEY,
    enrolment_id INT NOT NULL,
    assessment_id INT NOT NULL,
    marks_obtained DECIMAL(10,2),
    letter_grade VARCHAR(5),
    is_published BIT DEFAULT 0,

    FOREIGN KEY(enrolment_id) REFERENCES ENROLMENT(enrolment_id),
    FOREIGN KEY(assessment_id) REFERENCES ASSESSMENT(assessment_id)
);
GO

CREATE TABLE ANNOUNCEMENT
(
    announcement_id INT IDENTITY(1,1) PRIMARY KEY,
    posted_by INT NOT NULL,
    course_id INT NULL,
    target_role VARCHAR(20) DEFAULT 'ALL',
    title VARCHAR(200),
    content VARCHAR(MAX),
    posted_at DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(posted_by) REFERENCES [USER](user_id),
    FOREIGN KEY(course_id) REFERENCES COURSE(course_id)
);
GO

CREATE TABLE NOTIFICATION
(
    notification_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    type VARCHAR(50),
    message VARCHAR(MAX),
    is_read BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(user_id) REFERENCES [USER](user_id)
);
GO

CREATE TABLE MESSAGE
(
    message_id INT IDENTITY(1,1) PRIMARY KEY,
    sender_user_id INT NOT NULL,
    receiver_user_id INT NOT NULL,
    subject VARCHAR(200) NOT NULL,
    message_body VARCHAR(MAX) NOT NULL,
    parent_message_id INT NULL,
    is_read BIT DEFAULT 0,
    sent_at DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(sender_user_id) REFERENCES [USER](user_id),
    FOREIGN KEY(receiver_user_id) REFERENCES [USER](user_id),
    FOREIGN KEY(parent_message_id) REFERENCES MESSAGE(message_id)
);
GO

CREATE TABLE FEE
(
    fee_id INT IDENTITY(1,1) PRIMARY KEY,
    student_id INT NOT NULL,
    academic_year VARCHAR(20),
    semester VARCHAR(20),
    total_amount DECIMAL(10,2) DEFAULT 0.00,
    paid_amount DECIMAL(10,2) DEFAULT 0.00,
    balance_amount DECIMAL(10,2) DEFAULT 0.00,
    status VARCHAR(20) DEFAULT 'Unpaid',

    FOREIGN KEY(student_id) REFERENCES STUDENT(student_id)
);
GO

CREATE TABLE PAYMENT
(
    payment_id INT IDENTITY(1,1) PRIMARY KEY,
    fee_id INT NOT NULL,
    payment_date DATE DEFAULT GETDATE(),
    amount_paid DECIMAL(10,2),
    payment_method VARCHAR(50),
    receipt_no VARCHAR(50),
    status VARCHAR(20) DEFAULT 'Successful',
    remarks VARCHAR(200) NULL,

    FOREIGN KEY(fee_id) REFERENCES FEE(fee_id)
);
GO

CREATE TABLE ACADEMIC_WARNING
(
    warning_id INT IDENTITY(1,1) PRIMARY KEY,
    student_id INT NOT NULL,
    warning_type VARCHAR(50),
    warning_message VARCHAR(MAX),
    cgpa DECIMAL(4,2),
    attendance_percentage DECIMAL(5,2),
    status VARCHAR(20) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    resolved_at DATETIME NULL,

    FOREIGN KEY(student_id) REFERENCES STUDENT(student_id)
);
GO

CREATE TABLE TRANSACTION_LOG
(
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NULL,
    action_type VARCHAR(100),
    table_name VARCHAR(100),
    record_id INT NULL,
    description VARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(user_id) REFERENCES [USER](user_id)
);
GO

USE UniversityDB;
GO

SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;