using BillingManagement.Models;
using BillingManagement.UI.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace BillingManagement.UI.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        //--------------------------------------------------------------Variables

        #region Variables
        private BillingManagementContext bdCVM;
        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        private Customer selectedCustomer = new Customer();
        
        #endregion

        //-------------------------------------------------------------Definitions

        #region Definitions
        
        public ObservableCollection<Customer> Customers
        {
            get => customers;
            set
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
        public RelayCommand<Customer> AddCustomerBDCommand { get; set; }
        #endregion

        //-------------------------------------------------------------Constructeur

        public CustomerViewModel(ObservableCollection<Customer> c, BillingManagementContext bd)
        {
            DeleteCustomerCommand = new RelayCommand<Customer>(DeleteCustomer, CanDeleteCustomer);
            AddCustomerBDCommand = new RelayCommand<Customer>(RegisterNewCustomer, CanRegisterNewCustomer); //Ajoute customer new dans OC et BD

            bdCVM = bd;
            Customers = c;
            selectedCustomer = customers.First();
        }

        //-------------------------------------------------------------Methodes

        #region Methodes

        private void RegisterNewCustomer(Customer c)
        {
            c.NewFlag = false;
            bdCVM.Update(c);
            bdCVM.SaveChanges();

            Customers = new ObservableCollection<Customer>(Customers.OrderBy(c => c.LastName));
            Debug.WriteLine("Save to BD");
            
        }

        private bool CanRegisterNewCustomer(Customer c)
        {
            if ((c == null) || (!c.NewFlag)) return false;

            return true;
        }



        private void DeleteCustomer(Customer c)
        {
            var currentIndex = Customers.IndexOf(c);

            if (currentIndex > 0) currentIndex--;
            SelectedCustomer = Customers[currentIndex];

            Customers.Remove(c);
            bdCVM.Remove(c);
            bdCVM.SaveChanges();
        }

        private bool CanDeleteCustomer(Customer c)
        {
            if ((c == null) || (c.NewFlag)) return false;

            return c.Invoices.Count == 0;
        }

        #endregion

    }
}
