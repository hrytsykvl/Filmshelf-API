using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IOfflineEvaluationService
{
    Task<EvaluationResultDTO> EvaluateMethodAsync(string method, EvaluationRequestDTO request);
    Task<List<EvaluationResultDTO>> EvaluateAllMethodsAsync(EvaluationRequestDTO request);
}
