using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;

using DroidBackuper.NET.ViewModels.Helpers;
using DroidBackuper.NET.Classes.Helpers;

namespace DroidBackuper.NET.ViewModels
{
    class ViewModelBase : IDataErrorInfo, INotifyPropertyChanged, IClosableViewModel
    {
        #region Закрытие окна

        /// <summary>
        /// Событие на закрытие окна
        /// </summary>
        public event EventHandler CloseWindowEvent;

        /// <summary>
        /// Закрытие окна
        /// </summary>
        protected void CloseWindow()
        {
            if (this.CloseWindowEvent != null)
                this.CloseWindowEvent(this, null);
        }

        /// <summary>
        /// Окно О программе
        /// </summary>
        protected void AboutWindow()
        {
            // View.WPFAboutBox about = new View.WPFAboutBox();
            //about.ShowDialog();
        }

        #endregion

        #region Common Buttons
        /// <summary>
        /// Cancel
        /// </summary>
        public RelayCommand<Window> CancelCommandBtn { get; set; }

        private void CancelCommand(object param)
        {
            this.CloseWindow();
        }

        /// <summary>
        /// About
        /// </summary>
        public RelayCommand<Window> AboutCommandBtn { get; set; }

        private void AboutCommand(object param)
        {
            this.AboutWindow();

        }
        #endregion

        public ViewModelBase()
        {
            CancelCommandBtn = new RelayCommand<Window>(CancelCommand);
            AboutCommandBtn = new RelayCommand<Window>(AboutCommand);
        }

        #region Виртуальные методы
        protected virtual void RefreshAll() { }

        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// Событие обновления свойства
        /// </summary>
        /// <param name="prop">Наименование свойства</param>
        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDataErrorInfo
        protected Dictionary<string, Binder> ruleMap = new Dictionary<string, Binder>();

        public void AddRule<T>(Expression<Func<T>> expression, Func<bool> ruleDelegate, string errorMessage)
        {
            var name = CommonFunctions.GetPropertyName(expression);

            ruleMap.Add(name, new Binder(ruleDelegate, errorMessage));
        }

        public string Error
        {
            get
            {
                var errors = from b in ruleMap.Values where b.HasError select b.Error;

                return string.Join("\n", errors);
            }
        }

        public bool HasErrors
        {
            get
            {
                var values = ruleMap.Values.ToList();
                values.ForEach(b => b.Update());

                return values.Any(b => b.HasError);
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (ruleMap.ContainsKey(columnName))
                {
                    ruleMap[columnName].Update();
                    return ruleMap[columnName].Error;
                }
                return null;
            }
        }
        #endregion
    }
}
