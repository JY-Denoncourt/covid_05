using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BillingManagement.Models
{
    public class Customer : INotifyPropertyChanged
    {
        #region Variables
        private int customerID;
        private string name;
        private string lastName;
        private string address;
        private string city;
        private string province;
        private string postalCode;
        private string picturePath;
        private string contactInfo;
        private ObservableCollection<ContactInfo> contactInfos = new ObservableCollection<ContactInfo>();
        private ObservableCollection<Invoice> invoices = new ObservableCollection<Invoice>();
        private bool newFlag;
        #endregion


        #region Property definitions
        public int CustomerID
        {
            get => customerID;
            private set
            {
                customerID = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Info));
            }
        }
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Info));
            }
        }
        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged();
            }
        }
        public string City
        {
            get => city;
            set
            {
                city = value;
                OnPropertyChanged();
            }
        }
        public string Province
        {
            get => province;
            set
            {
                province = value;
                OnPropertyChanged();
            }
        }
        public string PostalCode
        {
            get => postalCode;
            set
            {
                postalCode = value;
                OnPropertyChanged();
            }
        }
        public string PicturePath
        {
            get => picturePath;
            set
            {
                picturePath = value;
                OnPropertyChanged();
            }
        }
        public bool NewFlag
        {
            get => newFlag;
            set
            {
                newFlag = value;
                OnPropertyChanged();
            }
        }

        public string ContactInfo
        {
            get => contactInfo;
            set
            {
                contactInfo = value;
                OnPropertyChanged();
            }
        }

        public string Info => $"{LastName}, {Name}";
        

        public ObservableCollection<ContactInfo> ContactInfos
        {
            get => contactInfos;
            set
            {
                contactInfos = value;
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


        public Customer()
        {
            PicturePath = "images/user.png";
            NewFlag = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
