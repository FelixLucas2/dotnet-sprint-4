using Microsoft.EntityFrameworkCore;
using Sprint03.Context;
using Sprint03.Repository;
using Sprint03.Service;
using Sprint03.Entidades;
using FluentAssertions;

public class ServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Usuario_CRUD_Works()
    {
        using var db = CreateDb();
        var repo = new UsuarioRepository(db);
        var service = new UsuarioService(repo);

        var created = await service.CreateAsync(new Usuario { Nome = "João", Email = "joao@ex.com" });
        created.Id.Should().BeGreaterThan(0);

        var fetched = await service.GetByIdAsync(created.Id);
        fetched!.Email.Should().Be("joao@ex.com");

        await service.UpdateAsync(created.Id, new ( "João Silva", "js@ex.com"));
        var updated = await service.GetByIdAsync(created.Id);
        updated!.Nome.Should().Be("João Silva");

        var ok = await service.DeleteAsync(created.Id);
        ok.Should().BeTrue();
        (await service.GetByIdAsync(created.Id)).Should().BeNull();
    }
}
