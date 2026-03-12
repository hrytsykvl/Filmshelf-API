using FilmShelf.API.VMs;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;

namespace FilmShelf.API.MappingExtensions;
public static class MappingExtensions
{
    public static RegisterDTO ToRegisterDTO(this RegisterVM registerVM)
    {
        return new RegisterDTO
        {
            PersonName = registerVM.PersonName,
            Email = registerVM.Email,
            Password = registerVM.Password,
            ConfirmationPassword = registerVM.ConfirmationPassword
        };
    }

    public static LoginDTO ToLoginDTO(this LoginVM loginVM)
    {
        return new LoginDTO
        {
            Email = loginVM.Email,
            Password = loginVM.Password
        };
    }

    public static TokenModelDTO ToTokenModelDTO(this TokenVM tokenVM)
    {
        return new TokenModelDTO
        {
            Token = tokenVM.Token,
            RefreshToken = tokenVM.RefreshToken
        };
    }

    public static LogoutDTO ToLogoutDTO(this LogoutVM logoutVM)
    {
        return new LogoutDTO
        {
            RefreshToken = logoutVM.RefreshToken
        };
    }

    public static ForgotPasswordDTO ToForgotPasswordDTO(this ForgotPasswordVM forgotPasswordVM)
    {
        return new ForgotPasswordDTO
        {
            Email = forgotPasswordVM.Email,
            ResetPasswordUrl = forgotPasswordVM.ResetPasswordUrl
        };
    }

    public static ResetPasswordDTO ToResetPasswordDTO(this ResetPasswordVM resetPasswordVM)
    {
        return new ResetPasswordDTO
        {
            Email = resetPasswordVM.Email,
            Token = resetPasswordVM.Token,
            Password = resetPasswordVM.Password
        };
    }

    public static AuthenticationResponseVM ToAuthenticationResponseVM (
        this AuthenticationResponseDTO authenticationResponseDTO) {
        return new AuthenticationResponseVM
        {
            Token = authenticationResponseDTO.Token,
            RefreshToken = authenticationResponseDTO.RefreshToken,
            RefreshTokenExpirationDate = authenticationResponseDTO.RefreshTokenExpirationDate
        };
    }

    public static PasswordResponseVM ToPasswordResponseVM(this PasswordResponseDTO passwordResponseDTO)
    {
        return new PasswordResponseVM
        {
            Message = passwordResponseDTO.Message
        };
    }

    public static MovieListResponseVM ToMovieListResponseVM(
        this IEnumerable<MovieDTO> movies,
        int? totalPages = null)
    {
        var moviesVM = movies.Select(m => m.ToMovieResponseVM()).ToList();

        return new MovieListResponseVM
        {
            MovieList = moviesVM,
            TotalPages = totalPages ?? null
        };
    }

    public static MovieDetailsResponseVM ToMovieDetailsResponseVM(
        this MovieDetailsDTO movieDetailsDTO)
    {
        return new MovieDetailsResponseVM
        {
            Title = movieDetailsDTO.Title,
            Director = movieDetailsDTO.Director,
            Genres = movieDetailsDTO.Genres.Select(g => g.Name).ToList(),
            Overview = movieDetailsDTO.Overview,
            ReleaseDate = movieDetailsDTO.ReleaseDate,
            Runtime = movieDetailsDTO.Runtime,
            PosterPath = PhotoPathGenerator.GeneratePosterPath(movieDetailsDTO.PosterPath),
            AverageRating = movieDetailsDTO.AverageRating,
            Cast = movieDetailsDTO.Cast.Select(c => c.ToCastMemberVM()).ToList()
        };
    }

    public static ActorDetailsResponseVM ToActorDetailsResponseVM(
        this ActorDetailsDTO actorDetailsDTO)
    {
        return new ActorDetailsResponseVM
        {
            Name = actorDetailsDTO.Name,
            Bio = actorDetailsDTO.Bio,
            BirthDate = actorDetailsDTO.BirthDate,
            ProfilePath = actorDetailsDTO.ProfilePath,
            Movies = actorDetailsDTO.Movies.Select(m => m.ToActorMoviesVM()).ToList()
        };
    }

    public static WatchlistVM ToWatchlistVM(
        this WatchlistDTO watchlistDTO)
    {
        return new WatchlistVM
        {
            Id = watchlistDTO.Id,
            Movies = watchlistDTO.Movies.Select(m => m.ToWatchlistMovieVM()).ToList(),
            Title = watchlistDTO.Title,
            TotalMovies = watchlistDTO.TotalMovies,
            UpdatedAt = watchlistDTO.UpdatedAt
        };
    }

    public static List<WatchlistCheckVM> ToWatchlistCheckVMs(
        this List<WatchlistCheckDTO> watchlistCheckDTOs)
    {
        return watchlistCheckDTOs.Select(dto => new WatchlistCheckVM
        {
            WatchlistId = dto.WatchlistId,
            Title = dto.Title,
            MovieIds = new List<int>(dto.MovieIds)
        }).ToList();
    }

    public static ReviewAddDTO ToReviewAddDTO(
        this ReviewAddVM reviewAddVM,
        int userId)
    {
        return new ReviewAddDTO
        {
            Content = reviewAddVM.Content,
            Rating = reviewAddVM.Rating,
            UserId = userId,
            MovieId = reviewAddVM.MovieId
        };
    }

    public static ReviewVM ToReviewVM(
        this ReviewDTO reviewDTO)
    {
        return new ReviewVM
        {
            Id = reviewDTO.Id,
            Content = reviewDTO.Content,
            Rating = reviewDTO.Rating,
            CreatedAt = reviewDTO.CreatedAt,
            UserId = reviewDTO.UserId,
            MovieId = reviewDTO.MovieId,
            UserName = reviewDTO.UserName,
            MovieTitle = reviewDTO.MovieTitle,
            Responses = reviewDTO.Responses
                .Select(r => r.ToReviewResponseVM())
                .ToList()
        };
    }

    public static ReviewResponseAddDTO ToReviewResponseAddDTO(
        this ReviewResponseAddVM reviewResponseAddVM,
        int reviewId,
        int userId)
    {
        return new ReviewResponseAddDTO
        {
            Content = reviewResponseAddVM.Content,
            ReviewId = reviewId,
            UserId = userId
        };
    }

    public static ReviewResponseVM ToReviewResponseVM(
        this ReviewResponseDTO reviewResponseDTO)
    {
        return new ReviewResponseVM
        {
            Id = reviewResponseDTO.Id,
            ReviewId = reviewResponseDTO.ReviewId,
            Content = reviewResponseDTO.Content,
            CreatedAt = reviewResponseDTO.CreatedAt,
            UserId = reviewResponseDTO.UserId,
            UserName = reviewResponseDTO.UserName
        };
    }

    public static NotificationVM ToNotificationVM(
        this NotificationDTO reviewNotificationDTO)
    {
        return new NotificationVM
        {
            Id = reviewNotificationDTO.Id,
            CreatedAt = reviewNotificationDTO.CreatedAt,
            IsRead = reviewNotificationDTO.IsRead,
            UserId = reviewNotificationDTO.UserId,
            MovieId = reviewNotificationDTO.MovieId,
            MovieTitle = reviewNotificationDTO.MovieTitle,
            MoviePoster = reviewNotificationDTO.MoviePoster,
            ReviewResponse = reviewNotificationDTO.ReviewResponse?.ToReviewResponseVM() ?? null
        };
    }

    public static PopularMovieVM ToPopularMovieVM(
        this PopularMovieDTO popularMovieDTO)
    {
        return new PopularMovieVM
        {
            Id = popularMovieDTO.Id,
            Title = popularMovieDTO.Title,
            PosterPath = popularMovieDTO.PosterPath,
            AverageRating = popularMovieDTO.AverageRating,
            ReleaseDate = popularMovieDTO.ReleaseDate
        };
    }

    private static WatchlistMovieVM ToWatchlistMovieVM(
        this WatchlistMovieDTO watchlistMovieDTO)
    {
        return new WatchlistMovieVM
        {
            Id = watchlistMovieDTO.Id,
            Title = watchlistMovieDTO.Title,
            PosterPath = watchlistMovieDTO.PosterPath,
            AverageRating = watchlistMovieDTO.AverageRating
        };
    }

    private static ActorMoviesVM ToActorMoviesVM(
        this ActorMoviesDTO actorMoviesDTO)
    {
        return new ActorMoviesVM
        {
            Id = actorMoviesDTO.Id,
            Title = actorMoviesDTO.Title,
            PosterPath = actorMoviesDTO.PosterPath,
            Role = actorMoviesDTO.Role
        };
    }

    private static CastMemberVM ToCastMemberVM(
        this CastMemberDTO castMemberDTO)
    {
        return new CastMemberVM
        {
            Id = castMemberDTO.Id,
            Name = castMemberDTO.Name,
            Character = castMemberDTO.Character,
            ProfilePath = PhotoPathGenerator.GeneratePosterPath(castMemberDTO.ProfilePath)
        };
    }

    public static LlmRecommendationVM ToLlmRecommendationVM(
        this LlmRecommendationDTO dto)
    {
        return new LlmRecommendationVM
        {
            Movie = new MovieResponseVM
            {
                Id = dto.Movie.Id,
                Title = dto.Movie.Title,
                PosterPath = PhotoPathGenerator.GeneratePosterPath(dto.Movie.PosterPath)
            },
            Score = dto.Score,
            Reason = dto.Reason
        };
    }

    private static MovieResponseVM ToMovieResponseVM(
        this MovieDTO movieDTO)
    {
        return new MovieResponseVM
        {
            Id = movieDTO.Id,
            Title = movieDTO.Title,
            PosterPath = PhotoPathGenerator.GeneratePosterPath(movieDTO.PosterPath),
        };
    }

    public static EvaluationRequestDTO ToEvaluationRequestDTO(this EvaluationRequestVM vm)
    {
        return new EvaluationRequestDTO
        {
            K = vm.K,
            MinReviews = vm.MinReviews,
            RelevanceThreshold = vm.RelevanceThreshold
        };
    }

    public static EvaluationResultVM ToEvaluationResultVM(this EvaluationResultDTO dto)
    {
        return new EvaluationResultVM
        {
            Method = dto.Method,
            K = dto.K,
            EvaluatedUsers = dto.EvaluatedUsers,
            SkippedUsers = dto.SkippedUsers,
            HitRateAtK = dto.HitRateAtK,
            PrecisionAtK = dto.PrecisionAtK,
            RecallAtK = dto.RecallAtK,
            NdcgAtK = dto.NdcgAtK,
            Mrr = dto.Mrr,
            CatalogCoverage = dto.CatalogCoverage,
            EvaluationDurationSeconds = dto.EvaluationDurationSeconds,
            EvaluatedAt = dto.EvaluatedAt
        };
    }
}
