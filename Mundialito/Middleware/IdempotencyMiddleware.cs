using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using System.Text;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
    HttpContext context,
    IIdempotencyRepository repository,
    IUnitOfWork unitOfWork)
    {
        // Si no viene el header de idempotencia, dejamos pasar la request normalmente
        if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var keyValues))
        {
            await _next(context);
            return;
        }

        // Conversión explícita de StringValues a string limpio
        var key = keyValues.ToString();

        // Buscar si ya existe un registro con esta key
        var existing = await repository.GetByKeyAsync(key);
        if (existing is not null)
        {
            // Devolver la respuesta cacheada directamente al cliente
            context.Response.StatusCode = existing.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(existing.Response);
            return;
        }

        // Guardamos el stream original del cliente
        var originalBodyStream = context.Response.Body;

        using var memoryStream = new MemoryStream();

        // Redirigimos la respuesta al memory stream para poder leerla
        context.Response.Body = memoryStream;

        // Ejecutamos el siguiente middleware/endpoint
        await _next(context);

        // Leemos la respuesta generada
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        // Regresamos el stream al inicio para copiarlo al cliente
        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBodyStream);

        // Restauramos el body original
        context.Response.Body = originalBodyStream;

        // Solo cacheamos si la respuesta fue exitosa
        if (context.Response.StatusCode < 400)
        {
            var record = new IdempotencyRecord(
                key,
                context.Request.Path,
                responseBody,
                context.Response.StatusCode);

            await repository.AddAsync(record);
            await unitOfWork.CommitAsync();
        }
    }
}