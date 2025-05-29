using Repositories.Base;
using Repositories.DBContext;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FUNewsManagementContext _context;

        public IRepository<SystemAccount> SystemAccounts { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<NewsArticle> NewsArticles { get; }
        public IRepository<Tag> Tags { get; }

        public UnitOfWork(FUNewsManagementContext context)
        {
            _context = context;
            SystemAccounts = new Repository<SystemAccount>(_context);
            Categories = new Repository<Category>(_context);
            NewsArticles = new Repository<NewsArticle>(_context);
            Tags = new Repository<Tag>(_context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
