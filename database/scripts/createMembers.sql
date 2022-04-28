USE [LI4-Agentes];

-- Logins

drop user Diretor
drop user IC
drop user Agente
drop login Diretor
drop login IC
drop login Agente

create login Diretor with password = 'diretor',
default_database = [LI4-Agentes];

create login IC with password = 'inspetorchefe',
default_database = [LI4-Agentes];

create login Agente with password = 'agente',
default_database = [LI4-Agentes];

-- Users

create user Diretor for login Diretor;
alter role LI4_Diretor add member Diretor;

create user IC for login IC;
alter role LI4_IC add member IC;

create user Agente for login Agente;
alter role LI4_Agentes add member Agente;