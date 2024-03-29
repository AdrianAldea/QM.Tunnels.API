﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Tunnels.Core.Models;
using Tunnels.Core.Repositories;

namespace Tunnels.DAL.Repositories {
    public class ProductRepository : Repository<Product>, IProductRepository {
        public ProductRepository(TunnelsDbContext context)
            : base(context) {
        }

        public async Task<List<Product>> GetAllProductsAsync(bool? isActive) {
            if (isActive == null) {
                return await TunnelsDbContext.Products.ToListAsync();
            }
            else {
                return await TunnelsDbContext.Products.Where(x => x.IsActive == isActive).ToListAsync();
            }
        }

        public async Task DeleteById(int id) {
            var product = await TunnelsDbContext.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
            product.IsActive = false;
            TunnelsDbContext.Products.Update(product);
            await TunnelsDbContext.SaveChangesAsync();
        }

        public async Task UpdateAll(IEnumerable<Product> products)
        {
            // Update CurrentValue
            foreach (var product in products) 
            {
                product.CurrentValue = Math.Round(product.BuyPrice * product.CurrentQuantity, 2);
            }
            TunnelsDbContext.Products.UpdateRange(products);
            await TunnelsDbContext.SaveChangesAsync();
        }

        private TunnelsDbContext TunnelsDbContext => Context as TunnelsDbContext;
    }
}
