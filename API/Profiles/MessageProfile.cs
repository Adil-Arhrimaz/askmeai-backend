using AutoMapper;
using AskMeAI.API.Entities;
using AskMeAI.API.Models;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageDto>();

        CreateMap<MessageForCreationDto, Message>()
            .ForMember(dest => dest.MessageId, opt => opt.Ignore());

    }
}