using FilmShelf.API.Constants;
using FilmShelf.API.VMs;
using FilmShelf.BAL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace FilmShelf.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidEmailException ex)
        {
            await HandleCustomExceptionAsync(
                context,
                ErrorMessages.InvalidEmail,
                nameof(RegisterVM.Email),
                ex.Message,
                StatusCodes.Status400BadRequest);
        }
        catch (EmailInUseException ex)
        {
            await HandleCustomExceptionAsync(
                context,
                ErrorMessages.EmailInUse,
                nameof(RegisterVM.Email),
                ex.Message,
                StatusCodes.Status400BadRequest);
        }
        catch (InvalidUsernameException ex)
        {
            await HandleCustomExceptionAsync(
                context,
                ErrorMessages.InvalidUsername,
                nameof(RegisterVM.PersonName),
                ex.Message,
                StatusCodes.Status400BadRequest);
        }
        catch (InvalidPasswordException ex)
        {
            await HandleCustomExceptionAsync(
                context,
                ErrorMessages.InvalidPassword,
                nameof(RegisterVM.Password),
                ex.Message,
                StatusCodes.Status400BadRequest);
        }
        catch (InvalidLoginException ex)
        {
            await HandleCustomExceptionAsync(
                context,
                ErrorMessages.InvalidLogin,
                nameof(LoginVM.Email),
                ex.Message,
                StatusCodes.Status401Unauthorized);
        }
        catch (InvalidTokenException ex)
        {
            await HandleCustomExceptionAsync(
                context,
                ErrorMessages.InvalidToken,
                nameof(TokenVM.Token),
                ex.Message,
                StatusCodes.Status400BadRequest);
        }
        catch (InvalidRefreshTokenException ex)
        {
            await HandleCustomExceptionAsync(
                context,
                ErrorMessages.InvalidRefreshToken,
                nameof(TokenVM.RefreshToken),
                ex.Message,
                StatusCodes.Status400BadRequest);
        }
        catch (InvalidResetTokenException ex)
        {
            await HandleCustomExceptionAsync(
                context,
                ErrorMessages.InvalidResetToken,
                nameof(ResetPasswordVM.Token),
                ex.Message,
                StatusCodes.Status400BadRequest);
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "A database exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An unexpected error occurred.");
        }
    }

    private async Task HandleCustomExceptionAsync(
        HttpContext context,
        string title,
        string fieldName,
        string fieldError,
        int statusCode)
    {
        _logger.LogError($"An error occurred: {fieldName} - {fieldError}");

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Extensions =
            {
                ["errors"] = new Dictionary<string, List<string>>
                {
                    { fieldName, new() { fieldError } }
                }
            }
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
