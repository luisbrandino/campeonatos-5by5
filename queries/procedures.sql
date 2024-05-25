use db_futebol;
go

create or alter proc buscar_time_com_maior_pontuacao @campeonato_id int
as
begin
	select top 1 t.id as time_id
		from tb_time t
		join tb_participante p on t.id = p.time_id
		where p.campeonato_id = @campeonato_id
		order by p.pontos desc, p.total_gols desc;
end
go

create or alter proc buscar_time_com_mais_gols @campeonato_id int
as
begin
	select top 1 t.id, t.nome, t.apelido, t.data_criacao, p.total_gols
		from tb_time t
		join tb_participante p on t.id = p.time_id
		where p.campeonato_id = @campeonato_id
		order by p.total_gols desc;
end
go

create or alter proc buscar_maior_numero_de_gols_de_cada_time @campeonato_id int
as
begin
	declare @max_gols table (time_id int, max_gols int)

	insert into @max_gols (time_id, max_gols)
	select time_mandante_id, MAX(gols_time_mandante)
	from tb_jogo
	group by time_mandante_id

	insert into @max_gols (time_id, max_gols)
	select time_visitante_id, MAX(gols_time_visitante)
	from tb_jogo
	group by time_visitante_id

	select time_id, t.nome, t.apelido, t.data_criacao, MAX(max_gols) as 'max_gols' from @max_gols join tb_time as t on t.id = time_id group by time_id, t.nome, t.apelido, t.data_criacao;
end
go

create or alter proc buscar_time_que_tomou_mais_gols @campeonato_id int
as
begin
	declare @gols_sofridos table (time_id int, gols_sofridos int)

	-- gols sofridos pelo time mandante
	insert into @gols_sofridos (time_id, gols_sofridos)
	select time_mandante_id, sum(gols_time_visitante)
	from tb_jogo
	where campeonato_id = @campeonato_id
	group by time_mandante_id

	-- gols sofridos pelo time visitante
	insert into @gols_sofridos (time_id, gols_sofridos)
	select time_visitante_id, sum(gols_time_mandante)
	from tb_jogo
	where campeonato_id = @campeonato_id
	group by time_visitante_id

	select top 1 time_id, sum(gols_sofridos) as total_gols_sofridos from @gols_sofridos group by time_id order by total_gols_sofridos desc
end
go

create or alter proc buscar_jogo_com_mais_gols @campeonato_id int
as
begin
	select top 1 campeonato_id, time_mandante_id, time_visitante_id, time_vencedor_id, gols_time_mandante, gols_time_visitante
	from tb_jogo
	where campeonato_id = @campeonato_id
	order by (gols_time_mandante + gols_time_visitante) desc
end
go

create or alter proc time_existe @nome varchar(30)
as
begin
	if exists(select 1 from tb_time where nome = @nome)
		select 1
	else
		select 0
	return
end
go

create or alter proc criar_time @nome varchar(30), @apelido varchar(30), @data_criacao date
as
begin
	if exists(select 1 from tb_time where nome = @nome)
		return

	insert into tb_time (nome, apelido, data_criacao)
	values (@nome, @apelido, @data_criacao)
end
go

create or alter proc campeonato_existe @nome varchar(30)
as
begin
	if exists(select 1 from tb_campeonato where nome = @nome)
		select 1
	else
		select 0
	return
end
go

create or alter proc criar_campeonato @nome varchar(30)
as
begin
	if exists(select 1 from tb_campeonato where nome = @nome)
		return

	insert into tb_campeonato (nome)
	values (@nome)
end
go

create or alter proc definir_campeao_campeonato @campeonato_id int
as
begin
	if not exists(select 1 from tb_campeonato where id = @campeonato_id)
		return

	declare @temp table (time_id int)
	declare @time_id int

	insert into @temp(time_id) exec buscar_time_com_maior_pontuacao @campeonato_id

	select @time_id = time_id from @temp

	update tb_campeonato set
	time_campeao_id = @time_id
	where id = @campeonato_id
end
go

create or alter proc jogo_existe @campeonato_id int, @time_mandante_id int, @time_visitante_id int
as
begin
	if exists(select 1 from tb_jogo where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id)
		select 1
	else
		select 0
	return
end
go

create or alter proc criar_jogo @campeonato_id int, @time_mandante_id int, @time_visitante_id int
as
begin
	if @time_visitante_id = @time_mandante_id
		return

	if not exists(select 1 from tb_campeonato where id = @campeonato_id)
		return

	if not exists(select 1 from tb_time where id = @time_mandante_id)
		return

	if not exists(select 1 from tb_time where id = @time_visitante_id)
		return

	if exists(select 1 from tb_jogo where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id)
		return

	insert into tb_jogo (campeonato_id, time_mandante_id, time_visitante_id)
	values (@campeonato_id, @time_mandante_id, @time_visitante_id)
end
go

create or alter proc definir_vendecedor_jogo @campeonato_id int, @time_mandante_id int, @time_visitante_id int
as
begin
	if not exists(select 1 from tb_jogo where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id)
		return

	declare
		@gols_time_mandante int,
		@gols_time_visitante int,
		@time_vencedor_id int

	select @gols_time_mandante = gols_time_mandante, @gols_time_visitante = gols_time_visitante from tb_jogo where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id

	if @gols_time_mandante > @gols_time_visitante
		set @time_vencedor_id = @time_mandante_id
	else if @gols_time_mandante < @gols_time_visitante
		set @time_vencedor_id = @time_visitante_id
	else
		set @time_vencedor_id = null

	update tb_jogo
	set time_vencedor_id = @time_vencedor_id
	where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id

	if @time_vencedor_id is null
		return

	declare
		@pontos int = IIF(@time_vencedor_id = @time_mandante_id, 3, 5)

	exec adicionar_pontos @campeonato_id, @time_vencedor_id, @pontos	
end
go

create or alter proc definir_gols_time_mandante @campeonato_id int, @time_mandante_id int, @time_visitante_id int, @gols int
as
begin
	if not exists(select 1 from tb_jogo where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id)
		return

	update tb_jogo
	set gols_time_mandante = @gols
	where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id
	
	exec adicionar_gols @campeonato_id, @time_mandante_id, @gols
	exec definir_vendecedor_jogo @campeonato_id, @time_mandante_id, @time_visitante_id
end
go

create or alter proc definir_gols_time_visitante @campeonato_id int, @time_mandante_id int, @time_visitante_id int, @gols int
as
begin
	if not exists(select 1 from tb_jogo where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id)
		return

	update tb_jogo
	set gols_time_visitante = @gols
	where campeonato_id = @campeonato_id and time_mandante_id = @time_mandante_id and time_visitante_id = @time_visitante_id

	exec adicionar_gols @campeonato_id, @time_visitante_id, @gols
	exec definir_vendecedor_jogo @campeonato_id, @time_mandante_id, @time_visitante_id
end
go

create or alter proc participante_existe @campeonato_id int, @time_id int
as
begin
	if exists(select 1 from tb_participante where campeonato_id = @campeonato_id and time_id = @time_id)
		select 1
	else
		select 0
	return
end
go

create or alter proc criar_participante @campeonato_id int, @time_id int
as
begin
	if exists(select 1 from tb_participante where campeonato_id = @campeonato_id and time_id = @time_id)
		return

	declare @participantes int;

	select @participantes = count(*) from tb_participante where campeonato_id = @campeonato_id

	if @participantes >= 5
		return

	insert into tb_participante (campeonato_id, time_id)
	values (@campeonato_id, @time_id)
end
go

create or alter proc adicionar_pontos @campeonato_id int, @time_id int, @pontos int
as
begin
	if not exists(select 1 from tb_participante where campeonato_id = @campeonato_id and time_id = @time_id)
		return

	update tb_participante
	set pontos = pontos + @pontos
	where campeonato_id = @campeonato_id and time_id = @time_id
end
go

create or alter proc adicionar_gols @campeonato_id int, @time_id int, @gols int
as
begin
	if not exists(select 1 from tb_participante where campeonato_id = @campeonato_id and time_id = @time_id)
		return

	update tb_participante
	set total_gols = total_gols + @gols
	where campeonato_id = @campeonato_id and time_id = @time_id
end
go
use db_futebol;
select * from tb_campeonato;
select * from tb_time;
delete from tb_time;