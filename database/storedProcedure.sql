
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE CasosAtivos (@idIC int)
AS
BEGIN
	SELECT IdCaso, Nome, Descricao, Objetivos, Relatorio, Terminado
	FROM  dbo.Caso AS C
	WHERE (idUtilizadorResponsavel = @idIC)
END
GO

