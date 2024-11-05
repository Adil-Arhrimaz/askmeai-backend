using AskMeAI.API.Models;
using FluentValidation;

namespace AskMeAI.API.Validators;
public class MessageForCreationDtoValidator : AbstractValidator<MessageForCreationDto>
{
    public MessageForCreationDtoValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("ConversationId is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required when sender is a user.")
            .MaximumLength(500).WithMessage("Content must not exceed 500 characters.");

    }
}
