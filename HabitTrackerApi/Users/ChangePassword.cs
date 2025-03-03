namespace HabitTrackerApi.Users;

[ApiController]
public class ChangePassword(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Auth"], Summary = "Change password")]
    [HttpPut("/api/users/password")]
    public async Task<Result> ChangePasswordAsync([FromBody] ChangePasswordQuery changePasswordRequestBody)
    {
        return await mediator.Send(new ChangePasswordQuery() { Id = changePasswordRequestBody.Id, CurrentPassword = changePasswordRequestBody.CurrentPassword, NewPassword = changePasswordRequestBody.NewPassword });
    }

    public class ChangePasswordQuery : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordQuery>
    {
        public ChangePasswordValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();
            RuleFor(p => p.CurrentPassword)
                .NotEmpty();
            RuleFor(p => p.NewPassword)
                .NotEmpty();
        }
    }

    public class ChangePasswordQueryHandler(
        UserManager<User> userManager,
        IValidator<ChangePasswordQuery> validator,
        DataContext db)
        : IRequestHandler<ChangePasswordQuery, Result>
    {
        public UserManager<User> _userManager { get; set; } = userManager;
        public IValidator<ChangePasswordQuery> _validator { get; set; } = validator;
        public DataContext _db { get; set; } = db;

        public async Task<Result> Handle(ChangePasswordQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid == false)
            {
                return Result.BadRequest("The entered data is incorrect");
            }

            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            await _db.SaveChangesAsync(cancellationToken);

            if (!result.Succeeded)
            {
                return Result.BadRequest<Guid>("Hasło nie zostało zmienione");
            }

            return Result.Ok(request.Id);
        }
    }
}