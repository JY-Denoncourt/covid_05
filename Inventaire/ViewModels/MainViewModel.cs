using BillingManagement.Business;
using BillingManagement.Models;
using BillingManagement.UI.ViewModels.Commands;
using Inventaire;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public ChangeViewCommand ChangeViewCommand { get; set; }

		public DelegateCommand<object> AddNewItemCommand { get; private set; }

		public DelegateCommand<Invoice> DisplayInvoiceCommand { get; private set; }

		public DelegateCommand<Customer> DisplayCustomerCommand { get; private set; }

		public DelegateCommand<Customer> AddInvoiceToCustomerCommand { get; private set; }

		public RelayCommand<Customer> SearchCommand { get; private set; }

		public DelegateCommand<object> ExitApp { get; set; }

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

			AddNewItemCommand = new DelegateCommand<object>(AddNewItem, CanAddNewItem);   //Ajoute un customer
			AddInvoiceToCustomerCommand = new DelegateCommand<Customer>(AddInvoiceToCustomer);
			SearchCommand = new RelayCommand<Customer>(SearchContact, CanSearchContact);
			ExitApp = new DelegateCommand<object>(Exit_Click);



			//Depart de l'app
			_customers = new ObservableCollection<Customer>();
			_invoices = new ObservableCollection<Invoice>();
			GetDataBD();
			customerViewModel = new CustomerViewModel(_customers);
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
		
		
		public void GetDataBD()
		{
			List<Customer> ListCustomers = bd.Customers.ToList();
			foreach (Customer c in ListCustomers)
				Customers.Add(c);

			List<Invoice> ListInvoices = bd.Invoices.ToList();
			foreach (Invoice i in ListInvoices)
				Invoices.Add(i);
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


		

        #endregion









		//A MODIFIER========================================================
        private void AddNewItem (object item)
		{
			if (VM == customerViewModel)
			{
				var c = new Customer();
				customerViewModel.Customers.Add(c);
				customerViewModel.SelectedCustomer = c;
			}
		}


		private bool CanAddNewItem(object o)
		{
			bool result = false;

			result = VM == customerViewModel;
			return result;
		}


		private void AddInvoiceToCustomer(Customer c)
		{
			var invoice = new Invoice(c);
			c.Invoices.Add(invoice);
			DisplayInvoice(invoice);
		}




		private void SearchContact(object parameter)
		{
			Debug.WriteLine("search");

			string input = searchCriteria as string;
			int output;
			string searchMethod;
			if (!Int32.TryParse(input, out output))
				searchMethod = "name";
			else
				searchMethod = "id";


			switch (searchMethod)
			{
				case "id":
					Debug.WriteLine("search id");

					


					/*
					Contacts.Clear();
					SelectedContact = PhoneBookBusiness.GetContactByID(output);

					if (SelectedContact != null)
						Contacts.Add(SelectedContact);
					else
						MessageBox.Show("Aucun Id trouver");
					*/
					break;

				//------------------------------------------------

				case "name":
					Debug.WriteLine("search name");

					//*******************************************
					//Seach dans la Observable collection de customerViewModel.Customers

					List<Customer> customers = new List<Customer>();
					Customer selectedCustomer = new Customer();

					customers = customerViewModel.Customers.ToList<Customer>();

					selectedCustomer = customers.Find(c => c.LastName == input);
					customers.Where(c => c.LastName.StartsWith(input) || c.Name.StartsWith(input));

					if (selectedCustomer != null)
					{

					}
					else
					{

					}


					/*
					Contacts = PhoneBookBusiness.GetContactByName(input);
					if (Contacts.Count > 0)
						SelectedContact = Contacts[0];
					else
						MessageBox.Show("Aucun Nom / Prenom trouver (Astuce: essayer lettre%");
					*/
					break;

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

		//-----------
		#endregion
	}
}
