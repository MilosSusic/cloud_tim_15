namespace SmartApiary.Application.Interfaces.Security
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
