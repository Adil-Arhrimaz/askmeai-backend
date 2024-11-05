using AskMeAI.API.DbContexts;
using Microsoft.EntityFrameworkCore;
using AskMeAI.API.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using AskMeAI.API.Services;
using AskMeAI.API.Middlewares;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AskMeAI.API.PromptGenerator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = $"https://cognito-idp.{builder.Configuration["AWS:Region"]}.amazonaws.com/{builder.Configuration["AWS:UserPoolId"]}";
    options.Audience = builder.Configuration["AWS:ClientId"];
    options.MetadataAddress = $"https://cognito-idp.{builder.Configuration["AWS:Region"]}.amazonaws.com/{builder.Configuration["AWS:UserPoolId"]}/.well-known/openid-configuration";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };

});

builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKernel();
builder.Services.AddOpenAIChatCompletion(builder.Configuration["OpenAI:Model"], builder.Configuration["OpenAI:Key"]);

builder.Services.AddDbContext<AskMeAiDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Add validators here
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<MessageForCreationDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ConversationForCreationDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ConversationForUpdateDtoValidator>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<ResumeAiPrompt>();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
