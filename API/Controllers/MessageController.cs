using Microsoft.AspNetCore.Mvc;
using AskMeAI.API.Entities;
using AskMeAI.API.Models;
using AskMeAI.API.Services;
using AutoMapper;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Authorization;
using AskMeAI.API.PromptGenerator;

namespace AskMeAI.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly Kernel _kernel;
    private readonly ResumeAiPrompt _resumeAiPrompt;

    public MessageController(IMessageService messageService, IMapper mapper, Kernel kernel, ResumeAiPrompt resumeAiPrompt)
    {
        _messageService = messageService;
        _mapper = mapper;
        _kernel = kernel;
        _resumeAiPrompt = resumeAiPrompt;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMessage([FromBody] MessageForCreationDto messageDto)
    {
        var userMessage = _mapper.Map<Message>(messageDto);
        userMessage.SenderType = SenderType.User;

        var createdUserMessage = await _messageService.AddMessageAsync(userMessage);

        var userMessageContent = messageDto.Content;
        var prompt = _resumeAiPrompt.BuildResumeAiPrompt(userMessageContent);

        var aiMessageContent = await _kernel.InvokePromptAsync<string>(prompt);

        var aiMessage = new Message
        {
            ConversationId = createdUserMessage.ConversationId,
            SenderType = SenderType.AI,
            Content = aiMessageContent,
        };


        var createdAiMessage = await _messageService.AddMessageAsync(aiMessage);

        var aiMessageToReturn = _mapper.Map<MessageDto>(createdAiMessage);

        return CreatedAtAction(nameof(GetMessagesForConversation), new { conversationId = createdUserMessage.ConversationId }, aiMessageToReturn);
    }

    [HttpGet("conversation/{conversationId:guid}")]
    public async Task<IActionResult> GetMessagesForConversation(Guid conversationId)
    {
        var messages = await _messageService.GetMessagesForConversationAsync(conversationId);

        var messagesDto = _mapper.Map<IEnumerable<MessageDto>>(messages);
        return Ok(messagesDto);
    }
}
