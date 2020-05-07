using BillingManagement.Business;
using BillingManagement.Models;
using BillingManagement.UI.ViewModels.Commands;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace BillingManagement.UI.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        //--------------------------------------------------------------Variables

        #region Variables

        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        private Customer selectedCustomer = new Customer();

        #endregion

        //-------------------------------------------------------------Definitions

        #region Definitions
        
        public ObservableCollection<Customer> Customers
        {
            get => customers;
            private set
            {
                customers = value;
                OnPropertyChanged();
            }
        }

        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                selectedCustomer = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<Customer> DeleteCustomerCommand { get; private set; }

        #endregion

        //-------------------------------------------------------------Constructeur

        public CustomerViewModel(ObservableCollection<Customer> c)
        {
            DeleteCustomerCommand = new RelayCommand<Customer>(DeleteCustomer, CanDeleteCustomer);

            customers = c;
            selectedCustomer = customers.First();
        }

        //-------------------------------------------------------------Methodes

        #region Methodes

        private void DeleteCustomer(Customer c)
        {
            var currentIndex = Customers.IndexOf(c);

            if (currentIndex > 0) currentIndex--;

            SelectedCustomer = Customers[currentIndex];

            Customers.Remove(c);
        }

        private bool CanDeleteCustomer(Customer c)
        {
            if (c == null) return false;

            
            return c.Invoices.Count == 0;
        }




        #endregion
    }
}
