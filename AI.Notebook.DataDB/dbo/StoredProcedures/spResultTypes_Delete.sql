﻿CREATE PROCEDURE [dbo].[spResultTypes_Delete]
	@Id int = 0
AS
BEGIN
	SET NOCOUNT OFF;

	BEGIN TRY
		--Shouldn't do a complete delete, because it will leave remnants
		-- Delete from dbo.ResultTypes where [Id] = @Id
		Update dbo.ResultTypes
		Set [Active] = 0
		WHERE [Id]=@Id;

		return 1;
	END TRY
	BEGIN CATCH
		return 0;
	END CATCH
END