using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
        .ForMember(d => d.Age, o=> o.MapFrom(c => c.DateOfBirth.CalculateAge()))
            .ForMember(d => d.PhotoUrl, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(s=>DateOnly.Parse(s));
        CreateMap<Message, MessageDto>()
            .ForMember(x=>x.RecipientPhotoUrl,
                o=> o.MapFrom(m=>m.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d=>d.SenderPhotoUrl,
                o=> o.MapFrom(u => u.Sender.Photos.FirstOrDefault(p=>p.IsMain)!.Url));

        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue
            ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
