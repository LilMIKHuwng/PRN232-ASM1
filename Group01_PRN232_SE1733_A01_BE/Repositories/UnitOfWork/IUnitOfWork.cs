using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<SystemAccount> SystemAccounts { get; }
        IRepository<Category> Categories { get; }
        IRepository<NewsArticle> NewsArticles { get; }
        IRepository<Tag> Tags { get; }
        Task<int> SaveChangesAsync();
    }
}
