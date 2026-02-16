using FilmShelf.BAL.DTOs;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace FilmShelf.BAL.MLModels;
public class MovieRecommender
{
    private readonly MLContext _mlContext;
    private ITransformer _model;

    public MovieRecommender(IEnumerable<MovieRatingDTO> ratings)
    {
        _mlContext = new MLContext();

        var trainingDataView = _mlContext.Data.LoadFromEnumerable(ratings);

        var dataProcessingPipeline = _mlContext.Transforms.Conversion
            .MapValueToKey(nameof(MovieRatingDTO.UserId))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey(nameof(MovieRatingDTO.MovieId)));

        var trainingPipeline = dataProcessingPipeline.Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
            new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(MovieRatingDTO.UserId),
                MatrixRowIndexColumnName = nameof(MovieRatingDTO.MovieId),
                LabelColumnName = nameof(MovieRatingDTO.Label),
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                Alpha = 0.01,
                Lambda = 0.025,
                NumberOfIterations = 20,
                C = 0.00001
            }));

        _model = trainingPipeline.Fit(trainingDataView);
    }

    public List<int> RecommendForUser(int userId, IEnumerable<MovieRatingDTO> allRatings, int top = 5)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<MovieRatingDTO, MovieRatingPredictionDTO>(_model);

        var ratedMovieIds = allRatings
            .Where(r => r.UserId == userId)
            .Select(r => (int)r.MovieId)
            .ToHashSet();

        var allMovieIds = allRatings.Select(r => (int)r.MovieId).Distinct();

        var candidateMovieIds = allMovieIds.Except(ratedMovieIds);

        var scoredMovies = candidateMovieIds
            .Select(movieId => new
            {
                movieId,
                Score = predictionEngine.Predict(new MovieRatingDTO
                {
                    UserId = userId,
                    MovieId = movieId
                }).Score
            })
            .OrderByDescending(x => x.Score)
            .Take(top)
            .Select(x => x.movieId)
            .ToList();

        return scoredMovies;
    }
}
