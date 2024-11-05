using AutoMapper;
using AskMeAI.API.Entities;
using AskMeAI.API.Models;

public class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationDto>();

        CreateMap<ConversationForCreationDto, Conversation>()
            .ForMember(dest => dest.ConversationId, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

        CreateMap<ConversationForUpdateDto, Conversation>()
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}