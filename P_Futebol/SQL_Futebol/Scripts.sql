-- drop table clube
-- drop table partida
-- drop table classificacao

CREATE TABLE Clube
(
	Id int identity(1,1) PRIMARY KEY,
	Nome varchar(30) UNIQUE,
	Apelido varchar(20) UNIQUE,
	DtCriacao date
)
GO

CREATE TABLE Classificacao 
(
	IdTime int,
	Pontuacao int,
	Vitorias int,
	Empates int,
	Derrotas int,
	GolsFeitos int,
	GolsSofridos int,
	SaldoGols int,
	MaiorNroGols int,
	CONSTRAINT pkClassificacao PRIMARY KEY (IdTime),
	CONSTRAINT fkTime FOREIGN KEY (IdTime) REFERENCES Clube(Id)
)
GO

CREATE TABLE Partida 
(
	IdPartida int identity(1,1),
	IdTimeCasa int,
	GolsTimeCasa int,
	IdTimeVisitante int,
	GolsTimeVisitante int,
	Rodada int,
	CONSTRAINT ukPartida UNIQUE (IdPartida),
	CONSTRAINT pkPartida PRIMARY KEY (idTimeCasa, idTimeVisitante),
	CONSTRAINT fkTimeCasa FOREIGN KEY (IdTimeCasa) REFERENCES Clube(Id),
	CONSTRAINT fkTimeVisitante FOREIGN KEY (IdTimeVisitante) REFERENCES Clube(Id)
)
GO

CREATE OR ALTER PROCEDURE [dbo].[Inserir_Time](
	@Nome varchar(30),
	@Apelido varchar(30),
	@DtCriacao date
) AS
BEGIN
	DECLARE @IdTime int

	IF(SELECT COUNT(*) FROM Clube) = 5
	BEGIN
		SELECT 'Erro - Número máximo de times cadastrados atingido (5).'
	END
	ELSE
	BEGIN
		INSERT INTO CLUBE (NOME, APELIDO, DTCRIACAO)
		VALUES (@Nome, @Apelido, @DtCriacao)

		SELECT @IdTime = MAX(Id) FROM CLUBE

		INSERT INTO CLASSIFICACAO (IdTime, Pontuacao, GolsFeitos, GolsSofridos, MaiorNroGols, Vitorias, Empates, Derrotas, SaldoGols)
		VALUES (@IdTime, 0, 0, 0, 0, 0, 0, 0, 0)

		SELECT 'Time cadastrado com sucesso.'
	END
END
GO

CREATE OR ALTER TRIGGER TriggerMesmoTime ON Partida
INSTEAD OF INSERT
AS
BEGIN
	DECLARE @timeCasa int
	DECLARE @timeVisitante int
	DECLARE @golsCasa int
	DECLARE @golsVisitante int
	DECLARE @Rodada int
	
	IF (@TIMECASA = @TIMEVISITANTE)
	BEGIN
		RAISERROR('Erro - Não é possível inserir o mesmo time como Casa e Visitante.',10,1)
		RETURN;
	END
	ELSE
	BEGIN
		SELECT @timeCasa = IdTimeCasa, @golsCasa = GolsTimeCasa, @timeVisitante = IdTimeVisitante, @golsVisitante = GolsTimeVisitante, @Rodada = Rodada FROM INSERTED
		INSERT INTO Partida (IdTimeCasa, GolsTimeCasa, IdTimeVisitante, GolsTimeVisitante, Rodada)
		VALUES (@timeCasa, @golsCasa, @timeVisitante, @golsVisitante, @Rodada);
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ResetarTimes]
AS
BEGIN
	EXEC ResetarCampeonato
	DELETE FROM Classificacao
	DELETE FROM Clube
	DBCC CHECKIDENT ('[Clube]', RESEED, 0);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Inserir_Partida] 
(@IdTimeCasa int, @GolsCasa int, @IdTimeVisitante int, @GolsVisitante int, @Rodada int)
AS
BEGIN
	INSERT INTO Partida (IdTimeCasa, GolsTimeCasa, IdTimeVisitante, GolsTimeVisitante, Rodada)
	VALUES (@IdTimeCasa, @GolsCasa, @IdTimeVisitante, @GolsVisitante, @Rodada)

	-- ATUALIZA PONTUACAO
	IF (@GolsCasa > @GolsVisitante)
	BEGIN
		UPDATE CLASSIFICACAO
		SET Pontuacao = Pontuacao + 3,
			Vitorias = Vitorias + 1,
			SaldoGols = SaldoGols + (@GolsCasa - @GolsVisitante)
		WHERE IdTime = @IdTimeCasa

		UPDATE CLASSIFICACAO
		SET Derrotas = Derrotas + 1,
			SaldoGols = SaldoGols + (@GolsVisitante - @GolsCasa)
		WHERE IdTime = @IdTimeVisitante 
	END
	ELSE IF (@GolsCasa < @GolsVisitante)
	BEGIN
		UPDATE CLASSIFICACAO
		SET Pontuacao = Pontuacao + 5,
			Vitorias = Vitorias + 1,
			SaldoGols = SaldoGols + (@GolsVisitante - @GolsCasa)
		WHERE IdTime = @IdTimeVisitante

		UPDATE CLASSIFICACAO
		SET Derrotas = Derrotas + 1,
			SaldoGols = SaldoGols + (@GolsCasa - @GolsVisitante)
		WHERE IdTime = @IdTimeCasa 
	END
	ELSE
	BEGIN
		UPDATE CLASSIFICACAO
		SET Pontuacao = Pontuacao + 1,
			Empates = Empates + 1,
			SaldoGols = SaldoGols + (@GolsCasa - @GolsVisitante)
		WHERE IdTime = @IdTimeCasa

		UPDATE CLASSIFICACAO
		SET Pontuacao = Pontuacao + 1,
			Empates = Empates + 1,
			SaldoGols = SaldoGols + (@GolsVisitante - @GolsCasa)
		WHERE IdTime = @IdTimeVisitante

	END

	-- ATUALIZA SALDO DE GOLS REALIZADOS E SOFRIDOS
	UPDATE CLASSIFICACAO
	SET GolsFeitos = GolsFeitos + @GolsCasa,
		GolsSofridos = GolsSofridos + @GolsVisitante
	WHERE IdTime = @IdTimeCasa

	UPDATE CLASSIFICACAO
	SET GolsFeitos = GolsFeitos + @GolsVisitante,
		GolsSofridos = GolsSofridos + @GolsCasa
	WHERE IdTime = @IdTimeVisitante

	-- ATUALIZA MAIOR NRO DE GOLS DO TIME
	UPDATE CLASSIFICACAO
	SET MaiorNroGols = @GolsCasa
	WHERE IdTime = @IdTimeCasa
	AND MaiorNroGols < @GolsCasa

	UPDATE CLASSIFICACAO
	SET MaiorNroGols = @GolsVisitante
	WHERE IdTime = @IdTimeVisitante
	AND MaiorNroGols < @GolsVisitante

END
GO

CREATE OR ALTER PROCEDURE [dbo].[ResetarCampeonato]
AS
BEGIN
	DELETE FROM Partida
	DBCC CHECKIDENT ('[Partida]', RESEED, 0);
	UPDATE Classificacao 
	SET  Pontuacao = 0, Vitorias = 0, Empates = 0, Derrotas = 0, GolsFeitos = 0,
		GolsSofridos = 0, SaldoGols = 0, MaiorNroGols = 0
END
GO



CREATE PROCEDURE Relatorios_Gols 
(@Tipo integer)
AS
BEGIN

	IF @TIPO = 0
		BEGIN
		SELECT (B.Nome + ' (' + B.Apelido + ') ' + CONVERT(VARCHAR(10),B.DtCriacao,103)), A.GolsFeitos 
		FROM CLASSIFICACAO A JOIN CLUBE B ON A.IdTime = B.Id 
		WHERE A.GolsFeitos = (SELECT MAX(GolsFeitos) FROM CLASSIFICACAO) 
	END
	ELSE IF @TIPO = 1
	BEGIN
		SELECT (B.Nome + ' (' + B.Apelido + ') ' + CONVERT(VARCHAR(10),B.DtCriacao,103)), A.GolsSofridos 
		FROM CLASSIFICACAO A JOIN CLUBE B ON A.IdTime = B.Id
		WHERE A.GolsSofridos = (SELECT MAX(GolsSofridos) FROM CLASSIFICACAO)
	END
	ELSE IF @TIPO = 2
	BEGIN
		SELECT IdPartida, 
		(SELECT C.NOME FROM CLUBE C WHERE C.ID = IdTimeCasa) TimeCasa, 
		GolsTimeCasa,
		(SELECT C.NOME FROM CLUBE C WHERE C.ID = IdTimeVisitante) TimeVisitante, 
		GolsTimeVisitante,
		(GolsTimeCasa + GolsTimeVisitante) TotalGols
		FROM PARTIDA
		WHERE(GolsTimeCasa + GolsTimeVisitante) = (SELECT MAX((GolsTimeCasa + GolsTimeVisitante)) FROM Partida)
	END
	ELSE IF @TIPO = 3
	BEGIN
		SELECT A.IdTime, (B.Nome + ' (' + B.Apelido + ') ' + CONVERT(VARCHAR(10),B.DtCriacao,103)), A.MaiorNroGols, 
		ISNULL((SELECT TOP 1 idPartida FROM PARTIDA WHERE IdTimeCasa = IdTime AND GolsTimeCasa = MaiorNroGols), 
		(SELECT TOP 1 idPartida FROM PARTIDA WHERE IdTimeVisitante = IdTime AND GolsTimeVisitante = MaiorNroGols)) IdPartida 
		FROM CLASSIFICACAO A JOIN CLUBE B 
		ON A.IdTime = B.Id
	END
	ELSE IF @TIPO = 4
	BEGIN
		SELECT IdPartida, 
		(SELECT Nome + ' (' + Apelido + ') ' FROM Clube WHERE Clube.Id = Partida.IdTimeCasa), 
		CAST(GolsTimeCasa AS VARCHAR(2)), 
		CAST(GolsTimeVisitante AS VARCHAR(2)), 
		(SELECT Nome + ' (' + Apelido + ') ' FROM Clube WHERE Clube.Id = Partida.IdTimeVisitante) 
		FROM Partida
	END

END
GO

/*
-- Para testar
EXEC Inserir_Time 'Comédias FC','Coringas','2024-04-15'
EXEC Inserir_Time 'Sport Clube Five','5BY5','2013-08-01'
EXEC Inserir_Time 'Sons of Pestana','SoP','2016-12-31'
EXEC Inserir_Time 'Mortal Kombat Killers','MK Team','2018-04-01'
EXEC Inserir_Time 'Clube de Regatas do Café','Cafezinho Team','1993-06-10'
EXEC Inserir_Time 'Clube do Toninho','Toninho','2000-01-01'


EXEC Inserir_Partida 1, 1, 2, 3, 1
EXEC Inserir_Partida 2, 1, 1, 0, 1
EXEC Inserir_Partida 3, 5, 1, 3, 1
EXEC Inserir_Partida 4, 0, 1, 3, 1
EXEC Inserir_Partida 5, 5, 1, 5, 1

EXEC Inserir_Partida 1, 4, 3, 0, 2
EXEC Inserir_Partida 2, 0, 3, 3, 2
EXEC Inserir_Partida 3, 2, 2, 2, 2
EXEC Inserir_Partida 4, 2, 2, 3, 2
EXEC Inserir_Partida 5, 0, 2, 0, 2

EXEC Inserir_Partida 1, 1, 4, 1, 3
EXEC Inserir_Partida 2, 2, 4, 2, 3
EXEC Inserir_Partida 3, 0, 4, 0, 3
EXEC Inserir_Partida 4, 1, 3, 0, 3
EXEC Inserir_Partida 5, 3, 3, 3, 3

EXEC Inserir_Partida 1, 0, 5, 0, 4
EXEC Inserir_Partida 2, 4, 5, 2, 4
EXEC Inserir_Partida 3, 4, 5, 0, 4
EXEC Inserir_Partida 4, 1, 5, 1, 4
EXEC Inserir_Partida 5, 1, 4, 2, 4


SELECT TOP 1 idTime, Pontuacao FROM CLASSIFICACAO ORDER BY PONTUACAO DESC -- Campeao no final do campeonato
SELECT * FROM CLASSIFICACAO ORDER BY PONTUACAO DESC -- Os 5 primeiros times do campeonato
SELECT IdTime, GolsFeitos FROM CLASSIFICACAO WHERE GolsFeitos = (SELECT MAX(GolsFeitos) FROM CLASSIFICACAO) -- Time que mais fez gols no campeonato
SELECT IdTime, GolsSofridos FROM CLASSIFICACAO WHERE GolsSofridos = (SELECT MAX(GolsSofridos) FROM CLASSIFICACAO) -- Time que mais tomou gols no campeonato
SELECT TOP 1 *, (GolsTimeCasa + GolsTimeVisitante) As GolsTotais FROM Partida ORDER BY (GolsTimeCasa + GolsTimeVisitante) DESC -- Partida que mais teve gols
SELECT IdTime, MaiorNroGols FROM CLASSIFICACAO -- Maior nro de gols por time

*/