use ucubot;
create table lesson_signal (id int not null auto_increment, time_stamp datetime,
     signal_type int, user_id char(255), primary key(id));
