-- =============================================
-- Author:		Mason Makarwich
-- Create date: 2/20/23
-- Description:	Returns a filtered list of Medium records
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[GetMediaWithFiltersAndTagFilter] 
	@TagList varchar(100) = NULL, 
	@IncludeDeprecated bit = 0,
	@IncludeNonDeprDissociated bit = 0,
	@OriginId int = -1,
	@TypeId int = -1,
	@IsArchived bit = 0
AS
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #TagListTable (
		value varchar(50)
	)

	INSERT INTO #TagListTable
		SELECT value FROM STRING_SPLIT(@TagList, ',')

	CREATE TABLE #FirstMediaFilterResults (
		Id int,
		TypeId int,
		OriginId int,
		LocalPath varchar(50),
		ExtPath varchar(50),
		IsArchived bit
	)

	INSERT INTO #FirstMediaFilterResults
		SELECT m.Id, m.TypeId, m.OriginId, m.LocalPath, m.ExtPath, m.IsArchived
		FROM dbo.Media m
		LEFT JOIN dbo.MediumTag mt ON mt.MediumId = m.Id
		LEFT JOIN dbo.Tags t ON t.Id = mt.TagId
		WHERE (@TagList IS NULL OR t.Name IN (SELECT value FROM #TagListTable)) AND
			(@IncludeNonDeprDissociated = 1 OR (@IncludeNonDeprDissociated = 0 AND mt.IsDissociated = 0)) AND
			(@IncludeDeprecated = 1 OR (@IncludeDeprecated = 0 AND t.IsDeprecated = 0)) AND
			(@OriginId = -1 OR m.OriginId = @OriginId) AND
			(@TypeId = -1 OR m.TypeId = @TypeId) AND
			IsArchived = @IsArchived

	SELECT Id, TypeId, OriginId, LocalPath, ExtPath, IsArchived
	FROM #FirstMediaFilterResults fmf1
	WHERE (
		SELECT COUNT(Id) AS Count 
		FROM #FirstMediaFilterResults fmf2 
		WHERE fmf2.Id = fmf1.Id
	) = (
		SELECT COUNT(value)
		FROM #TagListTable
	)
	GROUP BY Id, TypeId, OriginId, LocalPath, ExtPath, IsArchived;

	DROP TABLE #TagListTable
	DROP TABLE #FirstMediaFilterResults
END