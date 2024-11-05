using AskMeAI.API.Models;
using FluentValidation;

namespace AskMeAI.API.Validators;
public class ConversationForCreationDtoValidator : AbstractValidator<ConversationForCreationDto>
{
    public ConversationForCreationDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
    }
}
