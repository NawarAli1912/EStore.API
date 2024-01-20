using Application;
using Infrastructure;
using Infrastructure.Persistence.DataSeed;
using Presentation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((context, loggerCofig) =>
    {
        loggerCofig.ReadFrom
            .Configuration(context.Configuration);
    });

    builder.Services
        .AddPresentation()
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    builder.Services.AddEndpointsApiExplorer();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using var scope = app.Services.CreateScope();

    var dbInit = scope.ServiceProvider.GetRequiredService<DbInit>();
    await dbInit.Init();

    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
