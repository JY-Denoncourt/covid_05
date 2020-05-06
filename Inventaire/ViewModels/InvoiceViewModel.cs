using BillingManagement.Business;
using BillingManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BillingManagement.UI.ViewModels
{
	class InvoiceViewModel : BaseViewModel
	{
        //----------------------------------------------------------------Variables

        #region Variables

        private Invoice selectedInvoice;
		private ObservableCollection<Invoice> invoices;

		#endregion

		//---------------------------------------------------------------Definitions

		#region Definitions

		public Invoice SelectedInvoice
		{
			get { return selectedInvoice; }
			set
			{
				selectedInvoice = value;
				OnPropertyChanged();
			}
		}


		public ObservableCollection<Invoice> Invoices
		{
			get => invoices;
			set
			{
				invoices = value;
				OnPropertyChanged();
			}
		}

        #endregion

        //---------------------------------------------------------------Constructeur

        public InvoiceViewModel(IEnumerable<Customer> customerData)
		{
			InvoicesDataService ids = new InvoicesDataService(customerData);
			Invoices = new ObservableCollection<Invoice>(ids.GetAll().ToList());
		}

		//---------------------------------------------------------------Methodes

		

		

	}
}
