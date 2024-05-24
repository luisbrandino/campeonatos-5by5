create table tb_time(
	id int identity(1, 1),
	nome varchar(30) not null unique,
	apelido varchar(30),
	data_criacao date not null,

	constraint pk_time primary key (id)
);

create table tb_campeonato(
	id int identity(1, 1),
	time_campeao_id int,
	nome varchar(40) not null unique,

	constraint pk_campeonato primary key (id),
	constraint fk_time_campeao foreign key (id) references tb_time(id)
);

create table tb_jogo(
	campeonato_id int,
	time_mandante_id int,
	time_visitante_id int,
	time_vencedor_id int,
	gols_time_mandante int default(0),
	gols_time_visitante int default(0),

	constraint pk_jogo primary key (campeonato_id, time_mandante_id, time_visitante_id),
	constraint fk_time_mandante foreign key (time_mandante_id) references tb_time(id),
	constraint fk_time_visitante foreign key (time_visitante_id) references tb_time(id),
	constraint fk_time_vencedor foreign key (time_vencedor_id) references tb_time(id)
);

create table tb_participante(
	campeonato_id int,
	time_id int,
	pontos int default(0),
	total_gols int default(0),

	constraint pk_participante primary key (campeonato_id, time_id),
	constraint fk_participante_campeonato foreign key (campeonato_id) references tb_campeonato(id),
	constraint fk_participante_time foreign key (time_id) references tb_time(id)
);
