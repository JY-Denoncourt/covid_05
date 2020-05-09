using BillingManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BillingManagement.UI
{
    public class BillingManagementContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(@"Data Source = BillingManagement.db");
        }

        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }


      
    }





    public static class DbContextExtensions
    {
        public static bool IsDirty(this DbContext context, object o )
        {
            Customer c = o as Customer;


            Contract.Requires<ArgumentNullException>(context != null);

            // Query the change tracker entries for any adds, modifications, or deletes.
            IEnumerable<EntityEntry> res = from e in context.ChangeTracker.Entries()
                                           where e.State.HasFlag(EntityState.Added) ||
                                               e.State.HasFlag(EntityState.Modified) ||
                                               e.State.HasFlag(EntityState.Deleted)
                                           select e;

            

            if (res.Any())
                return true;

            return false;

        }



        

    }
}
