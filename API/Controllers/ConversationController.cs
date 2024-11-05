using Microsoft.AspNetCore.Mvc;
using AskMeAI.API.Entities;
using AskMeAI.API.Services;
using AskMeAI.API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AskMeAI.API.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly IMapper _mapper;

    public ConversationController(IConversationService conversationService, IMapper mapper)
    {
        _conversationService = conversationService;
        _mapper = mapper;
    }

    [HttpGet(Name = "GetConversationsByUserId")]
    public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversationsByUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }
        var conversations = await _conversationService.GetConversationsByUserIdAsync(Guid.Parse(userId));

        var conversationDtos = _mapper.Map<IEnumerable<ConversationDto>>(conversations);

        return Ok(conversationDtos);
    }

    [HttpGet("{id:guid}", Name = "GetConversationById")]
    public async Task<ActionResult<ConversationDto>> GetConversationById(Guid id)
    {
        var conversation = await _conversationService.GetConversationByIdAsync(id);
        if (conversation == null)
            return NotFound();

        var conversationDto = _mapper.Map<ConversationDto>(conversation);
        return Ok(conversationDto);
    }

    [HttpPost(Name = "CreateConversation")]
    public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] ConversationForCreationDto conversationForCreationDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var conversation = _mapper.Map<Conversation>(conversationForCreationDto);

        conversation.UserId = Guid.Parse(userId);

        var createdConversation = await _conversationService.AddConversationAsync(conversation);

        var createdConversationDto = _mapper.Map<ConversationDto>(createdConversation);

        return CreatedAtAction(nameof(GetConversationById), new { id = createdConversationDto.ConversationId }, createdConversationDto);
    }

    [HttpPut("{id:guid}", Name = "UpdateConversation")]
    public async Task<IActionResult> UpdateConversation(Guid id, [FromBody] ConversationForUpdateDto conversationForUpdateDto)
    {
        var updatedConversation = await _conversationService.UpdateConversationAsync(id, conversationForUpdateDto);
        if (updatedConversation == null)
            return NotFound();

        var updatedConversationDto = _mapper.Map<ConversationDto>(updatedConversation);
        return Ok(updatedConversationDto);
    }


    [HttpDelete("{id:guid}", Name = "DeleteConversation")]
    public async Task<IActionResult> DeleteConversation(Guid id)
    {
        var conversation = await _conversationService.GetConversationByIdAsync(id);
        if (conversation == null)
            return NotFound();

        await _conversationService.SoftDeleteConversationAsync(conversation);
        return NoContent();
    }
}
