using AskMeAI.API.Models;
using FluentValidation;

namespace AskMeAI.API.Validators;
public class ConversationForUpdateDtoValidator : AbstractValidator<ConversationForUpdateDto>
{
    public ConversationForUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x)
            .Must(dto => !string.IsNullOrWhiteSpace(dto.Title) || dto.IsArchived != null)
            .WithMessage("At least one field (Title or IsArchived) must be provided for the update.");
    }
}
