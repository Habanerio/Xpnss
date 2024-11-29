using MediatR;

namespace Habanerio.Xpnss.UserProfiles.Domain.Interfaces;

public interface IUserProfilesQuery<out TResult> : IRequest<TResult>
{
    public string TimeZone { get; set; }
}

/*
 public interface IUserProfilesQuery<TResult> : IRequest<Result<TResult>>
   {
       public string TimeZone { get; set; }
   } 
*/