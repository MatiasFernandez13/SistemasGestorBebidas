IF OBJECT_ID('dbo.sp_Reporte_Ventas', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Reporte_Ventas;
GO
CREATE PROCEDURE dbo.sp_Reporte_Ventas
    @Zona NVARCHAR(100) = NULL,
    @ProductoId INT = NULL,
    @Fecha DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT v.Fecha, dv.Cantidad, p.Nombre AS Producto, c.NombreCompleto AS Cliente, c.Zona
        FROM Ventas v
        INNER JOIN DetalleVentas dv ON dv.VentaId = v.Id
        INNER JOIN Productos p ON dv.ProductoId = p.Id
        INNER JOIN Clientes c ON v.ClienteId = c.Id
        WHERE (@Zona IS NULL OR c.Zona = @Zona)
          AND (@ProductoId IS NULL OR p.Id = @ProductoId)
          AND (@Fecha IS NULL OR CONVERT(DATE, v.Fecha) = @Fecha);
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000)=ERROR_MESSAGE(), @ErrSeverity INT=ERROR_SEVERITY();
        RAISERROR(@ErrMsg, @ErrSeverity, 1);
    END CATCH
END
GO
