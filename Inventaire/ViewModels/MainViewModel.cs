using BillingManagement.Business;
using BillingManagement.Models;
using BillingManagement.UI.ViewModels.Commands;
using Inventaire;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace BillingManagement.UI.ViewModels
{
	class MainViewModel : BaseViewModel
    {

        //------------------------------------------------------------------Variables

        #region Variables
        private BaseViewModel _vm;

		private string searchCriteria;

		CustomerViewModel customerViewModel;
		InvoiceViewModel invoiceViewModel;

		private BillingManagementContext bd;
		private ObservableCollection<Customer> _customers;
		private ObservableCollection<Customer> _customersSearch;
		private ObservableCollection<Invoice> _invoices;

        #endregion

        //------------------------------------------------------------------Definitions

        #region Definitions

        #region -->BD et variables
        public BaseViewModel VM
		{
			get { return _vm; }
			set
			{
				_vm = value;
				OnPropertyChanged();
			}
		}

		public string SearchCriteria
		{
			get { return searchCriteria; }
			set { 
				searchCriteria = value;
				OnPropertyChanged();
			}
		}

		public BillingManagementContext Bd
		{
			get => bd;
			set
			{
				bd = value;
				OnPropertyChanged();
			}
		}
		public ObservableCollection<Customer> Customers
		{
			get => _customers;
			set
			{
				_customers = value;
				OnPropertyChanged();
			}
		}
		public ObservableCollection<Invoice> Invoices
		{
			get => _invoices;
			set
			{
				_invoices = value;
				OnPropertyChanged();
			}
		}
		#endregion

		//**********************************************

		#region -->RelayCommand & DelegateCommand
		//Modification App
		public DelegateCommand<object> ExitApp { get; set; }


		//Modification Data
		public DelegateCommand<object> AddNewItemCommand { get; private set; }
		public DelegateCommand<Customer> AddInvoiceToCustomerCommand { get; private set; }
		public DelegateCommand<object> GetAllCommand { get; private set; }  

		//Modification View
		public ChangeViewCommand ChangeViewCommand { get; set; }
		public DelegateCommand<Invoice> DisplayInvoiceCommand { get; private set; }
		public DelegateCommand<Customer> DisplayCustomerCommand { get; private set; }
		public RelayCommand<Customer> SearchCommand { get; private set; }
		
        #endregion

        #endregion

        //------------------------------------------------------------------Construction

        public MainViewModel()
		{
			//Initialisation de la BD et
			bd = new BillingManagementContext();
			InitValueBD();
			

			//RelayCommand et DelegateCommand
			ChangeViewCommand = new ChangeViewCommand(ChangeView);
			DisplayInvoiceCommand = new DelegateCommand<Invoice>(DisplayInvoice);
			DisplayCustomerCommand = new DelegateCommand<Customer>(DisplayCustomer);

			AddNewItemCommand = new DelegateCommand<object>(AddNewCustomer, CanAddNewCustomer);       //Ajoute customer vierge sans enregistrer
			AddInvoiceToCustomerCommand = new DelegateCommand<Customer>(AddInvoiceToCustomer);
			GetAllCommand = new DelegateCommand<object>(GetDataBD);
			SearchCommand = new RelayCommand<Customer>(SearchContact, CanSearchContact);
			ExitApp = new DelegateCommand<object>(Exit_Click);



			//Depart de l'app
			_customers = new ObservableCollection<Customer>();
			_invoices = new ObservableCollection<Invoice>();
			GetDataBD(null);
			customerViewModel = new CustomerViewModel(_customers, bd);
			invoiceViewModel = new InvoiceViewModel(_invoices);

			VM = customerViewModel;

		}

        //-----------------------------------------------------------------Methodes

        #region Methodes

        #region -->Methodes pour BD
        public void InitValueBD()
		{
			//Seeder la BD quand 1iere fois lance app (BD vide)
			if (bd.Customers.Count() == 0)
			{
				
				List<Customer> Customers = new CustomersDataService().GetAll().ToList();
				List<Invoice> Invoices = new InvoicesDataService(Customers).GetAll().ToList();
				
				foreach (Customer c in Customers)
					bd.Customers.Add(c);

				bd.SaveChanges();
			}	
			
		}
		
		
		public void GetDataBD(object item)
		{
			//Customers
			List<Customer> ListCustomers =  bd.Customers.ToList();
			Customers.Clear();
			foreach (Customer c in ListCustomers)
				Customers.Add(c);

			Customers = new ObservableCollection<Customer>(Customers.OrderBy(c => c.LastName));

			if (customerViewModel != null) customerViewModel.Customers = Customers;
			if (customerViewModel != null) customerViewModel.SelectedCustomer = Customers.First();
			
			

			//Invoices
			List<Invoice> ListInvoices = bd.Invoices.ToList();
			Invoices.Clear();
			foreach (Invoice i in ListInvoices)
				Invoices.Add(i);
			if (invoiceViewModel != null) invoiceViewModel.SelectedInvoice = Invoices.First();
		}

		#endregion


		#region -->Methodes pour RelayCommand / DelegateCommand
		private void Exit_Click(object item)
		{
			App.Current.Shutdown();
		}


		private void ChangeView(string vm)
		{
			switch (vm)
			{
				case "customers":
					VM = customerViewModel;
					break;
				case "invoices":
					VM = invoiceViewModel;
					break;
			}
		}


		private void DisplayInvoice(Invoice invoice)
		{
			invoiceViewModel.SelectedInvoice = invoice;
			VM = invoiceViewModel;
		}


		private void DisplayCustomer(Customer customer)
		{
			customerViewModel.SelectedCustomer = customer;
			VM = customerViewModel;
		}


		private void AddNewCustomer (object item)
		{
			if (VM == customerViewModel)
			{
				var c = new Customer();
				c.NewFlag = true;

				//Section Local
				customerViewModel.Customers.Add(c);
				customerViewModel.SelectedCustomer = c;
				MessageBox.Show("N'oublier pas d'enregistrer votre Client");

			}
		}
		private bool CanAddNewCustomer(object o)
		{
			bool result = false;

			result = VM == customerViewModel;
			return result;
		}


		private void AddInvoiceToCustomer(Customer c)
		{
			var invoice = new Invoice(c);
			c.Invoices.Add(invoice);
			bd.Invoices.Add(invoice);
			c.NewFlag = true;

			DisplayInvoice(invoice);
			MessageBox.Show("N'oublier pas d'enregistrer votre facture dans le Client");
		}


		private void SearchContact(object parameter)
		{
			#region -->Type de seach int(ID) || srtring(Name,LastName)
			Debug.WriteLine("search");

			string input = searchCriteria as string;
			int output;
			string searchMethod;
			if (!Int32.TryParse(input, out output))
				searchMethod = "name";
			else
				searchMethod = "id";
			#endregion

			switch (searchMethod)
			{
				#region -->Case int(ID)
				case "id":
					Debug.WriteLine("search id");

					Customers.Clear();
					customerViewModel.SelectedCustomer = bd.Customers.Find(output);

					if (customerViewModel.SelectedCustomer != null)
						Customers.Add(customerViewModel.SelectedCustomer);
					else
						MessageBox.Show("Aucun Id trouver");

					break;
				#endregion

				//------------------------------------------------

				#region -->Case string(Name || LastName)
				case "name":
					Debug.WriteLine("search name");

					List<Customer> MV_customers = new List<Customer>();
					Customer MV_selectedCustomer = new Customer();
					input = input.ToLower();
					MV_customers = bd.Customers.Where(c => (c.LastName.ToLower().StartsWith(input)) || (c.Name.ToLower().StartsWith(input))).ToList();
					Customers.Clear();

					if (MV_customers.Count > 0)
					{
						foreach (Customer c in MV_customers)
							Customers.Add(c);
						Customers = new ObservableCollection<Customer>(Customers.OrderBy(c => c.LastName));
						customerViewModel.Customers = Customers;
						customerViewModel.SelectedCustomer = Customers.First();
					}
					else
						MessageBox.Show("Aucun Name, LastName trouver");

					break;
				#endregion
				//-----------------------------------------------

				default:
					MessageBox.Show("Unkonwn search method");
					break;
			}
		}

		private bool CanSearchContact(object c)
		{
			if (VM == null) return false;
			return VM == customerViewModel;
		}
		#endregion

		#endregion
	}
}
