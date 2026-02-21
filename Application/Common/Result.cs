using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

/// <summary>
/// Esto es una forma controlada de devolver resultados sin usar excepciones.
/// En vez de lanzar throw para todo, devolvemos éxito o error.
/// La prueba técnica exige NO usar excepciones como control de flujo.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    // Cuando todo sale bien
    public static Result Success()
        => new Result(true, null);

    // Cuando algo falla
    public static Result Failure(string error)
        => new Result(false, error);
}

/// <summary>
/// Versión genérica que devuelve un valor.
/// Por ejemplo, cuando creamos un equipo devolvemos su Id.
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
        => new Result<T>(true, value, null);

    public static new Result<T> Failure(string error)
        => new Result<T>(false, default, error);
}