using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using HappeningsDotNetC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HappeningsDotNetC
{
    public static class DependencyInjectionMappings
    {
        public static void Map(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IApiEntityService<Happening, HappeningDto>, HappeningService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IApiEntityService<Reminder, ReminderDto>, ReminderService>();
            services.AddScoped<IApiEntityService<User, UserDto>, UserService>();

            // with service specific extensions
            services.AddScoped<IInvitationEntityService, InvitationService>();
            services.AddScoped<IHappeningService, HappeningService>();            

            // mappings of the above services with the less visible interface
            services.AddScoped<IApiService<HappeningDto>, HappeningService>();
            services.AddScoped<IApiService<ReminderDto>, ReminderService>();
            services.AddScoped<IApiService<UserDto>, UserService>();
            services.AddScoped<IApiService<InvitationDto>, InvitationService>();
        }
    }
}
