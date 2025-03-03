namespace HabitTrackerApi.Users;

[ApiController]
public class CreateUser(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Tags = ["Users"], Summary = "Create user")]
    [HttpPost("api/user")]
    public async Task<Result<CreateUserResult>> CreateUserAsync([FromBody] CreateUserDto createUserDto)
    {
        return await mediator.Send(new CreateUserDto { Email = createUserDto.Email, Password = createUserDto.Password });
    }
    public class CreateUserDto : IRequest<Result<CreateUserResult>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class CreateUserResult
    {
        public Guid Id { get; set; }
    }
    public class ChangePasswordQueryHandler(IUserService userService) : IRequestHandler<CreateUserDto, Result<CreateUserResult>>
    {
        public async Task<Result<CreateUserResult>> Handle(CreateUserDto request, CancellationToken cancellationToken)
        {
            var result = await userService.AddUser(request.Email, request.Password);

            if (result.Success)
            {
                return Result.Ok(new CreateUserResult { Id = result.Value });
            }
            else
            {
                return Result.BadRequest<CreateUserResult>(result.Errors.FirstOrDefault()?.Message ?? "Adding user failed");
            }
        }
    }
}