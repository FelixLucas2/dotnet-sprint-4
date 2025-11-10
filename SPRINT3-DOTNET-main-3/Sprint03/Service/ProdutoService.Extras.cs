using Sprint03.Entidades;
namespace Sprint03.Service
{
    public partial class ProdutoService
    {
        public Task<List<Produto>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Produto?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    }
}
